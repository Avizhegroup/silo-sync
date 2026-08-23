using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetCargoByTruckCrossIdVm
{
    public string DocumentCode { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public int SumCount { get; set; }
    public int TruckCrossId { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetCargoByTruckCrossIdVm>>))]
public partial class GetCargoByTruckCrossIdVmContext : JsonSerializerContext
{
}
