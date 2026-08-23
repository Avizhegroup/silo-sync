using Silo.Domains.Entities;

namespace Silo.Application.Features;

public class SaveWarehouseCorridorHandler(WmsApiContext context)
    : IRequestHandler<SaveWarehouseCorridorCommand, SaveWarehouseCorridorVm>
{
    public async Task<SaveWarehouseCorridorVm> Handle(SaveWarehouseCorridorCommand request, CancellationToken cancellationToken)
    {
        var entity = new WarehouseCorridor
        {
            ContextKey = request.ContextKey ?? string.Empty,
            X1 = request.X1,
            Z1 = request.Z1,
            X2 = request.X2,
            Z2 = request.Z2,
            Width = request.Width,
            Label = request.Label
        };

        context.WarehouseCorridors.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return new SaveWarehouseCorridorVm { Result = true, Id = entity.Id };
    }
}
