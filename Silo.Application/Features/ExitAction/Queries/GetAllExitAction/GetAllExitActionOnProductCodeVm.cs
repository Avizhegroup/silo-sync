using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllExitActionOnProductCodeVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public decimal ProductCountInPack { get; set; }
    public string StoreCode { get; set; }
    public int? ActionType { get; set; }
    public int? OpCode { get; set; }
    public string ActionTypeTitle { get; set; }
    public string StoreTitle { get; set; }
    public string DestinationTitle { get; set; }
    public string TechnicalInfoData { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllExitActionOnProductCodeVm>>))]
public partial class GetAllExitActionOnProductCodeVmContext : JsonSerializerContext
{

}
