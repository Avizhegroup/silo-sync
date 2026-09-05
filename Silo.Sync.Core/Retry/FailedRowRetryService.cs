using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;
using Silo.Sync.Core.Configuration;
using Silo.Sync.Core.Failures;
using Silo.Sync.Core.Models;
using Silo.Sync.Core.Upsert;

namespace Silo.Sync.Core.Retry;

/// <summary>
/// Retries a previously failed sync row immediately, bypassing backoff schedules.
/// </summary>
public sealed class FailedRowRetryService(
    WmsApiContext context,
    ISyncSourceConfigProvider configProvider,
    IProductUpsertService upsertService,
    ISyncFailureStore failureStore) : IFailedRowRetryService
{
    /// <summary>
    /// Retries a single failed row immediately and returns the upsert result.
    /// </summary>
    public async Task<UpsertResult> RetryAsync(string sourceKey, string rowKey, CancellationToken cancellationToken = default)
    {
        var failure = await context.SyncRowFailures
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey && x.RowKey == rowKey && x.Status != "Resolved", cancellationToken);

        if (failure is null)
        {
            return new UpsertResult
            {
                Success = false,
                ErrorCategory = ErrorCategorizer.Other,
                ErrorMessage = "No pending failure found for the specified source and row key."
            };
        }

        var config = await configProvider.GetBySourceKeyAsync(sourceKey, cancellationToken);
        if (config is null || string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            return new UpsertResult
            {
                Success = false,
                ErrorCategory = ErrorCategorizer.Other,
                ErrorMessage = "Sync source configuration or connection string is missing."
            };
        }

        SaveProductCommand command;
        try
        {
            command = JsonSerializer.Deserialize<SaveProductCommand>(failure.RawPayload ?? "{}")
                      ?? throw new InvalidOperationException("Raw payload is empty or invalid.");
        }
        catch (Exception ex)
        {
            return new UpsertResult
            {
                Success = false,
                ErrorCategory = ErrorCategorizer.ConversionError,
                ErrorMessage = $"Failed to deserialize stored row payload: {ex.Message}"
            };
        }

        var result = await upsertService.UpsertOneAsync(command, config.ConnectionString, cancellationToken);

        if (result.Success)
        {
            await failureStore.ResolveFailureAsync(sourceKey, rowKey, cancellationToken);
        }
        else
        {
            await failureStore.RecordFailureAsync(sourceKey, rowKey, failure.RawPayload, result, failure.SyncRunLogId, cancellationToken);
        }

        return result;
    }
}
