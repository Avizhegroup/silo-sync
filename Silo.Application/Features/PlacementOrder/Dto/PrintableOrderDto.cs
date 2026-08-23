namespace Silo.Application.Features;
public class PrintableOrderDto
{
    public string ProductCode { get; set; }
    public string ProductTechnicalCode { get; set; }
    public string ProductName { get; set; }
    public decimal SumValue { get; set; }
    public string CountProduct { get; set; }
    public string Location { get; set; }
    public string SerialList { get; set; } = string.Empty;
}
