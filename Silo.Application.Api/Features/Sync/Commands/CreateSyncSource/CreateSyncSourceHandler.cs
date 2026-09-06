using Microsoft.EntityFrameworkCore;
using Silo.Application.Features;
using Silo.Domains.Entities;
using Silo.Domains.Services;
using Silo.Sync.Core.Encryption;

namespace Silo.Application.Api.Features.Sync;

public class CreateSyncSourceHandler(WmsApiContext context, ISyncConnectionStringProtector protector)
    : IRequestHandler<CreateSyncSourceCommand, CreateSyncSourceVm>
{
    public async Task<CreateSyncSourceVm> Handle(CreateSyncSourceCommand request, CancellationToken cancellationToken)
    {
        if (await context.SyncSourceConfigs.AnyAsync(x => x.SourceKey == request.SourceKey, cancellationToken))
        {
            return new CreateSyncSourceVm
            {
                Success = false,
                ErrorMessage = TextResources.APP_StringKeys_Validation_Code_Uniqueness
            };
        }

        var entity = new SyncSourceConfig
        {
            SourceKey = request.SourceKey,
            DisplayName = request.DisplayName,
            SourceType = request.SourceType,
            ConnectionStringEncrypted = string.IsNullOrWhiteSpace(request.ConnectionString)
                ? null
                : protector.Encrypt(request.ConnectionString),
            Command = request.Command,
            FieldKey = request.FieldKey,
            FieldCheck = request.FieldCheck,
            FieldOrder = request.FieldOrder,
            IntervalSeconds = request.IntervalSeconds,
            IsEnabled = request.IsEnabled,
            CreatedDate = DateTime.UtcNow
        };

        context.SyncSourceConfigs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateSyncSourceVm { Success = true, Id = entity.Id };
    }
}
