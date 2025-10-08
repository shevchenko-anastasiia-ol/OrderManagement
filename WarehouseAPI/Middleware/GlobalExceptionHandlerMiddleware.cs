using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseAPI.Middleware;

public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;

            _logger.LogError(exception,
                "An error occurred. CorrelationId: {CorrelationId}, Path: {Path}",
                correlationId,
                context.Request.Path);

            var (statusCode, title, detail) = MapExceptionToResponse(exception);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = _environment.IsDevelopment() ? detail : "An error occurred processing your request.",
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            problemDetails.Extensions["traceId"] = correlationId;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private (int StatusCode, string Title, string Detail) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException ex => ((int)HttpStatusCode.BadRequest, "Invalid Argument", ex.Message),
                ArgumentException ex => ((int)HttpStatusCode.BadRequest, "Invalid Argument", ex.Message),
                KeyNotFoundException ex => ((int)HttpStatusCode.NotFound, "Resource Not Found", ex.Message),
                UnauthorizedAccessException ex => ((int)HttpStatusCode.Unauthorized, "Unauthorized Access", ex.Message),
                InvalidOperationException ex => ((int)HttpStatusCode.Conflict, "Invalid Operation", ex.Message),
                TimeoutException ex => ((int)HttpStatusCode.RequestTimeout, "Request Timeout", ex.Message),
                NotImplementedException ex => ((int)HttpStatusCode.NotImplemented, "Not Implemented", ex.Message),
                _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error", exception.Message)
            };
        }
    }