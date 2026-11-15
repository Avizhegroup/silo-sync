namespace Silo.Application.Features;

public class SaveDocumentForGeneralHeaderDto
{
    /// <summary>
    /// کد سند
    /// </summary>
    [Required]
    public string DocumentCode { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    [Required]
    public string Date { get; set; }

    /// <summary>
    /// کد انبار مبدأ
    /// </summary>
    [Required]
    public string OriginWarehouseCode { get; set; }

    /// <summary>
    /// کد انبار مقصد
    /// </summary>
    [Required]
    public string DestinationWarehouseCode { get; set; }

    /// <summary>
    /// نوع سند
    /// </summary>
    [Required]
    public int DocumentType { get; set; }

    /// <summary>
    /// نوع عملیات
    /// </summary>
    [Required]
    public SaveDocumentForSaleOpType OpType { get; set; }

    public List<SaveDocumentForGeneralItemsDto> Items { get; set; }
}
