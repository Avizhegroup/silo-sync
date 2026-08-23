using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagStatsOnProductDateVm
{
    public string Date { get; set; }
    public int CountProductCode { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagStatsOnProductDateVm>>))]
public partial class GetOnProductDateContext : JsonSerializerContext
{
}