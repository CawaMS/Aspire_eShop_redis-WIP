using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestData;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHttpClient<ProductHTTPClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductApiUrl"]);
});

builder.Services.AddSingleton<UploadData>();

using IHost host = builder.Build();

UploadData uploadData = host.Services.GetRequiredService<UploadData>();

await uploadData.UploadCsvData("testData.csv");

