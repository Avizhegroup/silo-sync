using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetPlaceProductBySerialVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string RegCode { get; set; }
    public decimal Count { get; set; }
    public decimal SumCountInFrom { get; set; }
    public decimal SumCountInDestination { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetPlaceProductBySerialVm>>))]
public partial class GetPlaceProductBySerialVmContext : JsonSerializerContext
{
}