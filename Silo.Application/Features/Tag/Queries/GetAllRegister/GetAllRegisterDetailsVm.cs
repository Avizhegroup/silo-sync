using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllRegisterDetailsVm
{
    public string ProductSerial { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string Size { get; set; }

    [JsonPropertyName("ProductRegCode")]
    public string TechnicalCode { get; set; }

    [JsonPropertyName("ProductStatusTitle")]
    public string ProductStatusTitle { get; set; }

    public decimal Count { get; set; }
    public string DateTime { get; set; }
    public string Username { get; set; }
    public string Line { get; set; }
    public string Shift { get; set; }
    public string DateTimeEnter { get; set; }
    public string Status { get; set; }
    public string InspectStatus { get; set; }
    public string InspectDate { get; set; }
    public string ProductProperties { get; set; }
    public string ProductSize { get; set; }
    public string ProductTypeCode { get; set; }
    public string ProductUnit { get; set; }
    public decimal ProductCountInPack { get; set; }
    public decimal ProductPackValue { get; set; }
    public decimal ProductPackWeight { get; set; }
    public string ProductStatusCode { get; set; }
    public string DocumentId { get; set; }
    public string DestinationCode { get; set; }
    public string ProductProductionLine { get; set; }
    public string ProductProductionShift { get; set; }
    public bool RegisterFlag { get; set; }
    public bool IsChoosed { get; set; } = true;

    public int ActionId { get; set; }

    [JsonIgnore]
    public bool IsEditing { get; set; } = false;

    [JsonIgnore]
    public string NewSerial { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllRegisterDetailsVm>>))]
public partial class GetAllRegisterDetailsVmContext : JsonSerializerContext
{
}
