using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;


var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("sqlserver")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong@Passw0rd")
    .WithDataVolume()
    .WithBindMount("sql", "/docker-entrypoint-initdb.d");

var mongo = builder.AddMongoDB("mongodb")
    .WithDataVolume();


var mongoDb = mongo.AddDatabase("CatalogDb");

var warehouseDb = sqlserver.AddDatabase("WarehouseDb");
var orderManagementDb = sqlserver.AddDatabase("OrderManagementDb");

var redis = builder.AddRedis("redis")
    .WithDataVolume()
    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq",
        userName: builder.AddParameter("username", "admin", secret: true),
        password: builder.AddParameter("password", "admin123", secret: true))
    .WithManagementPlugin()
    .WithDataVolume();

var catalogService = builder.AddProject<Projects.Catalog_API>("catalogservice-api")
    .WithReference(mongoDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WaitFor(mongoDb)
    .WaitFor(redis)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 5002, name: "catalog-http")
    .WithHttpsEndpoint(port: 7048, name: "catalog-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

var orderManagementService = builder.AddProject<Projects.OrderManagementAPI>("ordermanagement-api")
    .WithReference(orderManagementDb)
    .WithReference(catalogService)
    .WaitFor(orderManagementDb)
    .WaitFor(catalogService)
    .WithHttpEndpoint(port: 5003, name: "ordermanagement-http")
    .WithHttpsEndpoint(port: 7047, name: "ordermanagement-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

var warehouseService = builder.AddProject<Projects.WarehouseAPI>("cartservice-api")
    .WithReference(redis)
    .WithReference(warehouseDb)
    .WithReference(rabbitmq)
    .WaitFor(redis)
    .WaitFor(warehouseDb)
    .WaitFor(rabbitmq)
    .WithHttpEndpoint(port: 5005, name: "warehouse-http")
    .WithHttpsEndpoint(port: 7050, name: "warehouse-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");


var aggregatorService = builder.AddProject<Projects.AggregatorService>("aggregatorservice-api")
    .WithReference(orderManagementService)
    .WithReference(catalogService)
    .WithReference(warehouseService)
    .WaitFor(orderManagementService)
    .WaitFor(catalogService)
    .WaitFor(warehouseService)
    .WithHttpEndpoint(port: 5004, name: "aggregator-http")
    .WithHttpsEndpoint(port: 7049, name: "aggregator-https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.ApiGateway>("gateway")
    .WithReference(catalogService)
    .WithReference(orderManagementService)
    .WithReference(warehouseService)
    .WithReference(aggregatorService)
    .WaitFor(orderManagementService)
    .WaitFor(catalogService)
    .WaitFor(warehouseService)
    .WaitFor(aggregatorService)
    .WithHttpEndpoint(port: 5000, name: "gateway-http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithHttpHealthCheck("/health");

builder.Build().Run();