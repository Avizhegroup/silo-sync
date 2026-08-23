namespace Silo.Application.Features;
public class TruckCrossDataDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Code))]
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    public string Plaque { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    [MinLength(2, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    //   [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string FirstPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    // [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Character { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    //  [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [MinLength(3, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    //  [RegularExpression("[0-9][0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    public string SecondPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Plaque))]
    //  [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [MinLength(2, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    [RegularExpression("[0-9][0-9]", ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_BadFormat))]
    public string CityPart { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_DriverName))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DriverName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Phone))]
    //  [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DriverPhone { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Company))]
    public int TruckCrossCompanyId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_NationalCode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string NationalCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_TypeTruck))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int TypeId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_TypeTruck))]
    public string? TruckTypeTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Status))]
    public TruckCrossStatuses TruckCrossStatus { get; set; }

    #region Present
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_Cause))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? PresentCause { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Turn))]
    public int PresentTurn { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_DateTime))]
    public DateTime? PresentDateTime { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Present_Desc))]
    public string PresentDesc { get; set; }

    public string PresentUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Present_User))]
    public string PresentUsername { get; set; }

    public bool PresentIsSaved { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Operation_Type))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? PresentOperationTypeId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Operation_Type))]
    public string? PresentOperationTypeTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Operation_Destination))]
    public int PresentOperationDestinationId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Operation_Destination))]
    public string? PresentOperationDestinationTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Shipment))]
    public int PresentShipmentId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Shipment))]
    public string? PresentShipmentTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Shipment_Number))]
    public string PresentShipmentNumber { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Customer))]
    public int PresentCustomerId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Customer))]
    public string PresentCustomerTitle { get; set; }

    public string PresentRevokeUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_UserRevoke))]
    public string PresentRevokeUsername { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductRevokeDateTime))]
    public DateTime? PresentRevokeDateTime { get; set; }
    #endregion

    #region Enter
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_DateTime))]
    public DateTime? EnterDateTime { get; set; }

    public string EnterUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_User))]
    public string EnterUsername { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage))]
    public decimal EnterWeightTonage { get; set; }

    public bool EnterIsSaved { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Enter_AcceptPlace))]
    public int? EnterAcceptPlaceId { get; set; }
    #endregion

    #region Exit
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_DateTime))]
    public DateTime? ExitDateTime { get; set; }
    public string ExitUserId { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_User))]
    public string ExitUsername { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_WeightTonage))]
    public decimal ExitWeightTonage { get; set; }
    public int ExitGateId { get; set; }
    public bool ExitIsSaved { get; set; } = false;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_PureWeightCargo))]
    public decimal ExitPureWeightCargo { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_CargoOwnerName))]
    public string ExitCargoOwnerName { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_ShipmentCost))]
    public string ExitShipmentCost { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_PaymentType))]
    public int ExitPaymentType { get; set; } = -1;

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_UnitPrice))]
    public string ExitUnitPrice { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_TotalCost))]
    public string ExitTotalCost { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_TruckCross_Exit_Distance))]
    public string ExitDistance { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_GateOpCode))]
    public string? GateOperationCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_MovementAction))]
    public string? MovementActionId { get; set; }
    #endregion

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

    public string OneRowData
    {
        get => $"{TextResources.APP_StringKeys_Driver}: {DriverName} | {TextResources.APP_StringKeys_Plaque}: {Plaque}";
    }
}
