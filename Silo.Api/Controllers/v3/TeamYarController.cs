using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;
public class TeamYarController (ILogger<LibraryController> logger
    , IConfiguration configuration, IMediator mediator) : SiloBaseControllerVersion3(logger)
{
    /// <summary>
    /// ثبت اطلاعات سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> SaveTag(SaveTagForTeamYarCommand request)
    {
        //var response = await mediator.Send<SaveTagForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }  
    
    /// <summary>
    /// تایید ثبت نهایی جابه جایی
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> VerifyLocate(VerifyLocateForTeamYarCommand request)
    {
        //var response = await mediator.Send<VerifyLocateForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }

    /// <summary>
    /// ثبت مصرف سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> SerialConsumption(SaveSerialConsumptionCommand request)
    {
        //var response = await mediator.Send<SaveSerialConsumptionVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }
    
    /// <summary>
    /// ثبت حواله فروش
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> DocumentForSales(SaveDocumentForSaleCommand request)
    {
        //var response = await mediator.Send<SaveDocumentForSaleVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }

    /// <summary>
    /// ابطال سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> RevokeTags(RevokeTagForTeamYarCommand request)
    {
        //var response = await mediator.Send<RevokeTagForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = true
        });
    }

    /// <summary>
    /// استعلام وضعیت سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult>GetTagsBySerials(GetTagsBySerialsQuery request)
    {
        //var response = await mediator.Send<GetTagsBySerialsVm>(request);
        return Ok(new ApiResponse<GetTagsBySerialsVm>()
        {
            Successful = true,
            Value = new()
            {
                Result = 
                {
                    new()
                    {
                        Epc = "E20047027B606426C8D3010B",
                        LocationCode = "L-1",
                        WarehouseCode = "W-1",
                        ProductSerial = "3852489025529",
                        Status = GetTagsBySerialsStatusEnum.Active
                    } 
                }
            }
        });
    }
}
