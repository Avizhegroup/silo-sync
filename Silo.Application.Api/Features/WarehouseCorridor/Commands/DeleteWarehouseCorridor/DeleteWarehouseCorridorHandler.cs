namespace Silo.Application.Features;

public class DeleteWarehouseCorridorHandler(WmsApiContext context)
    : IRequestHandler<DeleteWarehouseCorridorCommand, DeleteWarehouseCorridorVm>
{
    public async Task<DeleteWarehouseCorridorVm> Handle(DeleteWarehouseCorridorCommand request, CancellationToken cancellationToken)
        => new()
        {
            Result = await context.WarehouseCorridors
                .Where(c => c.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken) > 0
        };
}
