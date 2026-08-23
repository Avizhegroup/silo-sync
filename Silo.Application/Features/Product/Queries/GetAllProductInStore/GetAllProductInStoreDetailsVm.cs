using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllProductInStoreDetailsVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductSize { get; set; }
    public string RegCode { get; set; }
    public string Qc { get; set; }
    public decimal ProductCount { get; set; }
    public string Date { get; set; }
    public string EnterDate { get; set; }
    public string Zone { get; set; }
    public string ProductProperties { get; set; }
    public string Line { get; set; }
    public string Shift { get; set; }
    public string TagStatus { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllProductInStoreDetailsVm>>))]
public partial class GetAllProductInStoreDetailsVmContext : JsonSerializerContext
{
}
