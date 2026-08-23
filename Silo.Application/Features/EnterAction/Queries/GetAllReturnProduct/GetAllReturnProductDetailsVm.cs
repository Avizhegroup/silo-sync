using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllReturnProductDetailsVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }

    [JsonPropertyName("ProductSize")]
    public string Size { get; set; }
    public string RegCode { get; set; }

    [JsonPropertyName("ProductStatusTitle")]
    public string Qc { get; set; }

    [JsonPropertyName("ProductCount")]
    public decimal Count { get; set; }

    [JsonPropertyName("fld_ProductPropertyATitle")]
    public string Line { get; set; }

    [JsonPropertyName("fld_ProductPropertyBTitle")]
    public string Shift { get; set; }

    [JsonPropertyName("TagRegisterShamsiUnixDate")]
    public string DateTimeEnter { get; set; }

    [JsonPropertyName("HTagsMovementDate")]
    public string ExitDateTime { get; set; }

    [JsonPropertyName("MovementData")]
    public string ExitDesc { get; set; }

    [JsonPropertyName("TagStatus")]
    public string Status { get; set; }

}
[JsonSerializable(typeof(ApiResponse<List<GetAllReturnProductDetailsVm>>))]
public partial class GetAllReturnProductDetailsVmContext : JsonSerializerContext
{

}
