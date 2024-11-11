using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimulateLikeProducts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHttpClient<LeaderboardClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductApiUrl"]);
});

using IHost host = builder.Build();

LeaderboardClient leaderboardClient = host.Services.GetRequiredService<LeaderboardClient>();
await leaderboardClient.LikeProduct();


