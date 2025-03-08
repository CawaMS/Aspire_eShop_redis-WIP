using Store.Components;
using Store.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Components.Account;
using Store.Data;
using Azure.Identity;
using StackExchange.Redis.Configuration;
using StackExchange.Redis;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductContext") ?? throw new InvalidOperationException("Connection string 'ProductsContext' not found.")));

builder.AddServiceDefaults();

builder.Services.AddHttpClient<ProductService>(client =>
{
    client.BaseAddress = new("https+http://products");
});

builder.Services.AddScoped<ICartService, CartServiceCache>();

builder.Services.AddSingleton<SimilarItemsService>();

builder.Services.AddSingleton<ChatService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var configurationOptions = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("cache") ?? throw new InvalidOperationException("Could not find a 'cache' connection string."));

await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

builder.AddKeyedRedisClient(name: "cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});

builder.AddRedisOutputCache("cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});

//Add session

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(14);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.AddRedisDistributedCache("cache", configureOptions: options =>
{
    options.Defaults = configurationOptions.Defaults;
});

var configurationOptionsRedisVss = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redisvss") ?? throw new InvalidOperationException("Could not find a 'redisvss' connection string."));
await configurationOptionsRedisVss.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

builder.AddKeyedRedisClient(name: "redisvss", configureOptions: options =>
{
    options.Defaults = configurationOptionsRedisVss.Defaults;
});

builder.AddAzureOpenAIClient("openai", configureSettings: settings => { 
    settings.Credential = new DefaultAzureCredential();
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityUserAccessor>();

builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<StoreUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<UserContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IEmailSender<StoreUser>, IdentityNoOpEmailSender>();

builder.Services.AddBlazorBootstrap();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UserContext>();
    context.Database.Migrate();
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

 app.UseSession();

app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();;

app.Run();
