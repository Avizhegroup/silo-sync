using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;
public class NewChatSessionHandler(
    WmsApiContext context,
    IAiApiClient aiApiClient)
    : IRequestHandler<NewChatSessionCommand, NewChatSessionVm>
{
    public async Task<NewChatSessionVm> Handle(NewChatSessionCommand request, CancellationToken cancellationToken)
    {
        string serializedSession = await aiApiClient.NewSessionAsync(request.PromptKeys, cancellationToken);

        var chatSession = new Silo.Domains.Entities.ChatSessions
        {
            UserId = request.UserId,
            SessionData = serializedSession,
            TokenUsage = JsonSerializer.Serialize(new ChatTokenUsageDto()),
            Mode = (int)request.Mode,
            CreatedDate = DateTime.Now,
            LastUpdated = DateTime.Now
        };

        context.ChatSessions.Add(chatSession);

        await context.SaveChangesAsync(cancellationToken);

        return new NewChatSessionVm
        {
            SessionId = chatSession.SessionId
        };
    }
}
