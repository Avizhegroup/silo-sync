using Microsoft.EntityFrameworkCore;
using Silo.Application.Shared.Features;

namespace Silo.Application.Api.Features;
public class DeleteDestinationTypeHandler(WmsApiContext context) : IRequestHandler<DeleteDestinationTypeCommand, DeleteDestinationTypeVm>
{
    public async Task<DeleteDestinationTypeVm> Handle(DeleteDestinationTypeCommand request, CancellationToken cancellationToken)
    {
        var inUse = await context.Warehouses.AnyAsync(w => w.OperationalType == request.Code, cancellationToken);

        if (inUse)
        {
            return new DeleteDestinationTypeVm { Result = false };
        }
         
        var deleted = (await context.WarehouseTypes
                              .Where(p => p.Id == request.Id)
                              .ExecuteDeleteAsync(cancellationToken)) > 0;

        return new DeleteDestinationTypeVm
        {
            Result = deleted
        };
    }
}

