using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class TextResourceController(ILogger<TextResourceController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll()
        => Ok(new ApiResponse<List<GetAllTextResourcesVm>>()
        {
            Successful = true,
            Value = await mediator.Send<List<GetAllTextResourcesVm>>(new GetAllTextResourcesQuery())
        });

    [HttpPost("[action]")]
    public async Task<IActionResult> Save(SaveTextResourcesCommand command)
        => Ok(new ApiResponse<SaveTextResourcesVm>()
        {
            Successful = true,
            Value = await mediator.Send<SaveTextResourcesVm>(command)
        });
}
