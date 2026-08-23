using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class GetProductInventoryInfoBySerialVm
{
    public int OperationCode { get; set; }
    public string WarehouseTitle { get; set; }
    public string Date { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string UserName { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetProductInventoryInfoBySerialVm>>))]
public partial class GetProductInventoryInfoBySerialVmContext : JsonSerializerContext
{

}
