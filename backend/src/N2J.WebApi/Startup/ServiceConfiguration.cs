using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using N2J.Infrastructure.Persistence;
using System.Text.Json;

namespace N2J.WebApi.Startup;


/// <summary>
/// Configures services for the API, including controllers, versioning, health checks, and JSON options.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Configures and adds API-related services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddCustomJsonOptions()
            .AddCustomModelValidation();

        services.AddApiVersioningConfiguration()
            .AddCustomHealthChecks();

        return services;
    }

    /// <summary>
    /// Configures API versioning and exploration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    /// <summary>
    /// Configures custom model validation error responses.
    /// Responsible for returning structured validation error details in API responses for invalid model states.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static IMvcBuilder AddCustomModelValidation(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors,
                    StatusCode = 400,
                    TraceId = context.HttpContext.TraceIdentifier
                });

                return result;
            };
        });

        return builder;
    }

    /// <summary>
    /// Configures custom JSON serialization options for the API.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });
    }


    /// <summary>
    /// Adds and configures custom health checks for the application, including database and Redis checks.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
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