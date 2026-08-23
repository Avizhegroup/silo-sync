using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class ZoneController(ILogger<ZoneController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPut("[action]")]
    public async Task<IActionResult> SaveCoordinates(SaveZoneCoordinatesCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<SaveZoneCoordinatesVm>(command)
    });
}
