namespace N2J.WebApi.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (var scope = context.RequestServices.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CorrelationIdMiddleware>>();
            using (logger.BeginScope("{CorrelationId}", correlationId))
            {
                await _next(context);
            }
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) &&
            !string.IsNullOrEmpty(correlationId))
        {
            return correlationId!;
        }

        return Guid.NewGuid().ToString();
    }
}