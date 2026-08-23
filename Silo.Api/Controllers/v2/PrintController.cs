using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class PrintController(ILogger<PrintController> logger, IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(TransferPrintVm))]
    public async Task<IActionResult> SavePrint([FromBody] TransferPrintCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = (await mediator.Send<TransferPrintVm>(command)).Result
    });

    [HttpPut("[action]")]
    [ProducesDefaultResponseType(typeof(EditPrintVm))]
    public async Task<IActionResult> EditPrint([FromBody] EditPrintCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<EditPrintVm>(command)
    });

    [HttpDelete("[action]")]
    [ProducesDefaultResponseType(typeof(DeletePrintVm))]
    public async Task<IActionResult> DeletePrint([FromBody] DeletePrintCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<DeletePrintVm>(command)
    });
}
