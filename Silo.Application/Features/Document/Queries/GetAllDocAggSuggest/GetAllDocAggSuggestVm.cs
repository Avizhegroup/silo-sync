using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllDocAggSuggestVm
{
    public string DocAggCode { get; set; }
    public string DocumentType { get; set; }
    public int DocumentCount { get; set; }
    public decimal ItemSum { get; set; }
    public string GroupDataValue1 { get; set; }
    public string GroupDataValue2 { get; set; }
    public string GroupDataValue3 { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllDocAggSuggestVm>>))]
public partial class GetAllDocAggSuggestVmContext : JsonSerializerContext
{
}
