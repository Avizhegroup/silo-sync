using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllExitActionDetailsVm
{
    public string OpCode { get; set; }
    public string DateTime { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public string TechnicalCode { get; set; }
    public string Qc { get; set; }
    public decimal Count { get; set; }
    public string Line { get; set; }
    public string Shift { get; set; }
    public string StoreCode { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string ProductType { get; set; }
    public string GateCode { get; set; }
    public string DeviceIp { get; set; }
    public string ContractStatus { get; set; }
    public int? ActionType { get; set; }
    public string ActionTypeTitle { get; set; }
    public string StoreTitle { get; set; }
    public string DestinationTitle { get; set; }
    public string MovementActionData { get; set; }
    public string TechnicalInfoData { get; set; }
    public string ActionDocumentId { get; set; }
    public string MovementActionDesc { get; set; }
    public string StationName { get; set; }
    public string ProductProperties { get; set; }
    public string MovementProperties { get; set; }
    public string Username { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllExitActionDetailsVm>>))]
public partial class GetAllExitActionDetailsVmContext : JsonSerializerContext
{

}
