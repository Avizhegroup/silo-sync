using Microsoft.EntityFrameworkCore;
using Silo.Domains.Entities;
using Silo.Domains.Services;
using Silo.Sync.Core.Models;
using Silo.Sync.Core.Retry;

namespace Silo.Sync.Core.Failures;

/// <summary>
/// Represents the SyncFailureStore class.
/// </summary>
public sealed class SyncFailureStore(WmsApiContext context, IRetryScheduler retryScheduler) : ISyncFailureStore
{
    /// <summary>
    /// RecordFailureAsync operation.
    /// </summary>
    public async Task RecordFailureAsync(string sourceKey, string rowKey, string? rawPayload, UpsertResult result, int? runLogId, CancellationToken cancellationToken = default)
    {
        var failure = await context.SyncRowFailures
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey && x.RowKey == rowKey && x.Status != "Resolved", cancellationToken);

        var now = DateTime.UtcNow;

        if (failure is null)
        {
            failure = new SyncRowFailure
            {
                SourceKey = sourceKey,
                RowKey = rowKey,
                RawPayload = rawPayload,
                ErrorCategory = result.ErrorCategory,
                ErrorMessage = result.ErrorMessage,
                AttemptCount = 1,
                LastAttemptAt = now,
                NextAttemptAt = retryScheduler.GetNextAttemptTime(1, now),
                Status = "Pending",
                SyncRunLogId = runLogId
            };
            context.SyncRowFailures.Add(failure);
        }
        else
        {
            failure.AttemptCount++;
            failure.LastAttemptAt = now;
            failure.NextAttemptAt = retryScheduler.GetNextAttemptTime(failure.AttemptCount, now);
            failure.ErrorCategory = result.ErrorCategory;
            failure.ErrorMessage = result.ErrorMessage;
            failure.RawPayload = rawPayload ?? failure.RawPayload;
            failure.SyncRunLogId = runLogId;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// ResolveFailureAsync operation.
    /// </summary>
    public async Task ResolveFailureAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default)
    {
        var failure = await context.SyncRowFailures
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey && x.RowKey == rowKey && x.Status != "Resolved", cancellationToken);

        if (failure is null)
        {
            return;
        }

        failure.Status = "Resolved";
        failure.ResolvedDate = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// CanAutoRetryAsync operation.
    /// </summary>
    public async Task<bool> CanAutoRetryAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default)
    {
        var failure = await context.SyncRowFailures
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey && x.RowKey == rowKey && x.Status != "Resolved", cancellationToken);

        if (failure is null)
        {
            return true;
        }

        return failure.NextAttemptAt is null || failure.NextAttemptAt <= DateTime.UtcNow;
    }
}
