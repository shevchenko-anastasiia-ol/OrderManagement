using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using WarehouseBLL;
using WarehouseDAL;
using WarehouseDAL.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "WarehouseAPI")
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Warehouse API");

    builder.Services.AddDataAccess(configuration);
    builder.Services.AddBusinessServices(configuration);
    builder.Services.AddAutoMapper(typeof(WarehouseBLL.MappingProfiles.InventoryProfile).Assembly);
    
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Warehouse API",
            Version = "v1",
            Description = "Warehouse Management System API"
        });
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    var app = builder.Build();

    // Database seeding
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
            var seeder = new WarehouseSeedDb(dbContext);
            await seeder.SeedAsync();
            Log.Information("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse API V1");
            c.RoutePrefix = string.Empty; // Swagger UI at root
        });
    }
    else
    {
        // Global exception handler for production
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var correlationId = context.TraceIdentifier;
                Log.Error(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

                int statusCode = exception switch
                {
                    WarehouseBLL.Exceptions.NotFoundException => (int)HttpStatusCode.NotFound,
                    WarehouseBLL.Exceptions.BadRequestException => (int)HttpStatusCode.BadRequest,
                    WarehouseBLL.Exceptions.ConflictException => (int)HttpStatusCode.Conflict,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = statusCode;

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = GetTitleForStatusCode(statusCode),
                    Detail = app.Environment.IsDevelopment() ? exception?.Message : "An error occurred processing your request.",
                    Instance = context.Request.Path,
                    Type = $"https://httpstatuses.com/{statusCode}"
                };

                problemDetails.Extensions["traceId"] = correlationId;
                problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application started successfully");
    app.Run();
    
    
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

string GetTitleForStatusCode(int statusCode) => statusCode switch
{
    404 => "Resource Not Found",
    400 => "Bad Request",
    409 => "Conflict",
    500 => "Internal Server Error",
    _ => "An error occurred"
};