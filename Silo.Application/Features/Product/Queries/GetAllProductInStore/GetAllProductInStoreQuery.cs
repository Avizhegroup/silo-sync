using System.ComponentModel.DataAnnotations;

namespace Silo.Application.Features;

public class GetAllProductInStoreQuery
{
    public string ProductCode { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductStatus { get; set; }
    public string ProductSerial { get; set; }
    public string TagZone { get; set; }
    public string AgeRange { get; set; } = "-1";
    public bool TechnicalCodeLike { get; set; } = true;
    public bool TagZoneLike { get; set; } = true;
    public string RowCode { get; set; }
    public string EnterStatus { get; set; }
    public string Qc { get; set; } = "-1";
    public string Size { get; set; } = "-1";

    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [Display(ResourceType = typeof(TextResources) , Name = nameof(TextResources.APP_StringKeys_Field_Warehouse_Code))]
    public string WarehouseCode { get; set; }
}
