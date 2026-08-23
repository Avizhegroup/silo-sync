using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetDividableDocumentItemVm
{
    public int Id { get; set; }
    public string? Key { get; set; }
    public string DocumentType { get; set; }
    public string DocumentType1 { get; set; }
    public string DocumentType2 { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductTitle { get; set; }
    public decimal Count { get; set; }
    public string? ProductUnit { get; set; }
    public string? ItemData { get; set; }

    [NotMapped]
    public decimal DivisionCount { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetDividableDocumentItemVm>>))]
public partial class GetDividableDocumentItemVmContext : JsonSerializerContext
{
}
