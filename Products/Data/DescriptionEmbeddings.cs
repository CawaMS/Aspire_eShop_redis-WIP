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
using OpenAI;

namespace Products.Data;

public class DescriptionEmbeddings()
{

    public static async Task GenerateEmbeddingsInRedis([FromKeyedServices("redisvss")]IConnectionMultiplexer redisConnectionMultiplexer, IConfiguration configuration, AzureOpenAIClient aoaiClient, ProductDataContext productContext)
    {
        Console.WriteLine("Entered GenerateEmbeddingsInRedis - Generating embeddings for products");

        IConnectionMultiplexer _redisConnectionMultiplexer = redisConnectionMultiplexer;
        IConfiguration _config = configuration;
        AzureOpenAIClient _aoaiClient = aoaiClient;
        ProductDataContext _productContext = productContext;
        IDatabase db = _redisConnectionMultiplexer.GetDatabase();
        string? embeddingsDeploymentName = _config.GetConnectionString("textembedding");
        IEnumerable<Product> productList = _productContext.Product.ToList();

        Console.WriteLine("Deployment name of ada text embedding: " + embeddingsDeploymentName);

        foreach (var _product in productList)
        {
            if ((db.HashGet("id:"+_product.Id, "description_embeddings").IsNullOrEmpty) && (_product.Description != null))
            {
                db.HashSet("id:"+_product.Id,
                [
                    new("Name", _product.Name),
                    new("Price", _product.Price.ToString()),
                    new("Category", _product.Category),
                    new("description", _product.Description),
                    new("description_embeddings",textToEmbeddings(_product.Description,_aoaiClient, embeddingsDeploymentName).SelectMany(BitConverter.GetBytes).ToArray())
                ]);
            }
        }

        SearchCommands ft = db.FT();
        // index each vector field
        try { ft.DropIndex("vss_products"); } catch { };
        Console.WriteLine("Creating search index in Redis");
        Console.WriteLine();
        ft.Create("vss_products", new FTCreateParams().On(IndexDataType.HASH).Prefix("id:"),
            new Schema()
            .AddTagField("Name")
            .AddVectorField("description_embeddings", VectorField.VectorAlgo.FLAT,
                new Dictionary<string, object>()
                {
                    ["TYPE"] = "FLOAT32",
                    ["DIM"] = 1536,
                    ["DISTANCE_METRIC"] = "L2"
                }
        ));
    }

    static float[] textToEmbeddings(string text, AzureOpenAIClient _openAIClient, string embeddingsDeploymentName)
    {
        var embeddingClient = _openAIClient.GetEmbeddingClient(embeddingsDeploymentName);
        return embeddingClient.GenerateEmbedding(text).Value.ToFloats().ToArray();
    }
}
