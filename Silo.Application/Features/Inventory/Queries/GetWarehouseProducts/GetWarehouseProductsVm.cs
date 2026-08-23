using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetWarehouseProductsVm
{
    public string ProductCode { get; set;}
    public string ProductTitle { get; set;}
    public string RegCode { get; set;}
    public string Qc { get; set;}
    public string ProductSize { get; set;}
    public string ProductSerial { get; set;}
    public decimal ProductCount { get; set;}
    public decimal RealityCount { get; set;}
    public string Epc { get; set;}
    public string Zones { get; set; }
    public string ThisTagZone { get; set; }
    public string Date { get; set; }
    public int TagStatus { get; set; }
    public string ContractStatus { get; set; }
    public string DestinationTitle { get; set; }
    public string Place { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetWarehouseProductsVm>>))]
public partial class GetWarehouseProductsVmContext : JsonSerializerContext
{

}
