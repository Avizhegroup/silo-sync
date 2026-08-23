using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class GpsLogController(ILogger<GpsLogController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> GetGpsLog(GetGpsLogQuery command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetGpsLogVm>(command)
    });
}
