using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetDividableDocumentHeaderVm
{
    public int Id { get; set; }
    public string? Key { get; set; }
    public string? UserId { get; set; }
    public DocumentImportType ImportType { get; set; }
    public string? FileName { get; set; }
    public string DocumentType { get; set; }
    public DateTime? ImportDateTime { get; set; }
    public string? Description { get; set; }
    public int DocumentStatusId { get; set; }
    public string? HeaderData { get; set; }
    public List<GetDividableDocumentItemVm> DocumentItems { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetDividableDocumentHeaderVm>))]
public partial class GetDividableDocumentHeaderVmContext : JsonSerializerContext
{
}
