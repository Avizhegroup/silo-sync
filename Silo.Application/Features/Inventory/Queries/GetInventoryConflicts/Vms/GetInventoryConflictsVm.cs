using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetInventoryConflictsVm
{
    [JsonPropertyName("ProductCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("RegCode")]
    public string TechnicalCode { get; set; }

    [JsonPropertyName("ProductTitle")]
    public string ProductName { get; set; }

    [JsonPropertyName("ProductSize")]
    public string ProductSize { get; set; }

    [JsonPropertyName("ProductStatus")]
    public string Qc { get; set; }

    [JsonPropertyName("CountProduct")]
    public int Count { get; set; } = 0;

    [JsonPropertyName("SumValue")]
    public decimal SumCount { get; set; } = 0;

    public decimal SumCountAccounting { get; set; } = 0;

    public decimal SumCountReality { get; set; } = 0;

    [JsonPropertyName("inventoryCountProduct")]
    public int CountInventory { get; set; } = 0;

    [JsonPropertyName("InventorySumValue")]
    public decimal SumCountInventory { get; set; } = 0;

    public int ConflictCount { get; set; } = 0;

    [JsonPropertyName("ConflictValue")]
    public decimal ConflictSumCount { get; set; } = 0;

    public decimal ConflictSumCountAccounting { get; set; } = 0;

    public decimal ConflictSumCountReality { get; set; } = 0;

    [JsonIgnore]
    public bool IsExtraConflict { get; set; }

    [JsonPropertyName("Zones")]
    public string Locations { get; set; }

    [JsonIgnore]
    public List<string> LocationList { get; set; } = new();

    [JsonIgnore]
    public List<GetInventoryConflictDetailsVm> Details { get; set; } = new();
}


[JsonSerializable(typeof(ApiResponse<List<GetInventoryConflictsVm>>))]
public partial class GetInventoryConflictResponseContext : JsonSerializerContext
{

}
