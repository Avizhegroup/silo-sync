using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetEnterActionAggregateOnProductCodeVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public decimal ProductCountInPack { get; set; }
    public string Destination { get; set; }
    public int? ActionType { get; set; }
    public string ActionTypeTitle { get; set; }
    public string DestinationTitle { get; set; }
    public string StoreTitle { get; set; }
    public string TechnicalInfoData { get; set; }
    public string ProductProperties { get; set; }
    public int? OpCode { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetEnterActionAggregateOnProductCodeVm>>))]
public partial class GetEnterActionAggregateOnProductCodeVmContext : JsonSerializerContext
{

}
