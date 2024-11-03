using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Endpoints;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
//builder.Services.AddDbContext<ProductDataContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("ProductsContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

// builder.AddSqlServerDbContext<ProductDataContext>("ProductsContext");
builder.Services.AddDbContext<ProductDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

// Add Redis cache
builder.AddRedisClient("cache");

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
