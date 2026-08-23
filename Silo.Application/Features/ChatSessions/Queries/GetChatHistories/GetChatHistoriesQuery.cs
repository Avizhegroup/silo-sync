namespace Silo.Application.Features;

public class GetChatHistoriesQuery : IRequest<GetChatHistoriesVm>
{
    public string UserId { get; set; }
    public RagDocType? Mode { get; set; }
}
