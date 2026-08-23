using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class DestinationController(ILogger<DestinationController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPut("[action]")]
    public async Task<IActionResult> SaveCoordinates(SaveWarehouseCoordinatesCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<SaveWarehouseCoordinatesVm>(command)
    });
}
