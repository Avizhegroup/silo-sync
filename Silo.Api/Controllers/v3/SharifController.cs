using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;

#if DEBUG
[AllowAnonymous]
#endif
public class SharifController(ILogger<SharifController> logger
    , IConfiguration configuration
    , IMediator mediator) : SiloBaseControllerVersion3(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> GetOcrDataForGalleryMedia(GetOcrDataForGalleryMediaQuery request)
    {
        var response = await mediator.Send<GetOcrDataForGalleryMediaVm>(request);
        return Ok(new ApiResponse<GetOcrDataForGalleryMediaVm>()
        {
            Successful = true,
            Value = response
        });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SendTag(CreateSharifTagCommand command)
    {
        var response = await mediator.Send<CreateSharifTagVm>(command);
        return Ok(new ApiResponse<CreateSharifTagVm>
        {
            Successful = true,
            Value = response
        });
    }
}
