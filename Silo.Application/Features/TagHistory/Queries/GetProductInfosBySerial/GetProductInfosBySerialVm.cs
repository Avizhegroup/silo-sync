namespace Silo.Application.Features;

public class GetProductInfosBySerialVm
{
    public string TagEpc { get; set; }
    public string ProductSerial { get; set; }
    public string ProductCode { get; set; }
    public decimal ProductCount { get; set; }
    public string ProductName { get; set; }
    public string ProductStatusTitle { get; set; }
    public string ProductTypeTitle { get; set; }
    public string TagStatus { get; set; } = "0";
    public string TagRegisterShamsiUnixDate { get; set; }
    public string ProductProperties { get; set; }
    public string ProductAgeAnalysis { get; set; }
    public string RegCode { get; set; }
    public string fld_ProductPropertyATitle { get; set; }
    public string fld_ProductPropertyBTitle { get; set; }
    public string ProductENTitle { get; set; }
    public string FreezeStatus { get; set; }
    public decimal ProductPackWeight { get; set; }
    public decimal ProductPackVolume { get; set; }
    public decimal ProductCountInPack { get; set; }
    public decimal ProductValue { get; set; }
    public string ProductSize { get; set; }
    public string ProductUnit { get; set; }
    public string Warehouse { get; set; }
    public string TagZone { get; set; }
    public string TagStatusTitle { get; set; }
    public string Username { get; set; }
    public string ProductAge { get; set; }
    public int ProductGalleryId { get; set; }
    public string ImageBase64 { get; set; }
    public string OldProductSerial { get; set; } = "";
    public string DocumentCode { get; set; } = "";
    public string? ProductTechnicalData { get; set; }
    public string LastInspectResult { get; set; }
    public string ProductBrand { get; set; }
    public string ProductGroup { get; set; }
    public string ProductSubGroup { get; set; }
    public string RegisterDevice { get; set; }
    public string RegisterUserName { get; set; }

    public string RegisterActionDate { get; set; }
    public ProductDto ProductInfo { get; set; }
}
