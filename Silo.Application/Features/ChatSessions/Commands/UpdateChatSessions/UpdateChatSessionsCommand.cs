namespace Silo.Application.Features;

public class UpdateChatSessionsCommand : IRequest<UpdateChatSessionsVm>
{
    public int SessionId { get; set; }
    public string UserId { get; set; }
    public string? SessionData { get; set; }
    public ChatTokenUsageDto? TokenUsage { get; set; }

}
