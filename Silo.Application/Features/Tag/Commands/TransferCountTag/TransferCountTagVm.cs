namespace Silo.Application.Features;
public class TransferCountTagVm
{
    public string SourceSerial { get; set; }
    public string DestinationSerial { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public decimal TransferredQuantity { get; set; }
}
