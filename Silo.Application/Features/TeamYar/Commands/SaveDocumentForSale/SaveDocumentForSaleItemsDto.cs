namespace Silo.Application.Features;

public class SaveDocumentForSaleItemsDto
{
    /// <summary>
    /// کدکالا
    /// </summary>
    public string ProductCode { get; set; }

    /// <summary>
    /// مقدار 
    /// </summary>
    public decimal Count { get; set; }

    /// <summary>
    ///  واحد
    /// </summary>
    public string Unit { get; set; }
}
