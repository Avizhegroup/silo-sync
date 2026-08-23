using System.Text.Json.Serialization;
namespace Silo.Application.Features;
public class GetAllDocumentLogVm
{
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public string ImportDateTime { get; set; }
    public string HeaderData { get; set; }
    public DocumentEventType DocumentEventType { get; set; }
    public DateTime DateTime { get; set; }
    public string ShamsiDate { get; set; }
    public string User { get; set; }
    public string Description { get; set; }
    public int DocumentStatus { get; set; }
    public int MinutesUntilNext { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentLogVm>>))]
public partial class GetAllDocumentLogVmContext : JsonSerializerContext
{
}
