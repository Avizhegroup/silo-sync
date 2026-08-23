using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetGateAlertsBySerialVm
{
    public string GateCode { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; }
    public string Type { get; set; }
    public string StationName { get; set; }
    public string Username { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetGateAlertsBySerialVm>>))]
public partial class GetGateAlertsBySerialVmContext : JsonSerializerContext
{

}
