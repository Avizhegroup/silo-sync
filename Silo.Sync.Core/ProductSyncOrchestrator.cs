using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Silo.Sync.Core.Checkpoints;
using Silo.Sync.Core.Configuration;
using Silo.Sync.Core.Failures;
using Silo.Sync.Core.Fetching;
using Silo.Sync.Core.Models;
using Silo.Sync.Core.RunLogging;
using Silo.Sync.Core.Upsert;

namespace Silo.Sync.Core;

public sealed class ProductSyncOrchestrator(
    ILogger<ProductSyncOrchestrator> logger,
    IConfiguration configuration,
    ISyncSourceConfigProvider configProvider,
    ISyncCheckpointStore checkpointStore,
    ISyncRunLogger runLogger,
    ISyncFailureStore failureStore,
    ISourceProductFetcher fetcher,
    IProductUpsertService upsertService)
{
    public async Task<SyncRunResult> RunAsync(string sourceKey, bool ignoreBackoff = false, CancellationToken cancellationToken = default)
    {
        var runLog = await runLogger.StartRunAsync(sourceKey, cancellationToken);
        var mainConnectionString = configuration.GetConnectionString("SqlDefaultConnectionString");

        if (string.IsNullOrWhiteSpace(mainConnectionString))
        {
            await runLogger.CompleteRunAsync(runLog, 0, 0, 0, "Failed", "Main connection string not configured.", cancellationToken);
            return new SyncRunResult { Success = false, ErrorSummary = "Main connection string not configured." };
        }

        SyncSourceConfigDto? source;
        try
        {
            source = await configProvider.GetBySourceKeyAsync(sourceKey, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load source config for {SourceKey}", sourceKey);
            await runLogger.CompleteRunAsync(runLog, 0, 0, 0, "Failed", $"Failed to load source config: {ex.Message}", cancellationToken);
            return new SyncRunResult { Success = false, ErrorSummary = $"Failed to load source config: {ex.Message}" };
        }

        if (source is null)
        {
            await runLogger.CompleteRunAsync(runLog, 0, 0, 0, "Failed", $"Source config not found: {sourceKey}", cancellationToken);
            return new SyncRunResult { Success = false, ErrorSummary = $"Source config not found: {sourceKey}" };
        }

        if (string.IsNullOrWhiteSpace(source.ConnectionString))
        {
            await runLogger.CompleteRunAsync(runLog, 0, 0, 0, "Failed", $"Connection string could not be decrypted for source: {sourceKey}", cancellationToken);
            return new SyncRunResult { Success = false, ErrorSummary = $"Connection string could not be decrypted for source: {sourceKey}" };
        }

        var checkpoint = await checkpointStore.GetCheckpointAsync(sourceKey, cancellationToken);
        IReadOnlyList<ProductRow> rows;

        try
        {
            rows = await fetcher.FetchAsync(source, checkpoint, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch products from source {SourceKey}", sourceKey);
            await runLogger.CompleteRunAsync(runLog, 0, 0, 0, "Failed", $"Fetch failed: {ex.Message}", cancellationToken);
            return new SyncRunResult { Success = false, ErrorSummary = $"Fetch failed: {ex.Message}" };
        }

        var rowsFetched = rows.Count;
        var rowsSucceeded = 0;
        var rowsFailed = 0;
        var rowsSkipped = 0;
        var rowsDuplicate = 0;
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        DateTime? maxCheckpoint = null;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.RowKey))
            {
                rowsSkipped++;
                logger.LogWarning("Source {SourceKey}: skipped row with null/blank key.", sourceKey);
                continue;
            }

            if (!seenKeys.Add(row.RowKey))
            {
                rowsDuplicate++;
                logger.LogWarning("Source {SourceKey}: duplicate key {RowKey} discarded.", sourceKey, row.RowKey);
                continue;
            }

            if (!ignoreBackoff)
            {
                var canRetry = await failureStore.CanAutoRetryAsync(sourceKey, row.RowKey, cancellationToken);
                if (!canRetry)
                {
                    logger.LogInformation("Source {SourceKey}: row {RowKey} skipped due to backoff.", sourceKey, row.RowKey);
                    continue;
                }
            }

            var result = await upsertService.UpsertOneAsync(row.Command, mainConnectionString, cancellationToken);

            if (result.Success)
            {
                rowsSucceeded++;
                await failureStore.ResolveFailureAsync(sourceKey, row.RowKey, cancellationToken);

                if (maxCheckpoint is null || row.CheckValue > maxCheckpoint)
                {
                    maxCheckpoint = row.CheckValue;
                }
            }
            else
            {
                rowsFailed++;
                await failureStore.RecordFailureAsync(sourceKey, row.RowKey, row.RawPayload, result, runLog.Id, cancellationToken);
            }
        }

        if (maxCheckpoint.HasValue)
        {
            await checkpointStore.AdvanceCheckpointAsync(sourceKey, maxCheckpoint.Value, cancellationToken);
        }

        var status = rowsFailed == 0 ? "Succeeded" : (rowsSucceeded == 0 ? "Failed" : "Partial");
        var summary = rowsSkipped > 0 || rowsDuplicate > 0
            ? $"Skipped: {rowsSkipped}, Duplicate: {rowsDuplicate}, Succeeded: {rowsSucceeded}, Failed: {rowsFailed}"
            : null;

        await runLogger.CompleteRunAsync(runLog, rowsFetched, rowsSucceeded, rowsFailed, status, summary, cancellationToken);

        return new SyncRunResult
        {
            Success = rowsFailed == 0,
            RowsFetched = rowsFetched,
            RowsSucceeded = rowsSucceeded,
            RowsFailed = rowsFailed,
            ErrorSummary = summary
        };
    }

    public sealed record SyncRunResult
    {
        public required bool Success { get; init; }
        public int RowsFetched { get; init; }
        public int RowsSucceeded { get; init; }
        public int RowsFailed { get; init; }
        public string? ErrorSummary { get; init; }
    }
}
