namespace Silo.Application.Features;
public class ZoneExcelDto
{
    public string Code { get; set; }
    public string Title { get; set; }
    public decimal Capacity { get; set; }
    public decimal MaxCapacity { get; set; }
    public string Dimention { get; set; }
    public string ParentCode { get; set; }
    public int ParentLayer { get; set; }
    public string WarehouseCode { get; set; }
    public int CountPixle { get; set; }
    public decimal MinCapacity { get; set; }
    public int OccupiedCapacity { get; set; }
    public int RowIndex { get; set; }
}
