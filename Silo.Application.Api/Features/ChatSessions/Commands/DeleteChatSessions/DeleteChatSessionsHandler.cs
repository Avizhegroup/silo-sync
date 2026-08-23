namespace Silo.Application.Api.Features;

public class DeleteChatSessionsHandler(WmsApiContext context) : IRequestHandler<DeleteChatSessionsCommand, DeleteChatSessionsVm>
{
    public async Task<DeleteChatSessionsVm> Handle(DeleteChatSessionsCommand request, CancellationToken cancellationToken)
    => new DeleteChatSessionsVm
    {
        Result = (await context.ChatSessions
                              .Where(p => p.SessionId == request.SessionId &&
                                     p.UserId == request.UserId)
                              .ExecuteDeleteAsync(cancellationToken))
    };
}
