namespace Silo.Application.Features;
public class RegisterByCodeSerialCountCommand
{
    public string Serial { get; set; }
    public string ProductCode { get; set; }
    public string RefCode { get; set; }
    public string Count { get; set; }
    public string Epc { get; set; }
    public string Zone { get; set; }
    public string UserToken { get; set; }
    public string Line { get; set; } = "0";
    public string Shift { get; set; } = "0";
    public string DestinationCode { get; set; } = "0";
    public JToken Properties { get; set; } = null;
}
