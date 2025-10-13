using Microsoft.Extensions.Diagnostics.HealthChecks;
using N2J.Infrastructure.Persistence;

namespace N2J.WebApi.Startup;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddCustomJsonOptions()
            .AddCustomModelValidation();

        services.AddApiVersioningConfiguration();

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<CustomHealthCheck>("custom-health")
            .AddDbContextCheck<AppDbContext>("database")
            .AddRedis("redis", name: "redis-cache")
            .AddNpgSql("database", name: "postgres-db");

        return services;
    }
}

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(HealthCheckResult.Healthy("All systems operational"));
}