using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllPlaceHeadersVm
{
    public int PlaceId { get; set; }
    public string DateTime { get; set; }
    public string UserName { get; set; }
    public string DocumentId { get; set; }
    public string Destination { get; set; }
    public string DestinationWarehouseCode { get; set; }
    public string DestinationWarehouseTitle { get; set; }
    public decimal DestinationCapacity { get; set; }
    public decimal DestinationFreeCapacity { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetAllPlaceHeadersVm>>))]
public partial class GetAllPlaceOperationsVmContext : JsonSerializerContext
{
}
