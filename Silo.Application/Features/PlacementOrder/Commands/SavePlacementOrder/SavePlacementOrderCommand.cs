namespace Silo.Application.Features;

public class SavePlacementOrderCommand
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Zones { get; set; } = string.Empty;
    public string Count { get; set; } = "0";
}