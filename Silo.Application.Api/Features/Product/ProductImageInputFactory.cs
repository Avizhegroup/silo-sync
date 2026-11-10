using Microsoft.AspNetCore.Http;
using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>Validates uploaded images and converts them to raw AI inputs (bytes, no text encoding).</summary>
public static class ProductImageInputFactory
{
    public static async Task<(List<AiImageInput> Images, string Error)> CreateAsync(IEnumerable<IFormFile> files
        , CancellationToken cancellationToken = default)
    {
        var list = new List<AiImageInput>();

        foreach (var file in files ?? [])
        {
            if (file is null || file.Length == 0)
            {
                continue;
            }

            var mediaType = ResolveMediaType(file.ContentType, file.FileName);

            if (!ProductPatternHelper.IsAllowedMediaType(mediaType))
            {
                return (null, "UnsupportedImageType");
            }

            if (file.Length > ProductPatternHelper.MaxImageBytes)
            {
                return (null, "ImageSizeInvalid");
            }

            using var ms = new MemoryStream((int)file.Length);

            await file.CopyToAsync(ms, cancellationToken);

            list.Add(new AiImageInput(ms.ToArray(), mediaType, file.FileName));
        }

        if (list.Count == 0)
        {
            return (null, "NoImage");
        }

        if (list.Count > ProductPatternHelper.MaxImages)
        {
            return (null, "TooManyImages");
        }

        return (list, null);
    }

    /// <summary>Browser content type when it is an allowed image type, otherwise derived from the extension.</summary>
    public static string ResolveMediaType(string contentType, string fileName)
        => ProductPatternHelper.IsAllowedMediaType(contentType)
            ? contentType.Trim().ToLowerInvariant()
            : MediaTypeFromFileName(fileName);

    public static string MediaTypeFromFileName(string fileName)
        => System.IO.Path.GetExtension(fileName)?.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => null
        };
}
