using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetAllInventoryBySerialVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductName { get; set; }
    public string DateTime { get; set; }
    public string ProductDateTime { get; set; }
    public string OldSerial { get; set; }
    public string ContractStatus { get; set; }
    public string Desc { get; set; }
    public decimal ProductCount { get; set; }
    public string TypeTitle { get; set; }
    public string GroupTitle { get; set; }
    public string SizeTitle { get; set; }
    public string StatusTitle { get; set; }
    public string ProductProperties { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllInventoryBySerialVm>>))]
public partial class GetAllInventoryBySerialVmContext : JsonSerializerContext
{

}
