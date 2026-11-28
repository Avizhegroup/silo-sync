namespace Silo.Application.Features;

public class SaveDocumentForSaleHeaderDto
{
    /// <summary>
    /// کد سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DocumentCode { get; set; }

    /// <summary>
    /// کد سند مرجع
    /// </summary>
    public string? RefCode { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Date { get; set; }

    /// <summary>
    /// نوع عملیات
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public SaveDocumentForSaleOpType OpType { get; set; }

    /// <summary>
    /// آیتم‌ها
    /// </summary>
    public List<SaveDocumentForSaleItemsDto> Items { get; set; }
}
