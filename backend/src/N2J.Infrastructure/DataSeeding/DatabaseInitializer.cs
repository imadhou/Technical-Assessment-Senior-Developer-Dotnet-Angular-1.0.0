using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using N2J.Infrastructure.Persistence;

namespace N2J.Infrastructure.DataSeeding;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger, IServiceProvider serviceProvider, bool useMigrations = true, bool seedData = false)
    {
        try
        {
            logger.LogInformation("Starting database initialization...");
            logger.LogInformation("Configuration - UseMigrations: {UseMigrations}, SeedData: {SeedData}", useMigrations, seedData);

            if (useMigrations)
            {
                await InitializeWithMigrationsAsync(context, logger);
            }
            else
            {
                await InitializeWithEnsureCreatedAsync(context, logger);
            }

            // Seed data if requested
            if (seedData)
            {
                logger.LogInformation("Data seeding requested, starting seed process...");
                await SeedSampleDataAsync(context, logger, serviceProvider);
            }
            else
            {
                logger.LogInformation("Data seeding skipped (seedData = false)");
            }

            logger.LogInformation("Database initialization completed successfully");
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

    private static async Task SeedSampleDataAsync(AppDbContext context, ILogger logger, IServiceProvider serviceProvider)
    {
        try
        {
            logger.LogInformation("Starting sample data seeding...");

            // Check if data already exists
            var hasCompanies = await context.Companies.AnyAsync();
            var hasUsers = await context.Users.AnyAsync();

            logger.LogInformation("Current data status - Companies: {HasCompanies}, Users: {HasUsers}",
                hasCompanies, hasUsers);

            if (hasCompanies)
            {
                logger.LogInformation("Data already exists, skipping seeding");
                return;
            }

            logger.LogInformation("Resolving DataSeeder from service provider...");
            var seeder = serviceProvider.GetRequiredService<DataSeeder>();

            logger.LogInformation("Calling DataSeeder.SeedAsync()...");
            await seeder.SeedAsync();

            logger.LogInformation("Sample data seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed sample data");
            throw;
        }
    }

    private static bool IsMigrationsTableMissing(Exception ex)
    {
        return ex.Message.Contains("__EFMigrationsHistory") ||
               ex.Message.Contains("relation \"__EFMigrationsHistory\" does not exist");
    }
}