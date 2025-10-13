namespace N2J.WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        _logger.LogInformation($"HTTP {context.Request.Method} {context.Request.Path} started");

        await _next(context);

        var elapsed = DateTime.UtcNow - startTime;

        _logger.LogInformation($"HTTP {context.Request.Method} {context.Request.Path} completed with {context.Response.StatusCode} in {elapsed.TotalMilliseconds}ms");
    }
}