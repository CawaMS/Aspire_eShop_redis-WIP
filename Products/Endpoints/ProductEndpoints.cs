﻿using DataEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using Products.Data;
using StackExchange.Redis;
using System.Text.Json;

namespace Products.Endpoints;

public static class ProductEndpoints
{
    private static readonly string CacheKeyPostFix = "Product";

    public static void MapProductEndpoints (this IEndpointRouteBuilder routes, IConnectionMultiplexer connectionMultiplexer)
    {
        var group = routes.MapGroup("/api/Product");
        IDatabase RedisDb = connectionMultiplexer.GetDatabase();


        group.MapGet("/", async (ProductDataContext db) =>
        {
            return await db.Product.ToListAsync();
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        //group.MapGet("/{id}", async  (int id, ProductDataContext db) =>
        group.MapGet("/getProductById", async([FromQuery]int id, ProductDataContext db) =>
        {
            var ProductString = ((await RedisDb.StringGetAsync($"{id}"+$"_{CacheKeyPostFix}")));
            if (!ProductString.Equals(RedisValue.Null))
            {
                string[] product_properties = ProductString.ToString().Split("_&_");
                // Product _product = JsonSerializer.Deserialize<Product>(ProductString.ToString());
                Product _product = new Product
                {
                    Id = int.Parse(product_properties[0]),
                    Name = product_properties[1],
                    Category = product_properties[2],
                    Description = product_properties[3],
                    Price = decimal.Parse(product_properties[4]),
                    ImageUrl = product_properties[5]
                };
                return Results.Ok(_product);
            }
            else 
            {
                Product product = await db.Product.AsNoTracking()
                               .FirstOrDefaultAsync(model => model.Id == id) is Product model ? model : throw new Exception("item not found");

                string product_properties = product.Id + "_&_" + product.Name + "_&_" + product.Category + "_&_" + product.Description + "_&_" + product.Price + "_&_" + product.ImageUrl;
                
                await RedisDb.StringSetAsync($"{id}"+$"_{CacheKeyPostFix}", product_properties);
                // await RedisDb.StringSetAsync($"{id}"+$"_{CacheKeyPostFix}",JsonSerializer.Serialize(product));



                return Results.Ok(model);
            }


        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/Category/{Category}", async (string Category, ProductDataContext db) =>
        {
            return await db.Product
                            .Where(model => model.Category == Category)
                            .ToListAsync();
        })
        .WithName("GetProductByCategory")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async  (int id, Product product, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Category, product.Category)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/", async (Product product, ProductDataContext db) =>
        {
            db.Product.Add(product);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Product/{product.Id}",product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async  (int id, ProductDataContext db) =>
        {
            var affected = await db.Product
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduct")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/PostVote", async([FromQuery] int Id, ProductDataContext db) =>
        {
            Console.WriteLine("Entered Product endpoints: MapPost(/PostVote)");
            await RedisDb.SortedSetIncrementAsync("productVoteSortedSet", Id, 1);
            return Results.Ok("Post request");
        })
        .WithName("PostProductVote")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/ProductLeaderboard", async(ProductDataContext db) =>
        {
            Console.WriteLine("Entered ProductEndpoints.cs /ProductLeaderboard");
            var productRanking = (await RedisDb.SortedSetRangeByRankWithScoresAsync("productVoteSortedSet", order:Order.Descending))
                                    .Select(x => new ProductRanking
                                    {
                                        ProductId = (int) x.Element,
                                        Score = (int)x.Score
                                    }).ToList();
            return productRanking;

        }).WithName("getProductLeaderboard")
        .Produces<List<ProductRanking>>(StatusCodes.Status200OK); ;
    }
}
