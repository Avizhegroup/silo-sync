namespace Silo.Application.Features;
public class SaveZoneCoordinatesHandler(WmsApiContext context)
    : IRequestHandler<SaveZoneCoordinatesCommand, SaveZoneCoordinatesVm>
{
    public async Task<SaveZoneCoordinatesVm> Handle(SaveZoneCoordinatesCommand request, CancellationToken cancellationToken)
        => new()
        {
            Result = context.Zones.Where(p => p.Code == request.Code)
                          .ExecuteUpdate(p => p.SetProperty(x => x.Coordinates, request.Coordinates)) > 0
        };
}
