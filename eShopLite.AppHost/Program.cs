using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Provisions an Azure SQL Database when published
var sqlServer = builder.AddSqlServer("sqlserver")
                       .PublishAsAzureSqlDatabase()
                       .AddDatabase("ProductContext");

var products = builder.AddProject<Products>("products")
                .WithExternalHttpEndpoints()
                .WithReference(sqlServer);

builder.AddProject<Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(products);

builder.Build().Run();