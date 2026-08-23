namespace Silo.Application.Features;

public class SaveZoneCoordinatesCommand : IRequest<SaveZoneCoordinatesVm>
{
    public string Code { get; set; }
    public string Coordinates { get; set; }
}
