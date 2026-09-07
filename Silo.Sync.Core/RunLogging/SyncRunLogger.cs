using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Sync.Core.RunLogging;

/// <summary>
/// Represents the SyncRunLogger class.
/// </summary>
public sealed class SyncRunLogger(WmsApiContext context) : ISyncRunLogger
{
    /// <summary>
    /// StartRunAsync operation.
    /// </summary>
    public async Task<SyncRunLog> StartRunAsync(string sourceKey, CancellationToken cancellationToken = default)
    {
        var runLog = new SyncRunLog
        {
            SourceKey = sourceKey,
            StartedAt = DateTime.UtcNow,
            Status = "Running"
        };

        context.SyncRunLogs.Add(runLog);
        await context.SaveChangesAsync(cancellationToken);

        return runLog;
    }

    /// <summary>
    /// CompleteRunAsync operation.
    /// </summary>
    public async Task CompleteRunAsync(SyncRunLog runLog, int rowsFetched, int rowsSucceeded, int rowsFailed, string status, string? errorSummary = null, CancellationToken cancellationToken = default)
    {
        runLog.RowsFetched = rowsFetched;
        runLog.RowsSucceeded = rowsSucceeded;
        runLog.RowsFailed = rowsFailed;
        runLog.Status = status;
        runLog.ErrorSummary = errorSummary;
        runLog.FinishedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}
