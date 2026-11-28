using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silo.Api.Attributes;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v3;

#if DEBUG
[AllowAnonymous]
#endif
[DuplicateActionRequested]
public class TeamYarController (ILogger<LibraryController> logger
    , IConfiguration configuration, IMediator mediator) : SiloBaseControllerVersion3(logger)
{
    /// <summary>
    /// ثبت/ ویرایش اطلاعات سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> SaveTag(SaveTagForTeamYarCommand request)
    {
        var response = await mediator.Send<SaveTagForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    ///  ویرایش مقادیر سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> UpdateTagCount(UpdateTagCountCommand request)
    {
        var response = await mediator.Send<UpdateTagCountVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    /// تایید ثبت نهایی جابه جایی
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> VerifyLocate(VerifyLocateForTeamYarCommand request)
    {
        var response = await mediator.Send<VerifyLocateForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    /// ثبت مصرف سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> SerialConsumption(SaveSerialConsumptionCommand request)
    {
        var response = await mediator.Send<SaveSerialConsumptionVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }
    
    /// <summary>
    /// ثبت حواله فروش
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> DocumentForSales(SaveDocumentForSaleCommand request)
    {
        var response = await mediator.Send<SaveDocumentForSaleVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    /// ابطال سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> RevokeTags(RevokeTagForTeamYarCommand request)
    {
        var response = await mediator.Send<RevokeTagForTeamYarVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    /// استعلام وضعیت سری ساخت
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult>GetTagsBySerials(GetTagsBySerialsQuery request)
    {
        var response = await mediator.Send<GetTagsBySerialsVm>(request);
        return Ok(new ApiResponse<GetTagsBySerialsVm>()
        {
            Successful = true,
            Value = response
        });
    }

    /// <summary>
    /// ثبت اسناد عمومی
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> DocumentForGeneral(SaveDocumentForGeneralCommand request)
    {
        var response = await mediator.Send<SaveDocumentForGeneralVm>(request);
        return Ok(new ApiResponse<bool>()
        {
            Successful = true,
            Value = response.Result
        });
    }

    /// <summary>
    /// استعلام پاسخ آخرین درخواست با شناسه یکتا
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> GetRequestResponse(GetRequestResponseQuery request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<ApiResponseWithStatusCode<JsonElement>?>()
            {
                Successful = false,
                Messages = ModelState.Values.SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage).ToArray()
            });
        }

        var response = await mediator.Send<GetRequestResponseVm?>(request, HttpContext.RequestAborted);
        if (response is null)
        {
            return NotFound(new ApiResponse<ApiResponseWithStatusCode<JsonElement>?>()
            {
                Successful = false,
                Messages = new[] { TextResources.APP_StringKeys_Validation_NotFound }
            });
        }

        if (response.Result is null)
        {
            return Accepted(new ApiResponse<ApiResponseWithStatusCode<JsonElement>?>()
            {
                Successful = false,
                Messages = new[] { TextResources.APP_StringKeys_Alert_TryAgain }
            });
        }

        return Ok(new ApiResponse<ApiResponseWithStatusCode<JsonElement>?>()
        {
            Successful = true,
            Value = response.Result
        });
    }
}
