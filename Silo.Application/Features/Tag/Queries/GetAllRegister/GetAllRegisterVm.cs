using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllRegisterVm
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }

    [JsonPropertyName("RegCode")]
    public string TechnicalCode { get; set; }

    [JsonPropertyName("ProductStatusTitle")]
    public string Qc { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string Size { get; set; }
    public int NotEnterCount { get; set; }
    public string ContractStatus { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
    public string RegisterDevice { get; set; }
    public string LineCode { get; set; }
    public string LineTitle { get; set; }
    public int NotInspectCount { get; set; }
    public int AcceptInspectCount { get; set; }
    public int FailedInspectCount { get; set; }
    public decimal ProductCountInPack { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllRegisterVm>>))]
public partial class GetAllRegisterVmContext : JsonSerializerContext
{
}
