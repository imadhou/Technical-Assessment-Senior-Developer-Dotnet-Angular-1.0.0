using Asp.Versioning.ApiExplorer;
using N2J.Application.Models.Configs;
using N2J.Infrastructure.DataSeeding;
using N2J.Infrastructure.Persistence;
using N2J.WebApi.Extensions;
using N2J.WebApi.Middleware;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace N2J.WebApi.Startup;

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    private WebApplication? _app;

    public Startup(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public async Task RunAsync()
    {
        ConfigureTelemetry();
        ConfigureInfrastructure();
        ConfigureServices();

        _app = _builder.Build();

        ConfigureMiddleware();
        ConfigureEndpoints();

        await InitializeDevelopmentEnvironmentAsync();
        await _app.RunAsync();
    }

    /// <summary>
    /// Configures OpenTelemetry for metrics, tracing, and logging.
    /// Allows to monitor and trace the application's performance and behavior.
    /// Exports telemetry data to an OTLP collector. (Aspire UI).
    /// </summary>
    private void ConfigureTelemetry()
    {
        // OpenTelemetry configuration for Aspire
        _builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation()
                       .AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddOtlpExporter();
            });

        // OpenTelemetry logging
        _builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.AddOtlpExporter();
        });
    }

    /// <summary>
    /// Add redis cache and database context to the service collection.
    /// </summary>
    private void ConfigureInfrastructure()
    {
        _builder.AddRedisClient("cache");
        _builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = _builder.Configuration.GetConnectionString("cache");
        });

        _builder.AddNpgsqlDbContext<AppDbContext>("database");
        _builder.AddAzureBlobServiceClient("blobs");

        _builder.Services.Configure<FileStorageOptions>(
        _builder.Configuration.GetSection("FileStorage"));
    }

    private void ConfigureServices()
    {
        _builder.Services
            .AddInfrastructureServices()
            .AddApplicationServices()
            .AddApiServices(_builder.Configuration)
            .AddSwaggerDocumentation();
    }

    private void ConfigureMiddleware()
    {
        if (_app == null) return;

        if (_app.Environment.IsDevelopment())
        {
            _app.UseDeveloperExceptionPage();
        }
        else
        {
            _app.UseExceptionHandler("/error");
            _app.UseHsts();
        }

        _app.UseHttpsRedirection();
        _app.UseRouting();

        // Custom middleware pipeline
        _app.UseMiddleware<ExceptionHandlingMiddleware>();
        _app.UseMiddleware<RequestLoggingMiddleware>();

        var apiVersionProvider = _app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        _app.UseSwaggerDocumentation(apiVersionProvider);
        _app.UseAuthorization();
    }

    private void ConfigureEndpoints()
    {
        if (_app == null) return;

        _app.MapControllers();

        // Health check endpoint
        _app.MapHealthChecks("/health").ExcludeFromDescription();

        // Test endpoints to generate traffic for metrics
        _app.MapGet("/api/test", () => new { message = "Test API", timestamp = DateTime.UtcNow }).ExcludeFromDescription();
        _app.MapGet("/api/health", () => new { status = "Healthy", service = "WebApi" }).ExcludeFromDescription();

        _app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    }


    private async Task InitializeDevelopmentEnvironmentAsync()
    {
        if (_app == null || !_app.Environment.IsDevelopment())
            return;

        using var scope = _app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            logger.LogInformation("Starting database initialization...");
            await DatabaseInitializer.InitializeAsync(context, logger, serviceProvider, true, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }
}