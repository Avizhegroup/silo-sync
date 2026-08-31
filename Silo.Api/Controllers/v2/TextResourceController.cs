using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class TextResourceController(ILogger<TextResourceController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> ReadAll(GetAllTextResourcesQuery query)
        => Ok(new ApiResponse<List<GetAllTextResourcesVm>>()
        {
            Successful = true,
            Value = await mediator.Send<List<GetAllTextResourcesVm>>(query)
        });

    [HttpPost("[action]")]
    public async Task<IActionResult> Save(SaveTextResourcesCommand command)
        => Ok(new ApiResponse<SaveTextResourcesVm>()
        {
            Successful = true,
            Value = await mediator.Send<SaveTextResourcesVm>(command)
        });
}
