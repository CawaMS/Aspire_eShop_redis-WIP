using Azure;
using Azure.AI.OpenAI;
using Microsoft.IdentityModel.Tokens;
using NRedisStack.Search.Literals.Enums;
using NRedisStack.Search;
using StackExchange.Redis;
using static NRedisStack.Search.Schema;
using NRedisStack.RedisStackCommands;
using NRedisStack;
using DataEntities;

namespace Store.Services
{
    public class SimilarItemsService([FromKeyedServices("redisvss")] IConnectionMultiplexer redisConnectionMultiplexer, ProductService productService)
    {

        public async Task<List<Product>> GetSimilarItems(int id)
        {

            IDatabase _db = redisConnectionMultiplexer.GetDatabase();

            Product _product = await productService.GetProductById(id);

            SearchCommands ft = _db.FT();
            var descriptionEmbeddings = _db.HashGet("id:"+id, "description_embeddings");
            // search through the descriptions
            var res1 = ft.Search("vss_products",
                                new Query("*=>[KNN 2 @description_embeddings $query_vec]")
                                .AddParam("query_vec", descriptionEmbeddings)
                                .SetSortBy("__description_embeddings_score")
                                .Dialect(2));

            string _recommendation = "";
            List<Product> _recommendedProducts = new List<Product>();

            foreach (var doc in res1.Documents)
            {
                foreach (var item in doc.GetProperties())
                {
                    if (item.Key == "__description_embeddings_score")
                    {
                        Console.WriteLine($"id: {doc.Id}, score: {item.Value}");
                        Console.WriteLine("Item Name: " + _db.HashGet(doc.Id, "Name"));
                        Console.WriteLine("Item description: " + _db.HashGet(doc.Id, "description"));
                        Console.WriteLine();
                        if (!(doc.Id).Equals("id:"+_product.Id.ToString()))
                        {
                            _recommendation += $"id: {doc.Id}, score: {item.Value} " + " " +
                                                 "Item Name: " + _db.HashGet(doc.Id, "Name") + " "+
                                                "Item description: " + _db.HashGet(doc.Id, "description");

                            _recommendedProducts.Add(await productService.GetProductById(getId(doc.Id)));
                        }
                    }
                }
            }

            return _recommendedProducts;
        }

        private static int getId(string hashId)
        {
            string[] words = hashId.Split(':');
            return Int32.Parse(words[1]);
        }
    }
}
