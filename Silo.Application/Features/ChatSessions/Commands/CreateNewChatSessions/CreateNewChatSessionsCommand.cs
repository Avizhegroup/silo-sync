
namespace Silo.Application.Features;

public class CreateNewChatSessionsCommand : IRequest<CreateNewChatSessionsVm>
{
    public string UserId { get; set; }
    public string? SessionData { get; set; }
    public RagDocType Mode { get; set; }
}
