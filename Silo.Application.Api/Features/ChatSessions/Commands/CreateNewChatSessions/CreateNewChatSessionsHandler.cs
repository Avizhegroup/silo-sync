using System;
namespace Silo.Application.Api.Features;

public class CreateNewChatSessionsHandler(WmsApiContext context, IMapper mapper)
    : IRequestHandler<CreateNewChatSessionsCommand, CreateNewChatSessionsVm>
{
    public async Task<CreateNewChatSessionsVm> Handle(CreateNewChatSessionsCommand request, CancellationToken cancellationToken)
    {
        var chatsession = mapper.Map<Silo.Domains.Entities.ChatSessions>(request);

        chatsession.UserId = request.UserId;
        chatsession.SessionData = request.SessionData;
        chatsession.CreatedDate = DateTime.Now;
        chatsession.LastUpdated = DateTime.Now;

        context.ChatSessions.Add(chatsession);

        await context.SaveChangesAsync(cancellationToken);

        return new CreateNewChatSessionsVm
        {
            Result = chatsession.SessionId
        };
    }
}
