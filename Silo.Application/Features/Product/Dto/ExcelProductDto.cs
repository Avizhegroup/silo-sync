namespace Silo.Application.Features;

public class ExcelProductDto
{
    public int ProductId { get; set; }
    public string ProductSerial { get; set; } = string.Empty;
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public string ProductENTitle { get; set; }
    public string ProductPackValue { get; set; }
    public string ProductCountInPack { get; set; }
    public string ProductValue { get; set; }
    public string ProductPackWeight { get; set; }
    public string ProductPackVolume { get; set; }
    public string ProductTechnicalCode { get; set; }
    public string ProductProperties { get; set; } = "";
    public string ProductType { get; set; }
    public string ProductStatus { get; set; }
    public string ProductSize { get; set; }
    public string ProductUnit { get; set; }
    public string ProductRegUser { get; set; }
    public string ProductRegDateTime { get; set; }
    public int ProductGalleryId { get; set; }
    public string ProductTechnicalData { get; set; }
    public string ProductGroup { get; set; }
    public string ProductBrand { get; set; }
}
