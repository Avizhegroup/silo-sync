using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class WarehouseCorridorController(ILogger<WarehouseCorridorController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> GetAll()
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send(new GetAllWarehouseCorridorsQuery())
        });

    [HttpPost("[action]")]
    public async Task<IActionResult> Save(SaveWarehouseCorridorCommand command)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send<SaveWarehouseCorridorVm>(command)
        });

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Delete(int id)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send(new DeleteWarehouseCorridorCommand { Id = id })
        });
}
