using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Provisions an Azure SQL Database when published
var sql = builder.AddAzureSqlServer("sqlserver")
                 .AddDatabase("ProductContext");


var cache = builder.AddAzureRedis("cache");

var redisvss = builder.AddAzureRedis("redisvss");

var openai = builder.AddAzureOpenAI("openai")
                    .AddDeployment(new AzureOpenAIDeployment("chatModelDeployment", "gpt-4", "0613"))
                    .AddDeployment(new AzureOpenAIDeployment("textEmbeddingName", "text-embedding-ada-002", "2"));

var products = builder.AddProject<Products>("products")
                .WithExternalHttpEndpoints()
                .WithReference(sql)
                .WithReference(cache)
                .WithReference(redisvss)
                .WithReference(openai)
                .WaitFor(sql)
                .WaitFor(cache)
                .WaitFor(redisvss)
                .WaitFor(openai);

builder.AddProject<Store>("store")
       .WithExternalHttpEndpoints()
       .WithReference(cache)
       .WithReference(sql)
       .WithReference(redisvss)
       .WithReference(products)
       .WithReference(openai)
       .WaitFor(sql)
       .WaitFor(cache)
       .WaitFor(redisvss)
       .WaitFor(openai);

builder.Build().Run();