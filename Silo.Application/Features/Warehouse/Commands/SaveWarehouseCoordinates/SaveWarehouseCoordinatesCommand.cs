namespace Silo.Application.Features;

public class SaveWarehouseCoordinatesCommand : IRequest<SaveWarehouseCoordinatesVm>
{
    public string Code { get; set; }
    public string Coordinates { get; set; }
}
