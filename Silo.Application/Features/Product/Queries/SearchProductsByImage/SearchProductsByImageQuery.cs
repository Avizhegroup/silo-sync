namespace Silo.Application.Features;

public class SearchProductsByImageQuery : IRequest<SearchProductsByImageVm>
{
    public List<ProductImageFileDto> Images { get; set; } = [];
    public int MaxResults { get; set; } = 20;
    public double MinScore { get; set; } = 5;
}
