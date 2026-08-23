namespace Silo.Application.Features;

public class GetTruckCrossReportQuery
{
    public string NationalCode { get; set; }
    public string DriverName { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public int PresentCause { get; set; }
    public int PlaqueFirstPart { get; set; }
    public string PlaqueCharacter { get; set; }
    public int PlaqueSecondPart { get; set; }
    public int PlaqueCityPart { get; set; }
    public int PresentOperationTypeId { get; set; }
    public int PresentShipmentId { get; set; }
    public int PresentOperationDestinationId { get; set; }
    public int PresentCustomerId { get; set; }
    public string ProductTitle { get; set; }
    public TruckCrossStatuses Status { get; set; } = TruckCrossStatuses.None;
}
