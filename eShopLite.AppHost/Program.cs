using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Provisions an Azure SQL Database when published
var sql = builder.AddSqlServer("sqlserver")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("ProductContext");


var cache = builder.AddRedis("cache")
                    .PublishAsAzureRedis();

var products = builder.AddProject<Products>("products")
                .WithExternalHttpEndpoints()
                .WithReference(sql)
                .WithReference(cache);

builder.AddProject<Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(cache)
       .WithReference(sql)
       .WithReference(products);

builder.Build().Run();