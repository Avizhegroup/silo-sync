namespace Silo.Application.Dto;

public class ProductEnter
{
    public string PaleteCode { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string TechnicalCode { get; set; }
    public string Size { get; set; }
    public string Length { get; set; }
    public string QualityGrade { get; set; }
}

public class Enter
{
    public string Machine { get; set; }
    public string Driver { get; set; }
    public string Status { get; set; }
    public string Desc { get; set; }
    public string Location { get; set; }
    public string SummaryEnterInDay { get; set; }
    public string SummaryEnterInShift { get; set; }
    public string ProductEnterSerial { get; set; }
    public string ProductEnterProductName { get; set; }
    public string ProductEnterProductCode { get; set; }
    public string ProductEnterProductLine { get; set; }
    public string ProductEnterProductTechnicalCode { get; set; }
    public string ProductEnterProductSize { get; set; }
    public string ProductEnterProductCount { get; set; }
    public string ProductEnterProductStatus { get; set; }
    public List<ProductEnter> Products { get; set; }
}

public class PositionProductSearch
{
    public string MProductTitle { get; set; }= "-1";
    public string MProductCode { get; set; } = "-1";
    public string MTechCode { get; set; }    = "-1";
    public string MSize { get; set; }        = "-1";
    public string MQuality { get; set; } = "-1";
    public bool IsActive { get; set; } = true;
}

public class PositionSearchRequest
{
    public string ProductCode { get; set; }
    public string ZoneCode { get; set; }
}

public class ProductTypeRequest
{
    public string MCode { get; set; }
    public string MTitle { get; set; }

}
public class GateResult
{
    public string Row { get; set; }
    public string ProductCode { get; set; }
    public string ProductTechnicalCode { get; set; }
    public string ProductName { get; set; }
    public string Count { get; set; }
    public string SumValue { get; set; }
    public string ProductSerial { get; set; }
    public string TagSerial { get; set; }
    public string ProductType { get; set; }
    public string ProductStatus { get; set; }
    public string TagStatus { get; set; }
    public string TagInDestinationId { get; set; }
    public string EnterDate { get; set; }
    public string ProductLine { get; set; }
    public string ProductShift { get; set; }
    public string Lock { get; set; }
    public string DocumentId { get; set; }
    public string Freeze { get; set; }
    public string ProductOldSerial { get; set; }
    public string InspectStatus { get; set; }
    public string TagZone { get; set; }
    public string ProductGroupCode { get; set; }
    public string ProductSubGroupCode { get; set; }
    public string ProductSizeCode { get; set; }
    public string ProductBrandCode { get; set; }
    public string PMToStoreCode { get; set; }
    public string PMToStoreTitle { get; set; }
    public string PMToZoneCode { get; set; }
    public string LastInspectResult { get; set; }
    public string CheckResultType { get; set; }
    public string CheckActionStatus { get; set; }
    public string ExceptionMessage { get; set; }
    public string ProductProperties { get; set; }


    public DateTime TagRegisterDateTime { get; set; }

 


}
