using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Silo.Domains.Services;
using Silo.Sync.Core.Encryption;
using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Configuration;

/// <summary>
/// Represents the SyncSourceConfigProvider class.
/// </summary>
public sealed class SyncSourceConfigProvider(WmsApiContext context, ISyncConnectionStringProtector protector)
    : ISyncSourceConfigProvider
{
    /// <summary>
    /// GetBySourceKeyAsync operation.
    /// </summary>
    public async Task<SyncSourceConfigDto?> GetBySourceKeyAsync(string sourceKey, CancellationToken cancellationToken = default)
    {
        var entity = await context.SyncSourceConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SourceKey == sourceKey, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        string? connectionString = null;
        if (!string.IsNullOrWhiteSpace(entity.ConnectionStringEncrypted))
        {
            try
            {
                connectionString = protector.Decrypt(entity.ConnectionStringEncrypted);
            }
            catch (CryptographicException)
            {
                connectionString = null;
            }
        }

        return new SyncSourceConfigDto
        {
            SourceKey = entity.SourceKey,
            DisplayName = entity.DisplayName,
            SourceType = entity.SourceType,
            ConnectionString = connectionString,
            Command = entity.Command,
            FieldKey = entity.FieldKey,
            FieldCheck = entity.FieldCheck,
            FieldOrder = entity.FieldOrder,
            IntervalSeconds = entity.IntervalSeconds,
            IsEnabled = entity.IsEnabled
        };
    }

    public async Task<IReadOnlyList<SyncSourceConfigDto>> GetAllEnabledAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.SyncSourceConfigs
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .OrderBy(x => x.SourceKey)
            .ToListAsync(cancellationToken);

        var results = new List<SyncSourceConfigDto>(entities.Count);
        foreach (var entity in entities)
        {
            string? connectionString = null;
            if (!string.IsNullOrWhiteSpace(entity.ConnectionStringEncrypted))
            {
                try
                {
                    connectionString = protector.Decrypt(entity.ConnectionStringEncrypted);
                }
                catch (CryptographicException)
                {
                    connectionString = null;
                }
            }

            results.Add(new SyncSourceConfigDto
            {
                SourceKey = entity.SourceKey,
                DisplayName = entity.DisplayName,
                SourceType = entity.SourceType,
                ConnectionString = connectionString,
                Command = entity.Command,
                FieldKey = entity.FieldKey,
                FieldCheck = entity.FieldCheck,
                FieldOrder = entity.FieldOrder,
                IntervalSeconds = entity.IntervalSeconds,
                IsEnabled = entity.IsEnabled
            });
        }

        return results;
    }
}
