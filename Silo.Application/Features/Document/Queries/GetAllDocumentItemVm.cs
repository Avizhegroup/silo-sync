using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllDocumentItemVm
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("productTitle")]
    public string ProductTitle { get; set; }

    [JsonPropertyName("count")]
    public decimal Count { get; set; }

    [JsonPropertyName("productUnit")]
    public string ProductUnit { get; set; }

    [JsonPropertyName("itemData")]
    public string ItemData { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentItemVm>>))]
public partial class GetAllDocumentItemVmContext : JsonSerializerContext
{
}