
namespace Silo.Application.Features;

public class DeleteChatSessionsCommand : IRequest<DeleteChatSessionsVm>
{
    public int SessionId { get; set; }
    public string UserId { get; set; }
}
