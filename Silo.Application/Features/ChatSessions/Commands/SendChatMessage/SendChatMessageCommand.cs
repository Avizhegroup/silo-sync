namespace Silo.Application.Features;

public class SendChatMessageCommand : IRequest<SendChatMessageVm>
{
    public string UserId { get; set; }
    public int SessionId { get; set; }
    public string Message { get; set; }
    public RagDocType Mode { get; set; }
}
