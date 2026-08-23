using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetTagsStatsOnShiftVm
{
    public string Shift { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagsStatsOnShiftVm>>))]
public partial class GetOnShiftContext : JsonSerializerContext
{
}