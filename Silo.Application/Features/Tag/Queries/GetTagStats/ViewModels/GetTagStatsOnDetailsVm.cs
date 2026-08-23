using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagStatsOnDetailsVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductSerial { get; set; }
    public string LineTitle { get; set; }
    public string Date { get; set; }
    public string StatusTitle { get; set; }
    public string RegCode { get; set; }
    public decimal Value { get; set; }
    public string Shift { get; set; }
    public string Username { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetTagStatsOnDetailsVm>>))]
public partial class GetOnDetailContext : JsonSerializerContext
{
}
