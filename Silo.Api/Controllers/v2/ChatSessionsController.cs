using MediatR;
using Microsoft.AspNetCore.Mvc;
using Silo.Base.Controllers.Base;

namespace Silo.Api.Controllers.v2;

public class ChatSessionsController(ILogger<ChatSessionsController> logger
    , IMediator mediator) : SiloBaseControllerVersion2(logger)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create( CreateNewChatSessionsCommand command)
        => Ok(new ApiResponse()
        {
            Successful = true,
            Value = await mediator.Send<CreateNewChatSessionsVm>(command)

        });
    
    [HttpPut("[action]")]
    public async Task<IActionResult> Update(UpdateChatSessionsCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<UpdateChatSessionsVm>(command)
    });

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(DeleteChatSessionsCommand command)
    => Ok(new ApiResponse()
    {
        Successful = true,
        Value = await mediator.Send<DeleteChatSessionsVm>(command)
    });

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadAll([FromQuery] string? userId)
        => Ok(new ApiResponse<GetAllChatSessionsVm>()
        {
            Successful = true,
            Value = await mediator.Send<GetAllChatSessionsVm>(new GetAllChatSessionsQuery { UserId = userId })
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> ReadById([FromQuery] int sessionId, [FromQuery] string userId)
        => Ok(new ApiResponse<GetChatSessionByIdVm>()
        {
            Successful = true,
            Value = await mediator.Send<GetChatSessionByIdVm>(new GetChatSessionByIdQuery { SessionId = sessionId, UserId = userId })
        });

    [HttpPost("[action]")]
    public async Task<IActionResult> NewSession([FromBody] NewChatSessionCommand command)
        => Ok(new ApiResponse<NewChatSessionVm>()
        {
            Successful = true,
            Value = await mediator.Send<NewChatSessionVm>(command)
        });

    [HttpPost("[action]")]
    public async Task<IActionResult> SendMessage([FromBody] SendChatMessageCommand command)
        => Ok(new ApiResponse<SendChatMessageVm>()
        {
            Successful = true,
            Value = await mediator.Send<SendChatMessageVm>(command)
        });

    [HttpGet("[action]")]
    public async Task<IActionResult> GetChatHistories([FromQuery] string userId, [FromQuery] RagDocType? mode)
        => Ok(new ApiResponse<GetChatHistoriesVm>()
        {
            Successful = true,
            Value = await mediator.Send<GetChatHistoriesVm>(new GetChatHistoriesQuery { UserId = userId, Mode = mode })
        });
}


