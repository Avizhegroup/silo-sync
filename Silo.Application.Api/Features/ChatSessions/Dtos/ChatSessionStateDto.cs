namespace Silo.Application.Api.Features;

/// <summary>
/// Local persistence model stored (as JSON) in <see cref="ChatSessions.SessionData"/>.
/// Keeps the Silo AI RAG conversation id needed to continue the remote conversation
/// together with the locally rendered transcript, since the RAG API does not expose
/// an endpoint to retrieve a conversation's past messages.
/// </summary>
public class ChatSessionStateDto
{
    public Guid? ConversationId { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}
