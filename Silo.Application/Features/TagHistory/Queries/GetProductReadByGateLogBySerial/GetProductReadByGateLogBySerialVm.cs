using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductReadByGateLogBySerialVm
{
    public string GateCode { get; set; }
    public int LogId { get; set; }
    public string LogDate { get; set; }
    public string LogTime { get; set; }
    public string LogStatus { get; set; }
    public string StationName { get; set; }
    public string Username { get; set; }
    public string OperationType { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetProductReadByGateLogBySerialVm>>))]
public partial class GetProductReadByGateLogBySerialVmContext : JsonSerializerContext
{

}
