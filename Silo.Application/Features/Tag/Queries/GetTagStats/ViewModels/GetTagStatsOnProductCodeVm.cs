using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagStatsOnProductCodeVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public int CountSerial { get; set; }
    public decimal SumValue { get; set; }
    public decimal Percent { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagStatsOnProductCodeVm>>))]
public partial class GetOnProductReportContext : JsonSerializerContext
{
}