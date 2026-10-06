using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Matchi.Infrastructure.Persistence;

public static class DatabaseMigrationRunner
{
    public static async Task ApplyAsync(
        MatchiDbContext dbContext,
        ILogger logger,
        bool migrateOnStartup,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);

        var connection = dbContext.Database.GetDbConnection();
        logger.LogInformation(
            "Database target: {DataSource} / {Database}",
            connection.DataSource,
            connection.Database);

        var applied = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
        var pending = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        logger.LogInformation(
            "Database migration status:{NewLine}Applied: {Applied}{NewLine}Pending: {Pending}",
            Environment.NewLine,
            applied.Count,
            Environment.NewLine,
            pending.Count);

        foreach (var name in pending)
            logger.LogInformation("Pending migration: {Migration}", name);

        if (!migrateOnStartup)
        {
            logger.LogWarning(
                "Database:MigrateOnStartup is disabled. Pending migrations were not applied ({PendingCount}). The database is not claimed to be synchronized.",
                pending.Count);
            return;
        }

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed. Startup will abort.");
            throw;
        }
    }
}
