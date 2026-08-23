namespace Silo.Application.Features;
public class EnterActionPrintMainDto
{
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductName { get; set; }
    public int Count { get; set; } = 0;
    public decimal SumCount { get; set; } = 0;
}
