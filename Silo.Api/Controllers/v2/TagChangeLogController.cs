using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class TablesChangeLogController(ILogger<TablesChangeLogController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll(GetAllTagChangeLogQuery request)
       => Ok(new ApiResponse<GetTablesChangeLogVm>()
       {
           Successful = true,
           Value = await mediator.Send<GetTablesChangeLogVm>(request)
       });
}
