namespace Silo.Application.Features;

public class SaveDocumentForSaleHeaderDto
{
    /// <summary>
    /// کد سند
    /// </summary>
    [Required]
    public string DocumentCode { get; set; }

    /// <summary>
    /// کد سند مرجع فروش
    /// </summary>
    public string? RefSalesCode { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// تاریخ
    /// </summary>
    [Required]
    public string Date { get; set; }

    [Required]
    public SaveDocumentForSaleOpType OpType { get; set; }
}
