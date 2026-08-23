using System.Text.Json;

namespace Silo.Application.Features;
public class TransferProductDto
{
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductCode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ProductTitle))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [StringLength(250, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductTitle { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Chart_Regcode))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductTechnicalCode { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Product_Model))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductUnit { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Field_ProductType))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductType { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Qc_Code))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductStatus { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Size_Code))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductSize { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Brand_Code))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductBrand { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Group_Code))]
    [StringLength(50, ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Stringlength_Max))]
    public string ProductGroup { get; set; }

    public decimal ProductPackValue { get; set; }
    public decimal ProductCountInPack { get; set; } 
    public decimal ProductValue { get; set; }
    public decimal ProductPackWeight { get; set; }
    public decimal ProductPackVolume { get; set; }
    public JsonDocument ProductTechnicalData { get; set; }
}
