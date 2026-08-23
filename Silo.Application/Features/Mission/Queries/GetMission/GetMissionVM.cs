using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetMissionVM
{
    [JsonPropertyName("fld_PPMProductCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("fld_PPMProductSerial")]
    public string ProductSerial { get; set; }

    [JsonPropertyName("ProductTitle")]
    public string ProductName { get; set; }

    [JsonPropertyName("ProductTechnicalCode")]
    public string TechnicalCode { get; set; }

    [JsonPropertyName("FromZoneTitle")]
    public string FromZone { get; set; }

    [JsonPropertyName("ToZoneTitle")]
    public string ToZone { get; set; }

    [JsonPropertyName("DriverUserName")]
    public string Driver { get; set; }

    [JsonPropertyName("fld_PPMId")]
    public int MissionCode { get; set; }

    [JsonPropertyName("fld_PPMWMCode")]
    public int Truck { get; set; }

    [JsonPropertyName("PPMStatus")]
    public string MissionStatus { get; set; }

    [JsonPropertyName("PPMType")]
    public string MissionType { get; set; }

    [JsonPropertyName("TagStatus")]
    public string ProductStatus { get; set; }

    [JsonPropertyName("fld_PPMDateTime")]
    public string MissionDate { get; set; }

    [JsonPropertyName("fld_PPMRegDateTime")]
    public string RegisterDate { get; set; }

    [JsonPropertyName("RegUsername")]
    public string RegisterUsername { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetMissionVM>>))]
public partial class GetMissionVMObjectContext : JsonSerializerContext
{

}