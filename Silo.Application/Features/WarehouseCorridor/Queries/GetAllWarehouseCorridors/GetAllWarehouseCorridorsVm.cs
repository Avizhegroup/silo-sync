namespace Silo.Application.Features;

public class GetAllWarehouseCorridorsVm
{
    public int Id { get; set; }
    public string ContextKey { get; set; } = string.Empty;
    public float X1 { get; set; }
    public float Z1 { get; set; }
    public float X2 { get; set; }
    public float Z2 { get; set; }
    public float Width { get; set; }
    public string? Label { get; set; }
}
