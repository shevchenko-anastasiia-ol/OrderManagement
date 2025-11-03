using Catalog.API.Controllers;
using Catalog.API.Middleware;
using Catalog.Application.Behaviours;
using Catalog.Application.Queries.Category.GetRecentCategories;
using MongoDB.Driver;
using Catalog.Domain.Exceptions;
using Catalog.Infrastructure.Repositories;
using Catalog.Application.Services;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.Repositories;
using Catalog.Domain.Interfaces.Services;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Indexes;
using Catalog.Infrastructure.Seeding;
using FluentValidation;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// --- MongoDB settings ---
var mongoConn = builder.Configuration.GetConnectionString("CatalogDb")
               ?? builder.Configuration.GetSection("MongoDbSettings").GetValue<string>("ConnectionString");

builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = mongoConn;
    options.DatabaseName = "catalog-db";
    options.MaxConnectionPoolSize = 100;
    options.MinConnectionPoolSize = 5;
    options.ConnectTimeoutSeconds = 10;
    options.SocketTimeoutSeconds = 10;
});

builder.Services.PostConfigure<MongoDbSettings>(options =>
{
    if (string.IsNullOrEmpty(options.ConnectionString))
        throw new InvalidOperationException("MongoDB ConnectionString is required");
    if (string.IsNullOrEmpty(options.DatabaseName))
        throw new InvalidOperationException("MongoDB DatabaseName is required");
});

// --- MongoDB context and unit of work ---
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<MongoDbContext>();
    return context.Database; // повертаємо IMongoDatabase
});
/*builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new MongoUnitOfWork(context.Database);
});*/

// --- Repositories ---
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// --- Services ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddSingleton<IIndexCreation, MongoIndexCreation>();
builder.Services.AddSingleton<IDataSeeder, DatabaseSeeder>();


// --- MediatR with pipeline behaviors ---
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(BaseApiController).Assembly);
    cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.RegisterServicesFromAssemblies(typeof(GetRecentCategoriesQueryHandler).Assembly);
});

// --- Validators ---
builder.Services.AddValidatorsFromAssembly(typeof(BaseApiController).Assembly);

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1"
    });
});





// --- Health checks ---
builder.Services.AddHealthChecks()
    .AddMongoDb(
    sp => new MongoClient(mongoConn), // делегат, що повертає IMongoClient
    name: "catalog-db",               // ім'я HealthCheck
    timeout: TimeSpan.FromSeconds(5)
);

// --- Build app ---
var app = builder.Build();

// --- Initialize Mongo indexes (if any) ---
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetRequiredService<IIndexCreation>();
    await indexService.CreateIndexesAsync();
    
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
    // Якщо будуть індекси або початкові дані, додай тут
    // var indexService = scope.ServiceProvider.GetRequiredService<IIndexCreationService>();
    // await indexService.CreateIndexesAsync();
}

// --- Middleware ---
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1");
        c.RoutePrefix = string.Empty; // Swagger на корінь сервера: http://localhost:5206/
    });
}


app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// --- Map endpoints ---
app.MapHealthChecks("/health");
app.MapControllers();

// --- Run ---
await app.RunAsync();
