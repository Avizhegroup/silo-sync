namespace Silo.Application.Features;
public class SaveUHFReaderLogHeaderCommand
{
    public string? StationCode { get; set; }
    public string? ActionTypeCode { get; set; }
    public string? DocumentCode { get; set; }
    public long? TruckCrossId { get; set; }
    public int? ControlType { get; set; }
    public string? CarProperties { get; set; }
    public int? MovementActionId { get; set; }
}
