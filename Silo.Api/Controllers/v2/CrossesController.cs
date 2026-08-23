using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;
public class CrossesController(ILogger<CrossesController> logger, IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPlaceByTruckCrossId(GetPlaceByTruckCrossIdQuery query)
      => Ok(new ApiResponse()
      {
          Successful = true,
          Value = await mediator.Send<GetPlaceByTruckCrossIdVm>(query)
      });

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPlaceProductDetailsByCrossId(GetPlaceProductDetailsByCrossIdQuery query)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetPlaceProductDetailsByCrossIdVm>(query)
    });

    [HttpGet("[action]")]
    public async Task<IActionResult> GetLoadedCargoProductByCrossId(GetLoadedCargoProductsQuery query)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetLoadedCargoProductsVm>(query)
    });

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTruckCrossDynamicSearch(GetTruckCrossDynamicSearchQuery query)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<GetTruckCrossDynamicSearchVm>(query)
    });
}
