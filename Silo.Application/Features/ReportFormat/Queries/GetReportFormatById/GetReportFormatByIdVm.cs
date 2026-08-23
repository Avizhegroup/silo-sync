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
}

[JsonSerializable(typeof(ApiResponse<GetReportFormatByIdVm>))]
public partial class GetReportFormatByIdVmContext : JsonSerializerContext
{

}
