using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class AndroidController(ILogger<AndroidController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> CheckVersion(GetNewApkQuery command)
    {
        var file = await mediator.Send<GetNewApkVm>(command);

        if (file.Result is null)
        {
            return NotFound();
        }

        return File(file.Result, "application/octet-stream", $"{file}.apk");
    }
}

