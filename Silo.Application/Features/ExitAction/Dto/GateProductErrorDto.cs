namespace Silo.Application.Features;

public class GateProductErrorDto
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductSerial { get; set; }
    public string Regcode { get; set; } = "";
    public decimal SumCount { get; set; }
    public string TagEpc { get; set; }
    public string Error { get; set; }
}
