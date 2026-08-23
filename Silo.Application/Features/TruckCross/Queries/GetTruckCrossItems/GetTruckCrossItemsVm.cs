namespace Silo.Application.Features;
public class GetTruckCrossItemsVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TruckCrossItemTypesEnum Type { get; set; }
    public string ProductUnit { get; set; }
    public decimal ProductCount { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public int TruckCrossProductTypeId { get; set; }
    public string TruckCrossProductTypeTitle { get; set; }
    public long TruckCrossId { get; set; }
}

public enum TruckCrossItemTypesEnum
{
    Enter = 1,
    Exit = 2
}
