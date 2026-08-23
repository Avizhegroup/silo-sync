namespace Silo.Application.Features;

public class GetAllChatSessionsQuery : IRequest<GetAllChatSessionsVm>
{
    public string? UserId { get; set; }
}
