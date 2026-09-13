using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetReportFormatByIdVm
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public List<ReportFormatDetail> DetailsList
    {
        get => JsonSerializer.Deserialize<List<ReportFormatDetail>>(Details);

    }
    public string AiQuery { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetReportFormatByIdVm>))]
[JsonSerializable(typeof(GetReportFormatByIdVm))]
[JsonSerializable(typeof(ReportFormatDetail))]
[JsonSerializable(typeof(GetAiReportDataVm))]
[JsonSerializable(typeof(ApiResponse<GetAiReportDataVm>))]
[JsonSerializable(typeof(List<List<object>>))]
[JsonSerializable(typeof(List<object>))]
public partial class GetReportFormatByIdVmContext : JsonSerializerContext
{

}

public class GetAiReportDataVm
{
    public string Name { get; set; }
    //public string AiQuery { get; set; }
    public List<List<object>> Data { get; set; } = new();
    public int QueryReferenceId { get; set; }
}
