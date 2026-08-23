namespace Silo.Application.Features;

public class DeleteWarehouseCorridorCommand : IRequest<DeleteWarehouseCorridorVm>
{
    public int Id { get; set; }
}
