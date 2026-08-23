using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllDocumentHeaderVm
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [JsonPropertyName("importType")]
    public DocumentImportType ImportType { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; }

    [JsonPropertyName("importDateTime")]
    public DateTime ImportDateTime { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("headerData")]
    public string HeaderData { get; set; }

    [JsonPropertyName("documentItems")]
    public List<GetAllDocumentItemVm> DocumentItems { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetAllDocumentHeaderVm>))]
public partial class GetAllDocumentHeaderVmContext : JsonSerializerContext
{
}
