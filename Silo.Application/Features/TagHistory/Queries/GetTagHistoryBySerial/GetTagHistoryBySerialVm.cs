using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetTagHistoryBySerialVm
{
    public List<GetProductInfosBySerialVm> ProductInfo { get; set; } = new();
    public List<GetProductStoreTransactionsBySerialVm> ProductStoreTransactions { get; set; } = new();
    public List<GetProductExitInfoBySerialVm> ProductExitInfo { get; set; } = new();
    public List<GetProductPlacementInfoBySerialVm> ProductPlacementInfo { get; set; } = new();
    public List<GetProductInventoryInfoBySerialVm> ProductInventoryInfo { get; set; } = new();
    public List<GetProductReadByGateLogBySerialVm> ProductReadByGateLog { get; set; } = new();
    public List<GetInspectResultsBySerialVm> InspectResults { get; set; } = new();
    public List<GetFreezeHeadersBySerialVm> FreezeHeaders { get; set; } = new();
}
[JsonSerializable(typeof(ApiResponse<GetTagHistoryBySerialVm>))]
public partial class GetTagHistoryBySerialVmContext : JsonSerializerContext
{
}
