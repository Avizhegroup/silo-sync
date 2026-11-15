namespace Silo.Application.Features;

public class SaveDocumentForGeneralItemsDto
{
    /// <summary>
    /// کدکالا
    /// </summary>
    [Required]
    public string ProductCode { get; set; }

    /// <summary>
    /// مقدار 
    /// </summary>
    [Required]
    public decimal Count { get; set; }

    /// <summary>
    ///  واحد
    /// </summary>
    public string? Unit { get; set; }
}
