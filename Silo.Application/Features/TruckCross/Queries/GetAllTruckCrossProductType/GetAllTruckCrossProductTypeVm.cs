namespace Silo.Application.Features;

public class GetAllTruckCrossProductTypeVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int TruckCrossCauseId { get; set; }
    public int[] TruckCrossCauseIds { get; set; }
}
