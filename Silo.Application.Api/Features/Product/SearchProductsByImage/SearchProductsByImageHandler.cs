using Microsoft.Extensions.Logging;
using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;

public class SearchProductsByImageHandler(WmsApiContext context
    , IAiApiClient aiApiClient
    , ILogger<SearchProductsByImageHandler> logger) : IRequestHandler<SearchProductsByImageQuery, SearchProductsByImageVm>
{
    public async Task<SearchProductsByImageVm> Handle(SearchProductsByImageQuery request, CancellationToken cancellationToken)
    {
        var (images, error) = ProductImageInputFactory.Create(request.Images);

        if (error is not null)
        {
            return new() { Succeeded = false, Message = error };
        }

        ProductPattern query;

        try
        {
            var text = await aiApiClient.AnalyzeImagesAsync(images, ProductPatternHelper.Prompt, null, cancellationToken);
            query = ProductPatternHelper.Parse(text);
        }
        catch (AiImageAnalysisException ex)
        {
            logger.LogWarning(ex, "Image search rejected ({Status})", ex.StatusCode);

            return new() { Succeeded = false, Message = ex.Message };
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Image search failed");

            return new() { Succeeded = false, Message = "AiUnavailable" };
        }

        if (query.IsEmpty)
        {
            return new() { Succeeded = false, Message = "EmptyAnalysis" };
        }

        var candidates = await context.Products.AsNoTracking()
            .Where(p => p.ProductPattern != null && p.ProductPattern != "" && (p.ProductIsActive == null || p.ProductIsActive == true))
            .Select(p => new { p.Id, p.Code, p.Title, p.ProductGalleryId, p.ProductPattern })
            .ToListAsync(cancellationToken);

        var items = ProductPatternMatcher.Rank(query
            , candidates.Select(c => new ProductPatternCandidate(c.Id, c.Code, c.Title, c.ProductGalleryId, c.ProductPattern))
            , request.MinScore
            , request.MaxResults);

        return new()
        {
            Succeeded = true,
            QuerySummary = query.Summary,
            QueryKeywords = query.Keywords,
            Items = items
        };
    }
}
