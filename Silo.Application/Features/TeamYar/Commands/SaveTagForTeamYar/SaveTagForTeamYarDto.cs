namespace Silo.Application.Features;
public class SaveTagForTeamYarDto
{
    /// <summary>
    /// کدکالا
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductCode { get; set; }

    /// <summary>
    /// کد سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string DocumentCode { get; set; }

    /// <summary>
    /// نوع سند
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int DocumentType { get; set; }

    /// <summary>
    /// سری ساخت
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductSerial { get; set; }

    /// <summary>
    /// مقدار اصلی
    /// </summary>
    public decimal? OriginalCount { get; set; }

    /// <summary>
    /// مقدار فرعی
    /// </summary>
    public decimal? SubCount { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// کد انبار
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string WarehouseCode { get; set; }

    /// <summary>
    /// کد نوع کالا
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string ProductTypeCode { get; set; }

    /// <summary>
    ///  واحد برای مقدار اصلی
    /// </summary>
    public string? UnitForOriginalCount { get; set; }

    /// <summary>
    ///  واحد برای مقدار فرعی
    /// </summary>
    public string? UnitForSubCount { get; set; }

    /// <summary>
    /// فیلد های چاپ
    /// </summary>
    public List<SaveTagForTeamYarPrintDto> PrintData { get; set; }
}
