using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetInventoryResultTagVm
{
    public string InventoryDate { get; set; }
    public int InventoryHeaderId { get; set; }
    public string InventoryZone { get; set; }
    public string ProductSerial { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string Epc { get; set; }
    public decimal ProductCount { get; set; }
    public int TagStatus { get; set; }
    public string RegCode { get; set; }
    public string RegisterDate { get; set; }
    public string ContractStatus { get; set; }
    public string DestinationTitle { get; set; }
    public string Place { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetInventoryResultTagVm>>))]
public partial class GetInventoryResultTagVmContext : JsonSerializerContext
{

}
