using Silo.Application.Features;
using Silo.Domains.Services;

namespace Silo.Application.Api.Features.Sync;

public class EnableDisableSyncSourceHandler(WmsApiContext context)
    : IRequestHandler<EnableDisableSyncSourceCommand, EnableDisableSyncSourceVm>
{
    public async Task<EnableDisableSyncSourceVm> Handle(EnableDisableSyncSourceCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SyncSourceConfigs.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity is null)
        {
            return new EnableDisableSyncSourceVm { Success = false };
        }

        entity.IsEnabled = request.IsEnabled;
        entity.ModifiedDate = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return new EnableDisableSyncSourceVm { Success = true };
    }
}
