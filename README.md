# Improving .NET Aspire eShop web application using Azure Redis Services

This code repository is based on the [.NET eShop](https://github.com/dotnet/eShop) and [eShopLite](https://github.com/MicrosoftDocs/mslearn-dotnet-cloudnative-devops) projects.
The application code was refactored to highlight the usage of Azure Redis, and for the convenience of running performance testing.
Thanks to the colleagues who worked on the two reference projects!

## Pre-requisites
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [.NET 9 preview](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Register for Azure Managed Redis private preview access: 
    ```
    az login
    az account set --subscription <subscription id>
    az feature register --namespace "Microsoft.Cache" -n "may2024preview"
    ```
    check registration status with:
    ```
    az feature show --namespace "Microsoft.Cache" -n "may2024preview"
    ```
    

## Running the project in Azure using Azure Managed Redis (Preview) B1 
The B1 SKU is designed for quick dev/test scenario in Azure.

1. Download this repository
    ```
    git clone https://github.com/CawaMS/Aspire_eShop_redis-WIP.git
    ```
2. Change directory to the AppHost folder

    ```
    cd ./Aspire_eShop_redis-WIP/eShopLite.AppHost
    ```

3. Use Azure Developer CLI tp deploy to Azure
    ```
    azd up
    ```

4. Recommending to use 'North Europe' for the time being. All region, control plane, and performance optimziations roll out will finish on Thursday Nov 14. 

## Running the project in Azure using other Azure Redis service SKUs

To clone a branch use:
```
git clone -b <branch> https://github.com/CawaMS/Aspire_eShop_redis-WIP.git
```
The follow branches contains the customized bicep files generated from `azd infra synth` command.
The `infra` folder can be found in the `eShopLite.AppHost` folder
- OSS Basic C3 (~5G) with Entra ID: [azd_BasicC3_EntraID](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/azd_BasicC3_EntraID)
- OSS Basic C3 (~5G)  with automatically populated connection strings in key vault: [azd_BasicC3_ConnectionString](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/azd_BasicC3_ConnectionString)
- Azure Managed Redis (Preview) B5 (~5G) with High Availability using automatically populated connection strings in Key Vault: [azd_AmrB5-HA_ConnectionString](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/azd_AmrB5-HA_ConnectionString)
- Azure Managed Redis (Preview) B5 (~5G) without High Availability using automatically populated connection strings in Key Vault: [azd_AmrB5-HA_ConnectionString](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/azd_AmrB5-noHA_ConnectionString)
- A generic project that takes any user-provided connection string: [azure_publishRedisAsConnectionString](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/azure_publishRedisAsConnectionString)
- .NET 9 Hybrid Cache preview: [dotnet9](https://github.com/CawaMS/Aspire_eShop_redis-WIP/tree/dotnet9)

## Features to demo

### Output Cache
The eShop product catalog is using Output Cache for fast access. NOTE that this only works without user logged.
Code is at [Program.cs line 32](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/d50eee66de22dbe0e392b26869f3f0a3ca251f06/Store/Program.cs#L32) and [Products.razor line 7](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/d50eee66de22dbe0e392b26869f3f0a3ca251f06/Store/Components/Pages/Products.razor#L7)

### Entra ID
The eShop Redis Cache connections are using Entra ID for secure, passwordless auth. Code example at [Program.cs line 35](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/4e956d0be933ace1c6eca03b8afa783b7fddf07e/Store/Program.cs#L35)

### Shopping Cart
The eShop uses Redis to store volatile data like Shopping Cart. Code is at [CartService.cs](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/main/Store/Services/CartService.cs)

### Leaderboard
The eShop uses Redis SortedSet to save leaderboard of top liked items. Click on the "like" button at the product overview page for a few items and multiple times. The *Top Products* page would display the sorted items.

**TODO** Add an application to automatically simulate a lot of users voting in real time.

### Distributed Cache
The getProductById is an important page where users can add an item to the shopping cart. The code for doing so in .NET 8 is at [ProductEndpoints.cs line 32](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/d50eee66de22dbe0e392b26869f3f0a3ca251f06/Products/Endpoints/ProductEndpoints.cs#L32) and for .NET 9 is at [ProductEndpoints.cs line L32](https://github.com/CawaMS/Aspire_eShop_redis-WIP/blob/a0c9f40518b6c04af9868d7d3a8b9191d0201901/Products/Endpoints/ProductEndpoints.cs#L32). 
**Noticing the simplified programming interface for .NET 9 Hybrid Cache!**

To test the performance with more product items and users at scale, we will upload test data and create Azure Load Test.

#### Upload 5000 test data

1. Change directory to the TestData folder
```
cd Aspire_eShop_redis-WIP/TestData
```

2. Set user secrets to point to the Products service API and run the application. Uploading 5000 products may take a while
```
dotnet user-secrets init  
dotnet user-secrets set ProductApiUrl https://products.<domainName>.<region>.azurecontainerapps.io/api/product
dotnet build
dotnet run
```

#### Hydrate cache

Skip this step for **SQL** only environment

1. Change directory to the TestData folder
```
cd ..
cd PreloadRedis
```

2. Set user secrets to point to the Products service API and run the application. Hydrate the cache may take a while.
```
dotnet user-secrets init  
dotnet user-secrets set ProductApiUrl https://products.<domainName>.<region>.azurecontainerapps.io/api/product
dotnet build
dotnet run
```

#### Create Azure Loadtest
1. Create an [Azure Load Test](https://learn.microsoft.com/azure/load-testing/overview-what-is-azure-load-testing)
2. Create a test following the instructions below.

- Enter test name and description.
![Basic info](./images/LoadTest1.png)

- Configure test plan. 
    - Enter the getProductById URL as the target URL. For example: https://products.youracadomaname.northeurope.azurecontainerapps.io/api/product/getProductById
    - Enter the Query parameters. Enter id for **Name**, ${id} for **Value**
![Add a request](./images/LoadTest2.png)

- Upload the parameter file
    - Browse to the **Aspire_eShop_redis-WIP/AzureLoadTest** folder. Select the **input.csv** file. 
    - Click the **Upload** button
    - Enter **id** as variable
    - Check **Split CSV evenly between Test engines**
![Upload parameter input file](./images/LoadTest3.png)

- Skip the **Parameters** tab

- On the **Load** Tab, select 4 engines, 250 vUsers each, Test duration to be 7 minutes, Ramp up time to be 2 min.
![Configure load](./images/LoadTest4.png)

- Skip **Monitoring**

- Configure **Test Criteria** so the report contains P95 and P99 latency, which are common metrics to care about.

![Configure test criteria](./images/LoadTest5.png)

- After the test is created, it will start the first run

#### Run test with multiple environments
Repeat the same process for all the environments you want to run. Suggested:
- Compare **SQL** only environment with **OSS Basic B3** to show that adding Redis makes the response times faster.
- Compare **OSS Basic B3** to **AMR Balanced B5 without HA**. Scenario is for around 5GB cache for getting started scenarios.






