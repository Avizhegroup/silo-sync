using Microsoft.Extensions.Logging;
using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;

public class AnalyzeProductImageHandler(WmsApiContext context
    , IAiApiClient aiApiClient
    , ILogger<AnalyzeProductImageHandler> logger) : IRequestHandler<AnalyzeProductImageCommand, AnalyzeProductImageVm>
{
    public async Task<AnalyzeProductImageVm> Handle(AnalyzeProductImageCommand request, CancellationToken cancellationToken)
    {
        if (request.GalleryId <= 0)
        {
            return new() { Succeeded = false, Message = "NoImage" };
        }

        var gallery = await context.GalleryMedias.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GalleryId, cancellationToken);

        if (gallery is null || !File.Exists(gallery.MediaPath))
        {
            return new() { Succeeded = false, Message = "NoImage" };
        }

        var mediaType = ProductImageInputFactory.MediaTypeFromFileName(gallery.MediaPath)
            ?? ProductImageInputFactory.MediaTypeFromFileName(gallery.MediaName);

        if (mediaType is null)
        {
            return new() { Succeeded = false, Message = "UnsupportedImageType" };
        }

        var bytes = await File.ReadAllBytesAsync(gallery.MediaPath, cancellationToken);

        if (bytes.Length == 0 || bytes.Length > ProductPatternHelper.MaxImageBytes)
        {
            return new() { Succeeded = false, Message = "ImageSizeInvalid" };
        }

        List<AiImageInput> images = [new AiImageInput(bytes, mediaType, gallery.MediaName)];

        try
        {
            var text = await aiApiClient.AnalyzeImagesAsync(images, ProductPatternHelper.PromptFor(aiApiClient.ProductIdentifyKey), aiApiClient.ProductIdentifyKey, cancellationToken);
            var pattern = ProductPatternHelper.Parse(text);

            if (pattern.IsEmpty)
            {
                return new() { Succeeded = false, Message = "EmptyAnalysis" };
            }

            return new()
            {
                Succeeded = true,
                Pattern = ProductPatternHelper.Serialize(pattern),
                Summary = pattern.Summary,
                Keywords = pattern.Keywords
            };
        }
        catch (AiImageAnalysisException ex)
        {
            logger.LogWarning(ex, "Product image analysis rejected ({Status})", ex.StatusCode);

            return new() { Succeeded = false, Message = ex.Message };
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Product image analysis failed");

            return new() { Succeeded = false, Message = "AiUnavailable" };
        }
    }
}
