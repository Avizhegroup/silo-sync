namespace Silo.Application.Dto;

public class ExitActionDto
{
    public int GateOperationCode { get; set; }
    public string Gate { get; set; }
    public string DocumentId { get; set; }
    public string MovementActionData { get; set; }
    public string MovementActionDesc { get; set; }
    public string SourceWarehouseCode { get; set; }
    public string DestinationWarehouseCode { get; set; }
    public string TruckCrossId { get; set; }
}
