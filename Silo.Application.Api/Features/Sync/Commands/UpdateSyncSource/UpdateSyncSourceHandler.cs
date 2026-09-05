using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Services;
using Silo.Sync.Core.Encryption;

namespace Silo.Application.Api.Features.Sync;

public class UpdateSyncSourceHandler(WmsApiContext context, ISyncConnectionStringProtector protector)
    : IRequestHandler<UpdateSyncSourceCommand, UpdateSyncSourceVm>
{
    public async Task<UpdateSyncSourceVm> Handle(UpdateSyncSourceCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SyncSourceConfigs.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity is null)
        {
            return new UpdateSyncSourceVm { Success = false };
        }

        if (await context.SyncSourceConfigs.AnyAsync(x => x.Id != request.Id && x.SourceKey == request.SourceKey, cancellationToken))
        {
            return new UpdateSyncSourceVm { Success = false };
        }

        entity.SourceKey = request.SourceKey;
        entity.DisplayName = request.DisplayName;
        entity.SourceType = request.SourceType;
        entity.Command = request.Command;
        entity.FieldKey = request.FieldKey;
        entity.FieldCheck = request.FieldCheck;
        entity.FieldOrder = request.FieldOrder;
        entity.IntervalSeconds = request.IntervalSeconds;
        entity.IsEnabled = request.IsEnabled;
        entity.ModifiedDate = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            entity.ConnectionStringEncrypted = protector.Encrypt(request.ConnectionString);
        }

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateSyncSourceVm { Success = true };
    }
}
