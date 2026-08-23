namespace Silo.Application.Features;

public class GetInventoryConflictDetailsVm
{
    public bool IsSelected { get; set; } = false;
    public string ProductSerial { get; set; }
    public string Epc { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string RegCode { get; set; }
    public string Date { get; set; }
    public decimal ProductCount { get; set; }
    public string Zone { get; set; }
    public string Place { get; set; }
    public string Desc { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public string ContractStatus { get; set; }
    public string DestinationTitle { get; set; }
}
