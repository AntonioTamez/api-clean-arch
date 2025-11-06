using CleanArch.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extensiones para gestión de base de datos
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Asegura que la base de datos esté creada, migrada y con datos seed
    /// </summary>
    public static async Task<IHost> MigrateDatabaseAsync(this IHost host, bool seedData = true)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            logger.LogInformation("Starting database migration...");

            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Aplicar migraciones pendientes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} pending migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully");
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }

            // Seed de datos si se solicita
            if (seedData)
            {
                var seederLogger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
                var seeder = new DatabaseSeeder(context, seederLogger);
                await seeder.SeedAsync();
            }

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
            throw;
        }

        return host;
    }

    /// <summary>
    /// Solo ejecuta los seeders (útil para desarrollo)
    /// </summary>
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(context, logger);
        await seeder.SeedAsync();
    }

    /// <summary>
    /// Elimina y recrea la base de datos (solo para desarrollo/testing)
    /// </summary>
    public static async Task RecreateDatabase(this IHost host, bool seedData = true)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            logger.LogWarning("Deleting database...");
            await context.Database.EnsureDeletedAsync();

            logger.LogInformation("Creating database...");
            await context.Database.EnsureCreatedAsync();

            if (seedData)
            {
                var seederLogger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
                var seeder = new DatabaseSeeder(context, seederLogger);
                await seeder.SeedAsync();
            }

            logger.LogInformation("Database recreated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while recreating the database");
            throw;
        }
    }
}
