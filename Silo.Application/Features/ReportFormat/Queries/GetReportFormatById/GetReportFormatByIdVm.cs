using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetReportFormatByIdVm
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }

    public List<ReportFormatDetail> DetailsList =>
    string.IsNullOrWhiteSpace(Details)
        ? new List<ReportFormatDetail>()
        : JsonSerializer.Deserialize<List<ReportFormatDetail>>(Details)!;

    public string AiQuery { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetReportFormatByIdVm>))]
[JsonSerializable(typeof(ApiResponse<GetAiReportDataVm>))]
public partial class GetReportFormatByIdVmContext : JsonSerializerContext
{

}

