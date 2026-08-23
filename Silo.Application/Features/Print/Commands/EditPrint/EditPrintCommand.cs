namespace Silo.Application.Features;

public class EditPrintCommand : IRequest<EditPrintVm>
{
    public string ProductSerial { get; set; }
    public string ProductName { get; set; }
    public string ProductRegCode { get; set; }
    public decimal? ProductPackWeight { get; set; }
    public string DocumentId { get; set; }
    public string ProductProductionShift { get; set; }
    public string ProductProductionLine { get; set; }
    public string DestinationCode { get; set; }
    public string ProductStatusCode { get; set; }
    public string ProductProperties { get; set; }
    public decimal? ProductCount { get; set; }
}
