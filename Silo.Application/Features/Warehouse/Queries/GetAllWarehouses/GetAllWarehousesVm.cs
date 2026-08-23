using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllWarehousesVm
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("DestinationCode")]
    public string DestinationCode { get; set; }

    [JsonPropertyName("DestinationTitle")]
    public string DestinationTitle { get; set; }

    /// <summary>
    /// Destination Type
    /// </summary>
    [JsonPropertyName("OperationalType")]
    public DestinationOperationalType OperationalType { get; set; } = DestinationOperationalType.NotSpecified;

    [JsonPropertyName("InventoryType")]
    public DestinationInventoryType InventoryType { get; set; } = DestinationInventoryType.NotSpecified;

    [JsonPropertyName("IsDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("IsActive")]
    public bool IsActive { get; set; }

    public string Coordinates { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllWarehousesVm>>))]
public partial class GetAllWarehousesVmContext : JsonSerializerContext
{
}

public enum DestinationOperationalType
{
    NotSpecified = -1,
    Quarentine = 0,
    Product = 1,
    Sales = 3,
    Material = 4,
    Waste = 5,
    Loading = 2
}

public enum DestinationInventoryType
{
    NotSpecified = -1,
    Virtual,
    Physical

}
