using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllExitActionDetailsByExitCodeVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public decimal ProductCount { get; set; }
    public string StoreCode { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string ProductType { get; set; }
    public string GateCode { get; set; }
    public string TechnicalInfoData { get; set; }
    public string StationName { get; set; }
    public string GuaranteeStatus { get; set; }
    public string GuaranteeStartDate { get; set; }
    public string GuaranteeEndDate { get; set; }
    public string ProductProperties { get; set; }
    public string Line { get; set; }
    public string Shift { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllExitActionDetailsByExitCodeVm>>))]
public partial class GetAllExitActionDetailsByExitCodeVmContext : JsonSerializerContext
{

}
