using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagStatsOnLineVm
{
    public string LineTitle { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagStatsOnLineVm>>))]
public partial class GetOnProductLineReportContext : JsonSerializerContext
{
}