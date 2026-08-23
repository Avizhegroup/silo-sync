using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class PrintFormatController(ILogger<PrintFormatController> logger, IMediator mediator)
    : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(CreatePrintFormatVm))]
    public async Task<IActionResult> Create(CreatePrintFormatCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<CreatePrintFormatVm>(command)
    });

    [HttpDelete("[action]")]
    [ProducesDefaultResponseType(typeof(DeletePrintFormatVm))]
    public async Task<IActionResult> Delete(DeletePrintFormatCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<DeletePrintFormatVm>(command)
    });

    [HttpGet("[action]")]
    [ProducesDefaultResponseType(typeof(GetAllPrintFormatVm))]
    public async Task<IActionResult> GetAll()
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetAllPrintFormatVm>(new GetAllPrintFormatQuery())
    });

    [HttpGet("[action]")]
    [ProducesDefaultResponseType(typeof(GetPrintFormatsByPageTitleVm))]
    public async Task<IActionResult> GetPrintFormatsByPageTitle([FromQuery] GetPrintFormatsByPageTitleQuery query)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetPrintFormatsByPageTitleVm>(query)
    });
}
