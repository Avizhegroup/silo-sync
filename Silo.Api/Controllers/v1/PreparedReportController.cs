using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v1;
public class PreparedReportController(ILogger<PreparedReportController> logger, IMediator mediator) 
    : SiloBaseController(logger)
{
   [HttpGet("[action]")]
    [ProducesDefaultResponseType(typeof(GetPreparedReportByIdVm))]
    public async Task<IActionResult> GetById(GetPreparedReportByIdQuery query)
      => Ok(new ApiResponse()
      {
          Successful = true,
          Value = await mediator.Send<GetPreparedReportByIdVm>(query)
      });
}
