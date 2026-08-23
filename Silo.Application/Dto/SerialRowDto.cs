
namespace Silo.Application;

public class SerialRowDto
{
    public string TagZone { get; set; }
    public string ProductSerial { get; set; }
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string RegCode { get; set; }
    public decimal ProductCount { get; set; }
    public Dictionary<string, string> DynamicData { get; set; } = new();
}
