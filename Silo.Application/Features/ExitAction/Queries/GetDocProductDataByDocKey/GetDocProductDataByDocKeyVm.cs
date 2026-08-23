using System.Text.Json.Serialization;

namespace Silo.Application.Features;

public class GetDocProductDataByDocKeyVm
{
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal? SumValue { get; set; }
    public string? Status { get; set; }
    public string? DocumentHeaderData { get; set; }
    public decimal? DocumentUsedCount { get; set; }
    public DocumentCheckType? DocumentCheckType { get; set; }

    public decimal? DocumentUnusedCount 
    {
        get => SumValue - DocumentUsedCount < 0 ? 0 : SumValue - DocumentUsedCount;
    }
}

[JsonSerializable(typeof(ApiResponse<List<GetDocProductDataByDocKeyVm>>))]
public partial class GetDocProductDataByDocKeyVmContext : JsonSerializerContext
{
}
