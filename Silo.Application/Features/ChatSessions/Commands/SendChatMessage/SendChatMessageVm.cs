using Silo.Shared.Tools;

namespace Silo.Application.Features;

public class SendChatMessageVm
{
    public string ResponseText { get; set; }
    public int SessionId { get; set; }
    public int? StatusCode { get; set; }
    public List<List<object>> SqlCommandsResults { get; set; } = new();
    public int? QueryId { get; set; }
    public AiReportCommandType? ReportType { get; set; }
}
