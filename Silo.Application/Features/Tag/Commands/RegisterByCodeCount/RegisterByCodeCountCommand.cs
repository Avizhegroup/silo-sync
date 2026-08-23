using System.Text.Json.Serialization;

namespace Silo.Application.Features;
public class RegisterByCodeCountCommand
{
    [JsonPropertyName("epcs")]
    public string[] Epcs { get; set; }

    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; }

    [JsonPropertyName("refCode")]
    public string RefCode { get; set; }

    [JsonPropertyName("count")]
    public string Count { get; set; }

    [JsonPropertyName("zone")]
    public string Zone { get; set; }

    [JsonPropertyName("userToken")]
    public string UserToken { get; set; }

    [JsonPropertyName("line")]
    public string Line { get; set; } = "0";

    [JsonPropertyName("shift")]
    public string Shift { get; set; } = "0";

    [JsonPropertyName("destinationCode")]
    public string DestinationCode { get; set; } = "0";

    [JsonPropertyName("properties")]
    public JToken Properties { get; set; } = null;
}
