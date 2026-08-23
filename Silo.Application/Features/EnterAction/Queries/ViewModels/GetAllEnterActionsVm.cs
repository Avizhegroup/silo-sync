using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllEnterActionsVm
{
    public string OpCode { get; set; }
    public string DateTime { get; set; }
    public string? User { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string GateOp { get; set; }
    public string Destination { get; set; }
    public string GateCode { get; set; }
    public int? ActionType { get; set; }
    public string ActionTypeTitle { get; set; }
    public string DestinationTitle { get; set; }
    public string StoreTitle { get; set; }
    public string MovementActionData { get; set; }
    public string ActionDocumentId { get; set; }
    public string MovementActionDesc { get; set; }
    public string StationName { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllEnterActionsVm>>))]
public partial class GetAllEnterActionsVmContext : JsonSerializerContext
{

}
