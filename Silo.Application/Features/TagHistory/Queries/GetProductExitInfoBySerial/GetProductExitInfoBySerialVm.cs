using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductExitInfoBySerialVm
{
    public int GateOpCode { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public int MovementActionId { get; set; }
    public string Username { get; set; }
    public string MovementActionData { get; set; }
    public string MovementActionDocumentId { get; set; }
    public string MovementActionDesc { get; set; }
    public string OperationDesc { get; set; }
    public string OperationDestination { get; set; }
    public string AgeAnalsys { get; set; }
    public int MovementActionTruckCrossId { get; set; }
    public string DriverName { get; set; }
    public string NationalCode { get; set; }
    public string LicenseCode { get; set; }
    public string DriverPhone { get; set; }
    public string Plaque { get; set; }
    public string TruckCrossTypeTitle { get; set; }
    public decimal EnterWeightTonage { get; set; }
    public decimal ExitWeightTonage { get; set; }
    public decimal ExitPureWeightCargo { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetProductExitInfoBySerialVm>>))]
public partial class GetProductExitInfoBySerialVmContext : JsonSerializerContext
{

}
