using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class ActionTypeController(ILogger<ActionTypeControlsController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateNewActionTypeCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<CreateNewActionTypeVm>(command)
    });

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(DeleteActionTypeByIdCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<DeleteActionTypeByIdVm>(command)
    });

    [HttpPut("[action]")]
    public async Task<IActionResult> Update(UpdateActionTypeByIdCommand command)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send<UpdateActionTypeByIdVm>(command)
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll()
        => Ok(new ApiResponse<GetAllActionTypesVm>()
        {
            Successful = true,
            Value = await mediator.Send<GetAllActionTypesVm>(new GetAllActionTypesQuery())
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> CheckCode(GetActionTypeByCodeQuery command)
         => Ok(new ApiResponse()
         {
             Successful = true,
             Value = await mediator.Send<GetActionTypeByCodeVm>(command)
         });
}

