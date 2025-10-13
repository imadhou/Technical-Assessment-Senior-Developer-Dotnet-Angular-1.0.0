using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using N2J.Infrastructure.Persistence;
using N2J.WebApi.Extensions;
using N2J.WebApi.Middleware;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// --- Infrastructure setup ---
builder.AddRedisClient("cache");
builder.AddNpgsqlDbContext<AppDbContext>("database");

// --- Service registration ---
builder.Services
    .AddInfrastructureServices()
    .AddApplicationServices()
    .AddApiServices()
    .AddSwaggerDocumentation();

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("custom-health");

var app = builder.Build();
var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();


// --- Environment setup ---
if (app.Environment.IsDevelopment())
{
    await InitializeDatabaseAsync(app);
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// --- Middleware ---
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// --- Swagger + Authorization ---
app.UseSwaggerDocumentation(apiVersionProvider);
app.UseAuthorization();
app.MapControllers();

// Root redirect
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


app.Run();


// --- Database initialization ---
async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        logger.LogInformation("Starting database migration...");

        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public partial class Program;

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        => Task.FromResult(HealthCheckResult.Healthy("All systems operational"));
}
