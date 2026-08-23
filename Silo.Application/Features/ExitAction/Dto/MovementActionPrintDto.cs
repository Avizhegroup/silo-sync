namespace Silo.Application.Features;
public class MovementActionPrintDto
{
    public string ActionTypeTitle { get; set; } = string.Empty;
    public string OpCode { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
    public string? User { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string GateCode { get; set; } = string.Empty;
    public string GateOp { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string StationNames { get; set; } = string.Empty;
    public string ActionDocumentId { get; set; } = string.Empty;
    public string MovementActionDesc { get; set; } = string.Empty;
    public string MovementActionData { get; set; } = string.Empty;
    public List<TagMovementPrintDto> TagMovements { get; set; }
    public List<ExitActionPrintMainDto> ExitPrints { get; set; }
    public List<EnterActionPrintMainDto> EnterPrints { get; set; }
    public GetActionTruckCrossVm TruckCross { get; set; }
}
