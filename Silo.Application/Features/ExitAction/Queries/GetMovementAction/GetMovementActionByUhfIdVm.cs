using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetExitActionByUhfIdVm
{
    public int MovementActionId { get; set; }
    public int GateOperationCode { get; set; }
    public string Gate { get; set; }
    public string DocumentId { get; set; }
    public string MovementActionDesc { get; set; }
    public string MovementActionData { get; set; }
    public string DestinationWarehouseCode { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetExitActionByUhfIdVm>>))]
public partial class GetExitActionByUhfIdVmContext : JsonSerializerContext
{
}
