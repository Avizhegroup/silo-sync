using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Silo.Domains.Services;
using Silo.Sync.Core.Encryption;
using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Configuration;

public sealed class SyncSourceConfigProvider(WmsApiContext context, ISyncConnectionStringProtector protector)
    : ISyncSourceConfigProvider
{
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
}
