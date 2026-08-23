namespace Silo.Application.Features;

public class SaveProductCommand : ICloneable
{
    public int ProductId { get; set; }
    public string ProductSerial { get; set; } = string.Empty;
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public string ProductENTitle { get; set; }
    public decimal ProductPackValue { get; set; }
    public decimal ProductCountInPack { get; set; }
    public decimal ProductValue { get; set; }
    public decimal ProductPackWeight { get; set; }
    public decimal ProductPackVolume { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductTechnicalCode { get; set; }

    public string ProductProperties { get; set; } = "";
    
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductType))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Qc))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Size))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductSize { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Qc))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductUnit { get; set; }

    public string ProductRegUser { get; set; }
    public string ProductRegDateTime { get; set; }
    public int ProductGalleryId { get; set; }
    public string ProductTechnicalData { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductGroup))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductGroup { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductBrand))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductBrand { get; set; }

    public bool IsActive { get; set; }

    public string ProductClass { get; set; }
 
    public string ProductSubGroup { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class SaveProductCommandEnabilityCheck
{
    public bool ProductId { get; set; } = true;
    public bool ProductSerial { get; set; } = true;
    public bool ProductCode { get; set; } = true;
    public bool ProductTitle { get; set; } = true;
    public bool ProductENTitle { get; set; } = true;
    public bool ProductPackValue { get; set; } = true;
    public bool ProductCountInPack { get; set; } = true;
    public bool ProductValue { get; set; } = true;
    public bool ProductPackWeight { get; set; } = true;
    public bool ProductPackVolume { get; set; } = true;
    public bool ProductTechnicalCode { get; set; } = true;
    public bool ProductProperties { get; set; } = true;
    public bool ProductType { get; set; } = true;
    public bool ProductStatus { get; set; } = true;
    public bool ProductSize { get; set; } = true;
    public bool ProductUnit { get; set; } = true;
    public bool ProductRegUser { get; set; } = true;
    public bool ProductRegDateTime { get; set; } = true;
    public bool ProductGalleryId { get; set; } = true;
    public bool ProductTechnicalData { get; set; } = true;
    public bool ProductGroup { get; set; } = true;
    public bool ProductBrand { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool ProductClass { get; set; } = true;
    public bool ProductSubGroup { get; set; } = true;
}
