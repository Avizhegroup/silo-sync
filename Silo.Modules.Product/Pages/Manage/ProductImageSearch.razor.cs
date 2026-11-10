using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Silo.Modules.Product.Pages;

public partial class ProductImageSearch
{
    public const string AcceptedTypes = ".png,.jpg,.jpeg,.webp";
    public const long MaxFileSize = 10 * 1024 * 1024;

    public bool IsLoading;

    /// <summary>Selected files stay in the browser connection and are streamed on search; nothing is converted to text.</summary>
    public List<IBrowserFile> Images = [];

    public SearchProductsByImageVm Result;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public ILogger<ProductImageSearch> Logger { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        await Task.CompletedTask;
    }

    public void OnFilesSelected(InputFileChangeEventArgs args)
    {
        try
        {
            // GetMultipleFiles throws when more files than the limit are picked
            foreach (var file in args.GetMultipleFiles(int.MaxValue))
            {
                if (Images.Count >= ProductPatternHelper.MaxImages)
                {
                    break;
                }

                if (ContentTypeOf(file.Name) is null || file.Size == 0 || file.Size > MaxFileSize)
                {
                    Notification.Show(TextResources.APP_StringKeys_ProductPattern_UnsupportedImage, "error");

                    continue;
                }

                Images.Add(file);
            }

            Result = null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Selecting product search images failed");

            Notification.Show(TextResources.APP_StringKeys_ImageSearch_Failed, "error");
        }
    }

    public void OnRemoveFile(IBrowserFile file)
    {
        Images.Remove(file);
        Result = null;
    }

    public async Task OnSearchClick()
    {
        if (Images.Count == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_ProductPattern_NoImage, "error");

            return;
        }

        IsLoading = true;

        try
        {
            // Raw files as multipart parts; each is streamed from the browser, so the data is never buffered as base64.
            using var content = new MultipartFormDataContent();

            foreach (var file in Images)
            {
                var part = new StreamContent(file.OpenReadStream(maxAllowedSize: MaxFileSize));

                part.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentTypeOf(file.Name));

                content.Add(part, "Images", file.Name);
            }

            var result = (await Api.PostMultipartContentAsync<SearchProductsByImageVm>("Product/SearchProductsByImage", content))?.Value;

            if (result is { Succeeded: true })
            {
                Result = result;
            }
            else
            {
                Result = null;

                Notification.Show(result?.Message is "UnsupportedImageType" or "ImageSizeInvalid" or "TooManyImages"
                    ? TextResources.APP_StringKeys_ProductPattern_UnsupportedImage
                    : TextResources.APP_StringKeys_ImageSearch_Failed, "error");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Product image search failed");

            Result = null;

            Notification.Show(TextResources.APP_StringKeys_ImageSearch_Failed, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void OnResetClick()
    {
        Images = [];
        Result = null;
    }

    public static string ContentTypeOf(string fileName)
        => Path.GetExtension(fileName)?.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => null
        };
}
