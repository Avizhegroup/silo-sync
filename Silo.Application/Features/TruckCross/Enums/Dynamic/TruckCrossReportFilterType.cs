namespace Silo.Application.Features;

public enum TruckCrossReportFilterType
{
    // Date filters
    FromDate,
    ToDate,
    
    // Driver information
    NationalCode,
    DriverName,
    
    // Vehicle information
    PlaqueFirstPart,
    PlaqueCharacter,
    PlaqueSecondPart,
    PlaqueCityPart,
    
    // Present section filters
    PresentCause,
    PresentOperationType,
    PresentShipment,
    PresentOperationDestination,
    PresentCustomer,
    
    // Status filter
    Status,
    
    // Product filter
    ProductTitle,
    
    // Dynamic fields from JSON
    DynamicFields
}
