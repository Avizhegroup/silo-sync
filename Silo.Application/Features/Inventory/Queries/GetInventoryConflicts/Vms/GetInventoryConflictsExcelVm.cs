using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetInventoryConflictsExcelVm
{
    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("sumCount")]
    public decimal SumCount { get; set; }

    [JsonPropertyName("inventoryHeaderId")]
    public int InventoryHeaderId { get; set; }

    [JsonPropertyName("realityCount")]
    public decimal RealityCount { get; set; }
}
