using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductPlacementInfoBySerialVm
{
    public string WarehouseTitle { get; set; }
    public string Location { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public string Username { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetProductPlacementInfoBySerialVm>>))]
public partial class GetProductPlacementInfoBySerialVmContext : JsonSerializerContext
{

}
