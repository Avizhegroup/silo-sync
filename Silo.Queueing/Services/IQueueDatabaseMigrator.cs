namespace Silo.Queueing;

/// <summary>Explicitly applies pending queue database migrations.</summary>
public interface IQueueDatabaseMigrator
{
    /// <summary>Applies pending migrations without dropping or recreating existing tables.</summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
