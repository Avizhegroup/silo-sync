using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetPlaceProductBySerialWithAggResultVm
{
    public string Error { get; set; }
    public string TagEpc { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductUnit { get; set; }
    public string RegCode { get; set; }
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
    public string MaxDate { get; set; }
    public string SourceWarehouseCode { get; set; }
    public string Status { get; set; }
    public bool IsChoosed { get; set; }
    public string StationName { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetPlaceProductBySerialWithAggResultVm>>))]
public partial class GetPlaceProductBySerialWithAggResultVmContext : JsonSerializerContext
{
}
