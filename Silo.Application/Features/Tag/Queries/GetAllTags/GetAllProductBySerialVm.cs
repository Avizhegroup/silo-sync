namespace Silo.Application.Features;

public class GetAllProductBySerialVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductName { get; set; }
    public string DateTime { get; set; }
    public string Shift { get; set; }
    public string Line { get; set; }
    public string Qc { get; set; }
    public string Size { get; set; }
    public string Warehouse { get; set; }
    public string Location { get; set; }
    public decimal Count { get; set; }
    public string FreezeStatus { get; set; }
    public bool IsChoosed { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public string Brand { get; set; }
}
