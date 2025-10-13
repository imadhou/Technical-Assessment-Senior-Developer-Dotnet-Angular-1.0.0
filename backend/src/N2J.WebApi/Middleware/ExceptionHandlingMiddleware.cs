using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace N2J.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
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
            _logger.LogError(ex, "An unhandled exception has occurred for request {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = CreateProblemDetails(context, exception);
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = "An error occurred while processing your request",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Instance = context.Request.Path
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Detail = exception.ToString();
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        // Add correlation ID if available
        if (context.Response.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId.ToString();
        }

        return problemDetails;
    }
}