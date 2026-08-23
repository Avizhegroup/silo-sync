using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllTruckQuery
{
    [JsonPropertyName("fld_WMCode")]
    public string Code { get; set; }

    [JsonPropertyName("fld_WMTitle")]
    public string Title { get; set; }

    [JsonPropertyName("fld_WMRFID")]
    public string TagEpc { get; set; }

    [JsonPropertyName("fld_WMDriverName")]
    public string Driver { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllTruckQuery>>))]
public partial class GetAllTruckQueryContext : JsonSerializerContext
{
}
