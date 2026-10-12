namespace Silo.Queueing;

internal sealed class QueueDatabaseMigrator(IDbContextFactory<QueueDbContext> dbContextFactory)
    : IQueueDatabaseMigrator
{
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
