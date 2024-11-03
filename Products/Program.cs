using Aspire.StackExchange.Redis;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Products.Data;
using Products.Endpoints;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

// Add Redis cache, with Entra ID
var _redisHostName = builder.Configuration.GetConnectionString("cache");
var configurationOptions = await ConfigurationOptions.Parse($"{_redisHostName}").ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

Console.WriteLine($"Redis connection string: {_redisHostName}");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));

// Add services to the container.
var app = builder.Build();

app.MapDefaultEndpoints();

// Get Redis connection
var _redisConnectionMultiplexer = app.Services.GetRequiredService<IConnectionMultiplexer>();
//IDatabase db = _redisConnectionMultiplexer.GetDatabase();

// Configure the HTTP request pipeline.
app.MapProductEndpoints(_redisConnectionMultiplexer);

app.UseStaticFiles();

app.CreateDbIfNotExists();

app.Run();
