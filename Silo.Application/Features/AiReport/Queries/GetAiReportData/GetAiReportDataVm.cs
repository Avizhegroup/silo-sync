namespace Silo.Application.Features;

public class GetAiReportDataVm
{
    public string Name { get; set; }
    public List<List<object>> Data { get; set; } = new();
    public int QueryId { get; set; }
}
