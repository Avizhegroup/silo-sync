using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllExitActionVm
{
    public string OpCode { get; set; }
    public string DateTime { get; set; }
    public string? User { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string GateOp { get; set; }
    public string GateCode { get; set; }
    public int? ActionType { get; set; }
    public string ActionTypeTitle { get; set; }
    public string StoreTitle { get; set; }
    public string DestinationTitle { get; set; }
    public string StoreCode { get; set; }
    public string MovementActionData { get; set; }
    public string ActionDocumentId { get; set; }
    public string MovementActionDesc { get; set; }
    public string Serials { get; set; }
    public string StationName { get; set; }
    public string GuaranteeStatus { get; set; }
    public string GuaranteeStartDate { get; set; }
    public string GuaranteeEndDate { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllExitActionVm>>))]
public partial class GetAllExitActionVmContext : JsonSerializerContext
{

}
