namespace Silo.Application.Features;

public enum TruckCrossReportColumnsType
{
    // ID and Status
    Id,
    TruckCrossStatus,
    
    // Date columns
    PersianDateFull,
    PersianDateYear,
    PersianDateMonth,
    PersianDateDay,
    GregorianDateFull,
    GregorianDateYear,
    GregorianDateMonth,
    GregorianDateDay,
    
    // Driver information
    DriverName,
    NationalCode,
    DriverPhone,
    
    // Vehicle information
    Plaque,
    TruckTypeTitle,
    
    // Present section columns
    PresentDateTime,
    PresentUsername,
    PresentCause,
    PresentDesc,
    PresentTurn,
    PresentOperationTypeTitle,
    PresentShipmentTitle,
    PresentShipmentNumber,
    PresentOperationDestinationTitle,
    PresentCustomerTitle,
    PresentRevokeUsername,
    PresentRevokeDateTime,
    
    // Enter section columns
    EnterDateTime,
    EnterUsername,
    EnterWeightTonage,
    
    // Exit section columns
    ExitDateTime,
    ExitUsername,
    ExitWeightTonage,
    ExitPureWeightCargo,
    ExitCargoOwnerName,
    ExitShipmentCost,
    ExitPaymentType,
    ExitUnitPrice,
    ExitTotalCost,
    ExitDistance,
    GateOperationCode,
    MovementActionId,
    
    // Dynamic fields from JSON
    DynamicFields,
    
    // Data mining elements
    DataMiningElements
}
