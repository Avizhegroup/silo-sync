using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllGateProductVm
{
    public string Error { get; set; }
    public string TagEpc { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductUnit { get; set; }
    public string Regcode { get; set; }
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
    public string MaxDate { get; set; }
    public bool IsChoosed { get; set; } = false;
    public string Status { get; set; }
    public string SourceWarehouseCode { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllGateProductVm>>))]
public partial class GetAllGateProductVmContext : JsonSerializerContext
{
}
