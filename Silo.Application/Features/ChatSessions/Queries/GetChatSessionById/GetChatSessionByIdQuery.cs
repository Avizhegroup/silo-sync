namespace Silo.Application.Features;

public class GetChatSessionByIdQuery : IRequest<GetChatSessionByIdVm>
{
    public int SessionId { get; set; }
    public string UserId { get; set; }
}
