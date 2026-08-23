using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public  class GetTruckCrossItemsByTruckCrossIdVm
{
    public int Id { get; set; }

    public int? Type { get; set; }

    public string? Title { get; set; }

    public string? ProductUnit { get; set; }

    public decimal? ProductCount { get; set; }

    public string? ProductSerial { get; set; }

    public string? ProductCode { get; set; }

    public int? TruckCrossProductTypeId { get; set; }

    public string TruckCrossProductTypeTitle { get; set; }

    public long? TruckCrossId { get; set; }
}
[JsonSerializable(typeof(ApiResponse<List<GetTruckCrossItemsByTruckCrossIdVm>>))]
public partial class GetTruckCrossItemsByTruckCrossIdVmContext : JsonSerializerContext
{
}
