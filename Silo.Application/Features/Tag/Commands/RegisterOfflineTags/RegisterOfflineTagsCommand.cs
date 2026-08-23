namespace Silo.Application.Features;
public class RegisterOfflineTagsCommand
{
    public List<string> TagEpc { get; set; }
    public List<string> ProductSerial { get; set; }
    public List<string> Location { get; set; } = null;
    public string ProductCode { get; set; }
    public string RegisterUser { get; set; }
    public string ProductProduceLine { get; set; }
    public string ProductProduceShift { get; set; }
    public string ProductDocCode { get; set; }
    public string ProductZone { get; set; }
    public string ProductProperties { get; set; }
    public string DestinationCode { get; set; }
 }
