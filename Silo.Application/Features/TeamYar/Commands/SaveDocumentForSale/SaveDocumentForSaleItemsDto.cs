namespace Silo.Application.Features;

public class SaveDocumentForSaleItemsDto
{
    /// <summary>
    /// کدکالا
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductCode { get; set; }

    /// <summary>
    /// مقدار 
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public decimal Count { get; set; }

    /// <summary>
    ///  واحد
    /// </summary>
    public string? Unit { get; set; }
}
