namespace Silo.Application.Features;

public class GetAllPlacementOrderByProductCodeVm
{
    public int OrderCode { get; set; }
    public int Status { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string Zones { get; set; } = string.Empty;
    public int Count { get; set; }
    public string DateTime { get; set; }
    public string Username { get; set; }
    public string RegCode { get; set; }
    public int PackRemain { get; set; }
    public decimal SumValue { get; set; }
}
