using Silo.Application.Features;

namespace Silo.Application.Dto;

public class WarehouseDto
{
    public int Id { get; set; }
    public string DestinationCode { get; set; }
    public string DestinationTitle { get; set; }
    public int OperationalType { get; set; } 
    public DestinationInventoryType InventoryType { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string Epc { get; set; }
    public string Coordinates { get; set; }
}
