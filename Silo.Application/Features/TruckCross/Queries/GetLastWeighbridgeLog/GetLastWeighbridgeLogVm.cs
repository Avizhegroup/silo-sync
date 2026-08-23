using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetLastWeighbridgeLogVm
{
    public int Id { get; set; }
    public string? WeighbridgeCode { get; set; }
    public decimal? Weight { get; set; }
    public DateTime? DateTime { get; set; }
    public string? ShamsiDate { get; set; }
    public string? Plaque { get; set; }
}

[JsonSerializable(typeof(ApiResponse<GetLastWeighbridgeLogVm>))]
public partial class GetLastWeighbridgeLogVmContext : JsonSerializerContext
{
}
