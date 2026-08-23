using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllDocAggSuggestDetailVm
{
    [JsonPropertyName("documentKey")]
    public string DocumentKey { get; set; }
    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; }
    [JsonPropertyName("importDateTime")]
    public DateTime? ImportDateTime { get; set; }
    [JsonPropertyName("itemCount")]
    public int ItemCount { get; set; }
    [JsonPropertyName("itemSum")]
    public decimal ItemSum { get; set; }
    [JsonPropertyName("documentData")]
    public string DocumentData { get; set; }
    public List<DocumentItemDto> DocumentItems { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllDocAggSuggestDetailVm>>))]
public partial class GetAllDocAggSuggestDetailVmContext : JsonSerializerContext
{
}
