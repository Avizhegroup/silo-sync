namespace Silo.Application.Features;

public class PlaceProductBySerialDto
{
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string RegCode { get; set; }
    public decimal SumCount { get; set; }
    public string Status { get; set; }
    public bool IsEditMode { get; set; } = false;
}
