using Silo.Domains.Entities;

namespace Silo.Sync.Core.RunLogging;

public interface ISyncRunLogger
{
    Task<SyncRunLog> StartRunAsync(string sourceKey, CancellationToken cancellationToken = default);
    Task CompleteRunAsync(SyncRunLog runLog, int rowsFetched, int rowsSucceeded, int rowsFailed, string status, string? errorSummary = null, CancellationToken cancellationToken = default);
}
