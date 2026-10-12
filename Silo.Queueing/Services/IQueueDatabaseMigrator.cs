namespace Silo.Queueing;

/// <summary>Explicitly applies pending queue database migrations.</summary>
public interface IQueueDatabaseMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
