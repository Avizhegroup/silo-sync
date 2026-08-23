using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class ProductController(ILogger<DocumentController> logger, IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(TransferProductVm))]
    public async Task<IActionResult> SaveProduct([FromBody] TransferProductCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = (await mediator.Send<TransferProductVm>(command)).Result
    });
}
