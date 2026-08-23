namespace Silo.Application.Api.Features;
public class NewChatSessionHandler(
    WmsApiContext context,
    ISiloAiClient siloAiClient)
    : IRequestHandler<NewChatSessionCommand, NewChatSessionVm>
{
    public async Task<NewChatSessionVm> Handle(NewChatSessionCommand request, CancellationToken cancellationToken)
    {
        var conversationId = await siloAiClient.StartNewSessionAsync(cancellationToken);

        var state = new ChatSessionStateDto
        {
            ConversationId = conversationId
        };

        var chatSession = new Silo.Domains.Entities.ChatSessions
        {
            UserId = request.UserId,
            SessionData = JsonSerializer.Serialize(state),
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
