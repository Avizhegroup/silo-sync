using System.Text.Json.Nodes;

namespace Silo.Application.Features;
public class SaveProductTerchnicalDataCommand
{
    public string ProductCode { get; set; }
    public JsonObject Data { get; set; }
}
