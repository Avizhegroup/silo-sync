namespace Silo.Application.Features;

public class GetAllWarehouseCorridorsHandler(WmsApiContext context)
    : IRequestHandler<GetAllWarehouseCorridorsQuery, List<GetAllWarehouseCorridorsVm>>
{
    public async Task<List<GetAllWarehouseCorridorsVm>> Handle(GetAllWarehouseCorridorsQuery request, CancellationToken cancellationToken)
        => await context.WarehouseCorridors
            .Select(c => new GetAllWarehouseCorridorsVm
            {
                Id = c.Id,
                ContextKey = c.ContextKey,
                X1 = c.X1,
                Z1 = c.Z1,
                X2 = c.X2,
                Z2 = c.Z2,
                Width = c.Width,
                Label = c.Label
            })
            .ToListAsync(cancellationToken);
}
