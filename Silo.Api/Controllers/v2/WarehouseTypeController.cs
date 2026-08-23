using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class WarehouseTypeController(ILogger<WarehouseTypeController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetAllWarehouseTypesVm>(new GetAllWarehouseTypesQuery())
    });

}
