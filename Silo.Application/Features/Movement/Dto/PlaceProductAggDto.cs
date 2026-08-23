namespace Silo.Application.Features;

public class PlaceProductAggDto
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
    public string Status { get; set; }
}
