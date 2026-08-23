namespace Silo.Application.Features;
public class GetLoadedCargoProductsDto
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductSerial { get; set; }
    public decimal ProductCount { get; set; }
    public string DocumentCode { get; set; }
}
