using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetPreparedReportByIdVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<KeyValuePair<string, object>> Variables { get; set; } 
    public List<KeyValuePair<string, object>> DataSources { get; set; }
    public List<KeyValuePair<string, string>> Images { get; set; } 
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string ReportFileName { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetPreparedReportByIdVm>))]
public partial class GetPreparedReportByIdVmContext : JsonSerializerContext
{
}
