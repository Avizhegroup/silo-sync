using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class PreparedReportController(ILogger<PreparedReportController> logger, IMediator mediator) 
    : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    [ProducesDefaultResponseType(typeof(CreatePreparedReportVm))]
    public async Task<IActionResult> Create(CreatePreparedReportCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<CreatePreparedReportVm>(command)
    });
}
