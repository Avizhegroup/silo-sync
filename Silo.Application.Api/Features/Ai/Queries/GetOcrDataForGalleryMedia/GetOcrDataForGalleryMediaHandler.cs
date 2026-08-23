using Microsoft.Extensions.Logging;
using Silo.Application.Api.Contracts;

namespace Silo.Application.Api.Features;

public class GetOcrDataForGalleryMediaHandler(WmsApiContext context
    , IAiApiClient aiApiClient
    , ILogger<GetOcrDataForGalleryMediaHandler> logger) : IRequestHandler<GetOcrDataForGalleryMediaQuery, GetOcrDataForGalleryMediaVm>
{
    public async Task<GetOcrDataForGalleryMediaVm> Handle(GetOcrDataForGalleryMediaQuery request, CancellationToken cancellationToken)
    {
        var gallery = await context.GalleryMedias.FirstOrDefaultAsync(gm => gm.Id == request.GalleryId, cancellationToken);

        if (gallery is null)
        {
            return new()
            {
                Result = string.Empty
            };
        }

        if (gallery.Data.HasValue())
        {
            using JsonDocument doc = JsonDocument.Parse(gallery.Data);

            if (doc.RootElement.TryGetProperty(request.OcrType.ToString(), out JsonElement ocrElement))
            {
                string ocrResult = ocrElement.GetString();

                return new()
                {
                    Result = ocrResult
                };
            }

            return new()
            {
                Result = string.Empty
            };
        }

        try
        {
            var fileBytes = await File.ReadAllBytesAsync(gallery.MediaPath);

            if (fileBytes is null || fileBytes.Length == 0)
            {
                return new()
                {
                    Result = string.Empty
                }; ;
            }

            var extension = System.IO.Path.GetExtension(gallery.MediaName)?.ToLower();

            var mediaType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "image/jpeg"
            };

            var prompt = "";

            if (request.OcrType == GalleryOcrTypes.Plaque)
            {
                prompt = "plaque";
            }

            if (request.OcrType == GalleryOcrTypes.NationalCard)
            {
                prompt = "nationalcard";
            }

            var extractedText = await aiApiClient.SendImageAsync(fileBytes, mediaType, prompt, cancellationToken);

            await UpdateGallery(extractedText);

            return new()
            {
                Result = extractedText
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, ex.Message);

            return new()
            {
                Result = string.Empty
            };
        }

        async Task UpdateGallery(string extractedText)
        {
            using JsonDocument doc = JsonDocument.Parse(gallery.Data ?? "{}");

            var dict = new Dictionary<string, object>();

            foreach (JsonProperty property in doc.RootElement.EnumerateObject())
            {
                dict[property.Name] = property.Value.GetString();
            }

            dict[request.OcrType.ToString()] = extractedText;

            string updatedJson = JsonSerializer.Serialize(dict);

            gallery.Data = updatedJson;

            context.GalleryMedias.Update(gallery);

            await context.SaveChangesAsync();
        }
    }
}
