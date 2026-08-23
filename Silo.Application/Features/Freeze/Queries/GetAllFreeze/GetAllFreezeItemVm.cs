namespace Silo.Application.Features;

public class GetAllFreezeItemVm
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductName { get; set; }
    public decimal Count { get; set; }
}