using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GateProductPrintableVm
{
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductName { get; set; }
    public decimal SumCount { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GateProductPrintableVm>>))]
public partial class GateProductPrintableVmContext : JsonSerializerContext
{
}
