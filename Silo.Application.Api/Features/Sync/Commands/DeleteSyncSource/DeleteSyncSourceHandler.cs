using Silo.Application.Features;
using Silo.Domains.Services;

namespace Silo.Application.Api.Features.Sync;

public class DeleteSyncSourceHandler(WmsApiContext context)
    : IRequestHandler<DeleteSyncSourceCommand, DeleteSyncSourceVm>
{
    public async Task<DeleteSyncSourceVm> Handle(DeleteSyncSourceCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SyncSourceConfigs.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity is null)
        {
            return new DeleteSyncSourceVm { Success = false };
        }

        context.SyncSourceConfigs.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteSyncSourceVm { Success = true };
    }
}
