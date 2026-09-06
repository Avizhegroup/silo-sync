using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Application.Features;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class SyncAdminController(ILogger<SyncAdminController> logger, IMediator mediator)
    : SiloBaseControllerVersion2(logger)
{
    [HttpGet("sources")]
    public async Task<IActionResult> GetSources()
        => Ok(new ApiResponse<List<GetSyncSourcesVm>>
        {
            Successful = true,
            Value = await mediator.Send(new GetSyncSourcesQuery())
        });

    [HttpPost("sources")]
    public async Task<IActionResult> Create([FromBody] CreateSyncSourceCommand command)
        => Ok(new ApiResponse<CreateSyncSourceVm>
        {
            Successful = true,
            Value = await mediator.Send(command)
        });

    [HttpPut("sources/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSyncSourceCommand command)
    {
        command.Id = id;
        return Ok(new ApiResponse<UpdateSyncSourceVm>
        {
            Successful = true,
            Value = await mediator.Send(command)
        });
    }

    [HttpDelete("sources/{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => Ok(new ApiResponse<DeleteSyncSourceVm>
        {
            Successful = true,
            Value = await mediator.Send(new DeleteSyncSourceCommand { Id = id })
        });

    [HttpPost("sources/{id:int}/enable")]
    public async Task<IActionResult> Enable(int id)
        => Ok(new ApiResponse<EnableDisableSyncSourceVm>
        {
            Successful = true,
            Value = await mediator.Send(new EnableDisableSyncSourceCommand { Id = id, IsEnabled = true })
        });

    [HttpPost("sources/{id:int}/disable")]
    public async Task<IActionResult> Disable(int id)
        => Ok(new ApiResponse<EnableDisableSyncSourceVm>
        {
            Successful = true,
            Value = await mediator.Send(new EnableDisableSyncSourceCommand { Id = id, IsEnabled = false })
        });

    [HttpPost("sources/{id:int}/test-query")]
    public async Task<IActionResult> TestQuery(int id, [FromBody] TestSyncSourceQueryCommand? command)
    {
        var actual = command ?? new TestSyncSourceQueryCommand();
        actual.Id = id;
        return Ok(new ApiResponse<TestSyncSourceQueryVm>
        {
            Successful = true,
            Value = await mediator.Send(actual)
        });
    }

    [HttpGet("runs")]
    public async Task<IActionResult> GetRunHistory([FromQuery] GetSyncRunHistoryQuery query)
        => Ok(new ApiResponse<List<GetSyncRunHistoryVm>>
        {
            Successful = true,
            Value = await mediator.Send(query)
        });

    [HttpGet("failures")]
    public async Task<IActionResult> GetFailures([FromQuery] GetOpenSyncFailuresQuery query)
        => Ok(new ApiResponse<List<GetOpenSyncFailuresVm>>
        {
            Successful = true,
            Value = await mediator.Send(query)
        });

    [HttpPost("failures/{id:int}/retry")]
    public async Task<IActionResult> RetryFailure(int id)
        => Ok(new ApiResponse<RetrySyncRowFailureVm>
        {
            Successful = true,
            Value = await mediator.Send(new RetrySyncRowFailureCommand { Id = id })
        });
}
