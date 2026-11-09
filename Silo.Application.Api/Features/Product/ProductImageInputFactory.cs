using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>Validates and decodes the images of an analysis request.</summary>
public static class ProductImageInputFactory
{
    public static (List<AiImageInput> Images, string Error) Create(IEnumerable<ProductImageFileDto> files)
    {
        var list = new List<AiImageInput>();

        foreach (var file in files ?? [])
        {
            if (string.IsNullOrWhiteSpace(file?.Base64))
            {
                continue;
            }

            if (!ProductPatternHelper.IsAllowedMediaType(file.ContentType))
            {
                return (null, "UnsupportedImageType");
            }

            byte[] data;

            try
            {
                var base64 = file.Base64;
                var comma = base64.IndexOf(',');
                data = Convert.FromBase64String(comma >= 0 && base64.StartsWith("data:") ? base64[(comma + 1)..] : base64);
            }
            catch (FormatException)
            {
                return (null, "InvalidImageData");
            }

            if (data.Length == 0 || data.Length > ProductPatternHelper.MaxImageBytes)
            {
                return (null, "ImageSizeInvalid");
            }

            list.Add(new AiImageInput(data, file.ContentType.Trim().ToLowerInvariant(), file.FileName));
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

    public static string MediaTypeFromFileName(string fileName)
        => System.IO.Path.GetExtension(fileName)?.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
}
