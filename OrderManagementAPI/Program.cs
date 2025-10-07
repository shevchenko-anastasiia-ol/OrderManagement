using MarketplaceDAL;
using Microsoft.AspNetCore.Diagnostics;
using OrderManagementBLL;
using Serilog;
using System.Net;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


// --- DAL ---
builder.Services.AddDataAccess(builder.Configuration); 

// --- BLL ---
builder.Services.AddBusinessServices(builder.Configuration); 

builder.Services.AddAutoMapper(typeof(OrderManagementBLL.MappingProfiles.CustomerMappingProfile).Assembly);

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- ProblemDetails Middleware ---
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; 
});

var app = builder.Build();

// --- Middleware: ProblemDetails + Exception Handling ---
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        int statusCode = exception switch
        {
            OrderManagementBLL.Exceptions.NotFoundException => (int)HttpStatusCode.NotFound,
            OrderManagementBLL.Exceptions.ValidationException => (int)HttpStatusCode.BadRequest,
            OrderManagementBLL.Exceptions.BusinessConflictException => 409,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception?.Message,
            Detail = exception?.StackTrace
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

// --- Swagger UI ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderManagement API V1");
        c.RoutePrefix = string.Empty; // відкриває Swagger UI за http://localhost:5267/
    });
    
    var url = "http://localhost:5267";
    Task.Run(() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
        FileName = url,
        UseShellExecute = true
    }));
}

// --- HTTPS ---
app.UseHttpsRedirection();

// --- Map Controllers ---
app.MapControllers();



app.Run();


