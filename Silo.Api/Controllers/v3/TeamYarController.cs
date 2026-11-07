using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;
public class TeamYarController (ILogger<LibraryController> logger
    , IConfiguration configuration, IMediator mediator) : SiloBaseControllerVersion3(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> GetOcrDataForGalleryMedia(GetOcrDataForGalleryMediaQuery request)
    {
        return Ok();
    }
}
