using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllStationsVm
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public StationTypeEnum? Type { get; set; }
    public string? StationActionType { get; set; }
    public StationStatusEnum? StationStatus { get; set; }
    public string? Readers { get; set; }
    public string? Desc { get; set; }
    public string? Settings { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllStationsVm>>))]
public partial class GetAllStationsVmContext : JsonSerializerContext
{

}
