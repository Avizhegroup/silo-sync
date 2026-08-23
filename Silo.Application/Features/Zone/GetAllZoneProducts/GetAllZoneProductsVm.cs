namespace Silo.Application.Features;

public class GetAllZoneProductsVm
{
    public string ZoneCode { get; set; }
    public decimal TotalCapacity { get; set; }
    public decimal FreeCapacity { get; set; }
    public decimal FreePercent { get; set; }
    public decimal OccupiedCapacity { get; set; }
    public decimal OccupiedPercent { get; set; }
    public int CountProductCode { get; set; }
    public int CountSerials { get; set; }
}
