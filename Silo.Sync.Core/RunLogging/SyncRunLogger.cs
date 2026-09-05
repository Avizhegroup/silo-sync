using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Sync.Core.RunLogging;

public sealed class SyncRunLogger(WmsApiContext context) : ISyncRunLogger
{
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
