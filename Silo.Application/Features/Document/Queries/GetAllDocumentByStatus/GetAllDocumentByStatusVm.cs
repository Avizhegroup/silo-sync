using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllDocumentByStatusVm
{
    public int Id { get; set; }
    public string DocumentKey { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime LastDocumentEventDateTime { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerTitle { get; set; }
    public int ItemCount { get; set; }
    public decimal ItemSum { get; set; }
    public decimal Weight { get; set; } = 0;
    public decimal Volume { get; set; } = 0;
    public string DocumentType { get; set; }
    public int Status { get; set; }
    public string HeaderData { get; set; }
    public bool IsChoosed { get; set; } = false;
}
[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentByStatusVm>>))]
public partial class GetAllDocumentByStatusVmContext : JsonSerializerContext
{
}
