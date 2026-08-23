using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetInspectAggByQcHeaderVm
{
    public string RegCode { get; set; }
    public string Size { get; set; }
    public string Line { get; set; }
    public decimal AcceptedSum { get; set; }
    public decimal AcceptedCount { get; set; } 
    public decimal RejectedCount { get; set; }
    public decimal RejectedSum { get; set; } 
    public List<GetInspectAggByQcItemVm> Items { get; set; } = new();
}

[JsonSerializable(typeof(ApiResponse<List<GetInspectAggByQcHeaderVm>>))]
public partial class GetInspectAggByQcHeaderVmContext : JsonSerializerContext
{

}
