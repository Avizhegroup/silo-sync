namespace Silo.Application.Features;
public class GetActionTruckCrossVm
{
    public string DriverName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public string LicenseCode { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public string Plaque { get; set; } = string.Empty;
    public string TypeTitle { get; set; } = string.Empty;
    public decimal EnterWeightTonage { get; set; }
    public decimal ExitWeightTonage { get; set; }
    public string MovementActionId { get; set; } = string.Empty;
}
