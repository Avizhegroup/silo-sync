namespace Silo.Application.Features;
public class GetPlaceByTruckCrossIdDto
{
    public string DocumentCode { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public decimal ProductCount { get; set; }
    public decimal SumCount { get; set; }
}
