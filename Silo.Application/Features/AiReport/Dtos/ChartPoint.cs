namespace Silo.Application.Shared.Features;
public class ChartPoint
{
    public string Key { get; set; } = string.Empty;     
    public string Category { get; set; } = string.Empty;
    public string Short { get; set; } = string.Empty;    
    public string Label { get; set; } = string.Empty; 
    public string Color { get; set; } = string.Empty;
    public double Value { get; set; }
}
