namespace Silo.Application.Features;

public class SaveDocumentForGeneralHeaderDto
{
    /// <summary>
    /// کد سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DocumentCode { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Date { get; set; }

    /// <summary>
    /// کد انبار مبدأ
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string OriginWarehouseCode { get; set; }

    /// <summary>
    /// کد انبار مقصد
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DestinationWarehouseCode { get; set; }

    /// <summary>
    /// نوع سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int DocumentType { get; set; }

    /// <summary>
    /// نوع عملیات
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public SaveDocumentForSaleOpType OpType { get; set; }

    public List<SaveDocumentForGeneralItemsDto> Items { get; set; }
}
