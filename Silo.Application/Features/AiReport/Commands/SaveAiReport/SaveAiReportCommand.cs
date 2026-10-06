namespace Silo.Application.Features;

public class SaveAiReportCommand
{
    public string ReportName { get; set; }
    public string Mode { get; set; }
    public int QueryId { get; set; }
}
