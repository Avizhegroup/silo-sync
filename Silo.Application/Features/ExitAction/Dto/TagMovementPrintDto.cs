using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class TagMovementPrintDto
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductName { get; set; }
    public decimal ProductCount { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<TagMovementPrintDto>>))]
public partial class TagMovementPrintDtoContext : JsonSerializerContext
{
}
