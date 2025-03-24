using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Products.Data;
using Products.Endpoints;
using StackExchange.Redis;
using StackExchange.Redis.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

var configurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("cache") ?? throw new InvalidOperationException("Could not find a 'cache' connection string."));

await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

builder.AddKeyedRedisClient(name:"cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache(
   options =>
   {
       // options.MaximumPayloadBytes = 1024 * 1024;
       // options.MaximumKeyLength = 1024;
       options.DefaultEntryOptions = new HybridCacheEntryOptions
       {
           Expiration = TimeSpan.FromHours(2),
           LocalCacheExpiration = TimeSpan.FromMinutes(15)
       };
   });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddStackExchangeRedisCache(options =>
{
    // options.Configuration = builder.Configuration.GetConnectionString("cache") ?? throw new InvalidOperationException("Connection string 'cache' not found.");
    options.ConfigurationOptions = configurationOptions;
    options.InstanceName = "SampleInstance";
});

var configurationOptionsRedisVss = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redisvss") ?? throw new InvalidOperationException("Could not find a 'redisvss' connection string."));
await configurationOptionsRedisVss.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

builder.AddKeyedRedisClient(name:"redisvss", configureOptions: options =>
{
    options.Defaults = configurationOptionsRedisVss.Defaults;
});

builder.AddAzureOpenAIClient("openai", configureSettings: settings => {
    settings.Credential = new DefaultAzureCredential();
});

builder.Services.AddSingleton<DescriptionEmbeddings>();

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Get cache Redis connection
var _redisConnectionMultiplexer = app.Services.GetKeyedService<IConnectionMultiplexer>("cache");

var _hybridCache = app.Services.GetRequiredService<HybridCache>();

// Configure the HTTP request pipeline.
app.MapProductEndpoints(_redisConnectionMultiplexer, _hybridCache);

app.UseStaticFiles();

app.CreateDbIfNotExists();

//var AOAIClient = app.Services.GetRequiredService<AzureOpenAIClient>();
//Console.WriteLine("Test AOAI Connection: " + AOAIClient.ToString());

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Entered service scope for generating embeddings in Program.cs");
    // Get redis connection for VSS
    var _redisConnectionMultiplexerVSS = app.Services.GetKeyedService<IConnectionMultiplexer>("redisvss");
    var _configuration = app.Services.GetRequiredService<IConfiguration>();
    var _aoaiClient = app.Services.GetRequiredService<AzureOpenAIClient>();
    var _productContext = app.Services.GetRequiredService<ProductDataContext>();
    await DescriptionEmbeddings.GenerateEmbeddingsInRedis(_redisConnectionMultiplexerVSS, _configuration, _aoaiClient, _productContext);
}

app.Run();
