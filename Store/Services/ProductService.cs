﻿using DataEntities;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Store.Services;

public class ProductService
{
    HttpClient httpClient;
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
    {
        _logger = logger;
        this.httpClient = httpClient;
    }
    public async Task<List<Product>> GetProducts()
    {
        List<Product>? products = null;
        try
        {
            var response = await httpClient.GetAsync("/api/Product/Category/Gear");
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Http status code: {response.StatusCode}");
            _logger.LogInformation($"Http response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                products = await response.Content.ReadFromJsonAsync(ProductSerializerContext.Default.ListProduct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GetProducts.");
        }

        return products ?? new List<Product>();
    }

    public async Task<Product> GetProductById(int id)
    {
        Product? product = null;

        try
        {
            var response = await httpClient.GetAsync($"/api/Product/getProductById?id={id}");
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Http status code: {response.StatusCode}");
            _logger.LogInformation($"Http response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                product = await response.Content.ReadFromJsonAsync(ProductSerializerContext.Default.Product);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GetProduct.");
        }

        return product ?? new Product();
    }

    public async Task<ProductRanking[]> GetProductRankings()
    {
        Console.WriteLine("Entered ProductService.cs GetProductRankings()");
        List<ProductRanking>? productRanking = null;

        await foreach (var pR in httpClient.GetFromJsonAsAsyncEnumerable<ProductRanking>("/api/Product/ProductLeaderboard"))
        {
            if (pR is not null)
            {
                productRanking ??= [];
                productRanking.Add(pR);
            }
        }

        Console.WriteLine("Executed ProductService.cs GetProductRankings()");
        return productRanking?.ToArray() ?? [];
    }

    public async Task PostProductRankingAsync(int productId)
    {
        Console.WriteLine("Entered Product HTTPClient Service: PostProductRankingAsync");
        await httpClient.PostAsync($"/api/Product/PostVote/?id={productId}", new StringContent(JsonSerializer.Serialize(productId), Encoding.UTF8, "application/json"));
    }
}
