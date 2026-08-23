namespace Silo.Application.Features;

public class GetProductByZoneAndProductCodeVm
{
    public string ProductCode { get; set; }
    public string ZoneCode { get; set; }
    public string TechnicalCode { get; set; }
    public string Quality { get; set; }
    public int Count { get; set; }
    public decimal SumCount { get; set; }
    public string FirstDate { get; set; }
    public string LastDate { get; set; }
    public string Details { get; set; }
}