using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllDocumentItemByStatusVm
{
    public int Id { get; set; }
    public string DocumentKey { get; set; }
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public decimal Count { get; set; }
    public string ItemData { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllDocumentItemByStatusVm>>))]
public partial class GetAllDocumentItemByStatusVmContext : JsonSerializerContext
{
}
