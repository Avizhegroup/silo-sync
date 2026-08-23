using System.Text.Json.Serialization;

namespace Silo.Application.Features;


public class GetStatsOnQcVm
{
    public string StatusTitle { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetStatsOnQcVm>>))]
public partial class GetOnQcReportContext : JsonSerializerContext
{
}