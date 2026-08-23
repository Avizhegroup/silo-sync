using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductModelsVm
{
    public string? TechnicalCode { get; set; }
    public string? ProductGroup { get; set; }
    public string? ProductSubGroup { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetProductModelsVm>>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
public partial class GetProductModelsVmContext : JsonSerializerContext
{
}
