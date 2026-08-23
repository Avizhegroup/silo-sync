namespace Silo.Application.Features;
public class TruckCrossHeaderPrintDto
{
    public int Id { get; set; }

    public DateTime? PresentDateTime { get; set; }

    public string? PresentDate { get; set; }

    public string? PresentTime { get; set; }

    public string? PresentUser { get; set; }

    public string? NationalCode { get; set; }

    public string? DriverName { get; set; }

    public string? CompanyTitle { get; set; }

    public string? PassportCode { get; set; }

    public string? DriverPhone { get; set; }

    public string? LicenseCode { get; set; }

    public string? Plaque { get; set; }

    public string? InternationalPlaque { get; set; }

    public string? TruckTypeTitle { get; set; }

    public string? TypeDesc { get; set; }

    public string? PresentCauseTitle { get; set; }

    public string? OperationTypeTitle { get; set; }

    public string? PresentShipmentTitle { get; set; }

    public string? PresentShipmentNumber { get; set; }

    public string? PresentCustomerTitle { get; set; }

    public string? OperationDestinationTitle { get; set; }

    public string? PresentDesc { get; set; }

    public int PresentTurn { get; set; }

    public DateTime? EnterDateTime { get; set; }

    public string? EnterDate { get; set; }
    public string? EnterTime { get; set; }

    public string? EnterUser { get; set; }

    public decimal EnterWeightTonage { get; set; }

    public string? AcceptPlaceTitle { get; set; }

    public string? EnterAcceptor { get; set; }

    public DateTime? ExitDateTime { get; set; }

    public string? ExitDate { get; set; }

    public string? ExitTime { get; set; }

    public string? ExitUser { get; set; }

    public decimal ExitWeightTonage { get; set; }

    public decimal ExitPureWeightCargo { get; set; }

    public string? ExitWeightbridgeReceiptNumber { get; set; }

    public string? ExitCargoOwnerName { get; set; }

    public string? ExitDeliveryAddress { get; set; }

    public string? ExitCargoOwnerPhone { get; set; }

    public string? ExitDesc { get; set; }

    public string? ExitPaymentTypeTitle { get; set; }

    public string? ExitTotalCost { get; set; }

    public string? ExitUnitPrice { get; set; }

    public string? ExitDistance { get; set; }

    public string? ExitDestination { get; set; }

    public string? GateOpCode { get; set; }
    
    public string? DocumentId { get; set; }

    public string? GateCode { get; set; }

    public string? RelatedCargos { get; set; }
    
    public string? DynamicFieldsDisplayText { get; set; }
    
    // Dynamic field properties (supports up to 20 dynamic fields)
    public string? DynamicField1 { get; set; }
    public string? DynamicField2 { get; set; }
    public string? DynamicField3 { get; set; }
    public string? DynamicField4 { get; set; }
    public string? DynamicField5 { get; set; }
    public string? DynamicField6 { get; set; }
    public string? DynamicField7 { get; set; }
    public string? DynamicField8 { get; set; }
    public string? DynamicField9 { get; set; }
    public string? DynamicField10 { get; set; }
    public string? DynamicField11 { get; set; }
    public string? DynamicField12 { get; set; }
    public string? DynamicField13 { get; set; }
    public string? DynamicField14 { get; set; }
    public string? DynamicField15 { get; set; }
    public string? DynamicField16 { get; set; }
    public string? DynamicField17 { get; set; }
    public string? DynamicField18 { get; set; }
    public string? DynamicField19 { get; set; }
    public string? DynamicField20 { get; set; }
    
    public string? DynamicData { get; set; }

    private Dictionary<int, string>? _dynamicDataDict;
    public Dictionary<int, string> DynamicDataDict
    {
        get
        {
            if (_dynamicDataDict == null)
            {
                _dynamicDataDict = DynamicData.HasNoValue()
                    ? new Dictionary<int, string>()
                    : JsonSerializer.Deserialize<Dictionary<int, string>>(DynamicData);
            }

            return _dynamicDataDict;
        }
        set
        {
            _dynamicDataDict = value;

            DynamicData = JsonSerializer.Serialize(value);
        }
    }
}
