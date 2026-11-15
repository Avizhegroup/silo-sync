namespace Silo.Application.Features;

public class SaveDocumentForSaleHeaderDto
{
    /// <summary>
    /// کد سند
    /// </summary>
    [Required]
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
    [Required]
    public string Date { get; set; }

    /// <summary>
    /// نوع عملیات
    /// </summary>
    [Required]
    public SaveDocumentForSaleOpType OpType { get; set; }

    /// <summary>
    /// آیتم‌ها
    /// </summary>
    public List<SaveDocumentForSaleItemsDto> Items { get; set; }
}
