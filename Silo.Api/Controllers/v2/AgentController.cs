using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class AgentController(ILogger<AgentController> logger, IMediator mediator) 
    : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
   public async Task<IActionResult> GetOcrDataForGalleryMedia(GetOcrDataForGalleryMediaQuery request)
   {
       var response = await mediator.Send<GetOcrDataForGalleryMediaVm>(request);
       return Ok(new ApiResponse<GetOcrDataForGalleryMediaVm>()
       {
           Successful = true,
           Value = response
       });
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSqlDataForBot(GetSqlDataForBotQuery request)
    {
        var response = await mediator.Send<GetSqlDataForBotVm>(request);
        return Ok(new ApiResponse<GetSqlDataForBotVm>()
        {
            Successful = true,
            Value = response
        });
    }
}
