using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetAllInventoryVm
{
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public string ProductName { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string Desc { get; set; }
    public string Place { get; set; }
    public string Code { get; set; }
    public string WarehouseCode { get; set; }
}

[JsonSerializable(typeof(ApiResponse<List<GetAllInventoryVm>>))]
public partial class GetInventoryResponseContext: JsonSerializerContext
{
   
}