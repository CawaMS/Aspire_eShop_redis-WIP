using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PreloadRedis;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHttpClient<ProductHTTPClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductApiUrl"]);
});

using IHost host = builder.Build();

ProductHTTPClient productHTTPClient = host.Services.GetRequiredService<ProductHTTPClient>();
await productHTTPClient.ReadProduct();


