using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetRemainDividableDocumentItemsVm
{
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public string ProductUnit { get; set; }
    public decimal Count { get; set; }
    public decimal DivisionCount { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetRemainDividableDocumentItemsVm>>))]
public partial class GetRemainDividableDocumentItemsVmContext : JsonSerializerContext
{
}
