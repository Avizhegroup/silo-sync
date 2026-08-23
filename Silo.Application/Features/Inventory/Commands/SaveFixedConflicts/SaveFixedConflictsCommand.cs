namespace Silo.Application.Features;
public class SaveFixedConflictsCommand
{
    public string WarehouseCode { get; set; }
    public string Desc { get; set; }
    public List<GetInventoryConflictDetailsVm> Serials { get; set; } = new();
}
