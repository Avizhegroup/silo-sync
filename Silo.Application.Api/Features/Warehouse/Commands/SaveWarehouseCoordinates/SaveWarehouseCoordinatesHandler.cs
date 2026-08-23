namespace Silo.Application.Features;

public class SaveWarehouseCoordinatesHandler (WmsApiContext context)
    : IRequestHandler<SaveWarehouseCoordinatesCommand, SaveWarehouseCoordinatesVm>
{
    public async Task<SaveWarehouseCoordinatesVm> Handle(SaveWarehouseCoordinatesCommand request, CancellationToken cancellationToken)
        => new()
        {
            Result = context.Warehouses.Where(p => p.Code == request.Code)
                          .ExecuteUpdate(p => p.SetProperty(x => x.Coordinates, request.Coordinates)) > 0
        };
}
