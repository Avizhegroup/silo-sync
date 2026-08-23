using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllInspectElementVm
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("inspectElementType")]
    public InspectElementType InspectElementType { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("minValue")]
    public int MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public int MaxValue { get; set; }

    [JsonPropertyName("prevent")]
    public bool Prevent { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("productTypes")]
    public List<string> ProductTypes { get; set; } = new();

    [JsonPropertyName("options")]
    public List<string> Options { get; set; } = new();

    [JsonPropertyName("row")]
    public int Row { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllInspectElementVm>>))]
public partial class GetAllInspectElementVmContext : JsonSerializerContext
{
}
