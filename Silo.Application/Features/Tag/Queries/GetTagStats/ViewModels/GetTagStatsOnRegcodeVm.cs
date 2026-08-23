using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagStatsOnRegcodeVm
{
    public string RegCode { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagStatsOnRegcodeVm>>))]
public partial class GetOnRegcodeReportContext : JsonSerializerContext
{
}