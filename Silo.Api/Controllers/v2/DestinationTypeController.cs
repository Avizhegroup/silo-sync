using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Shared.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class DestinationTypeController(ILogger<DestinationTypeController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateNewDestinationTypeCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<CreateNewDestinationTypeVm>(command)
    });

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(DeleteDestinationTypeCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<DeleteDestinationTypeVm>(command)
    });

    [HttpPut("[action]")]
    public async Task<IActionResult> Update(UpdateDestinationTypeCommand command)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send<UpdateDestinationTypeVm>(command)
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll()
        => Ok(new ApiResponse<GetAllDestinationTypeVm>()
        {
            Successful = true,
            Value = await mediator.Send<GetAllDestinationTypeVm>(new GetAllDestinationTypeQuery())
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> CheckCode(CheckDestinationTypeCodeQuery command)
         => Ok(new ApiResponse()
         {
             Successful = true,
             Value = await mediator.Send<CheckDestinationTypeCodeVm>(command)
         });

}
