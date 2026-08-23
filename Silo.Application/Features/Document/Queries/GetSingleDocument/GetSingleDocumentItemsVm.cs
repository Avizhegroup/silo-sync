using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetSingleDocumentItemsVm
{
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public string? ProductUnit { get; set; }
    public string? ItemData { get; set; }
    public decimal Count { get; set; }
    public decimal EditCount { get; set; } = 0;
    public bool IsEditing { get; set; }
    public bool IsDeleting { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetSingleDocumentItemsVm>>))]
public partial class GetSingleDocumentItemsVmContext : JsonSerializerContext
{
}
