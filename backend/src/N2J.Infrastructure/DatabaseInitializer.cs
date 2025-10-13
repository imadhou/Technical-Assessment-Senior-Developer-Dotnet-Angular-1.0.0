using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using N2J.Infrastructure.Persistence;

namespace N2J.Infrastructure;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger, bool useMigrations = true)
    {
        try
        {
            if (useMigrations)
            {
                await InitializeWithMigrationsAsync(context, logger);
            }
            else
            {
                await InitializeWithEnsureCreatedAsync(context, logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }

    private static async Task InitializeWithMigrationsAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var migrationsList = pendingMigrations.ToList();

            if (migrationsList.Any())
            {
                logger.LogInformation("Found {Count} pending migrations: {Migrations}",
                    migrationsList.Count, string.Join(", ", migrationsList));

                await context.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully");
            }
            else
            {
                logger.LogInformation("No pending migrations found");
            }
        }
        catch (Exception ex) when (IsMigrationsTableMissing(ex))
        {
            logger.LogWarning("Migrations table not found. Falling back to EnsureCreated...");
            await InitializeWithEnsureCreatedAsync(context, logger);
        }
    }

    private static async Task InitializeWithEnsureCreatedAsync(AppDbContext context, ILogger logger)
    {
        var created = await context.Database.EnsureCreatedAsync();
        if (created)
        {
            logger.LogInformation("Database created successfully using EnsureCreated");
        }
        else
        {
            logger.LogInformation("Database already exists");
        }
    }

    private static bool IsMigrationsTableMissing(Exception ex)
    {
        return ex.Message.Contains("__EFMigrationsHistory") ||
               ex.Message.Contains("relation \"__EFMigrationsHistory\" does not exist");
    }
}