namespace Silo.Application.Features;

public class GetAllZonesVm
{
    public int Id { get; set; }
    public string ZoneCode { get; set; }
    public string Title { get; set; }
    public decimal Capacity { get; set; }
    public string MinCapacity { get; set; }
    public string MaxCapacity { get; set; }
    public string Dimention { get; set; }
    public string RowIndex { get; set; }
    public string ParentCode { get; set; }
    public string ParentLayer { get; set; }
    public string StoreCode { get; set; }
    public string StoreType { get; set; }
    public string StoreTitle { get; set; }
    public string ProductCode { get; set; }
    public int CountPixle { get; set; }
    public int OccupiedCapacity { get; set; }
    public decimal FreeCapacity { get; set; }
    public string Coordinates { get; set; }
}
