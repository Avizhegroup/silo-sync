namespace Silo.Application.Features;

public class SearchProductsByImageVm
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public string QuerySummary { get; set; }
    public List<string> QueryKeywords { get; set; } = [];
    public List<ProductImageMatchVm> Items { get; set; } = [];
}

public class ProductImageMatchVm
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductTitle { get; set; }
    public int ProductGalleryId { get; set; }
    public string PatternSummary { get; set; }
    public double Score { get; set; }
}
