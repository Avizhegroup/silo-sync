using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Shared.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class DashboardController(
    ILogger<DashboardController> logger,
    IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetDashboardItems()
       => Ok(new ApiResponse<List<GetDashboardItemsVm>>
       {
           Successful = true,
           Value = await mediator.Send(
               new GetDashboardItemsQuery())
       });
}
