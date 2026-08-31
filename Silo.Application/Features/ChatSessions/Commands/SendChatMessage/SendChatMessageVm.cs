namespace Silo.Application.Features;

public class SendChatMessageVm
{
    public string ResponseText { get; set; }
    public int SessionId { get; set; }
    public int? StatusCode { get; set; }
    public List<List<object>> SqlCommandsResults { get; set; } = new();
}
