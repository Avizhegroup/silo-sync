namespace Silo.Application.Features;

public class NewChatSessionCommand : IRequest<NewChatSessionVm>
{
    public string UserId { get; set; }
    public RagDocType Mode { get; set; }
    public List<string> PromptKeys { get; set; } = new();
}
