namespace Silo.Modules.Product.Pages;

public partial class ProductImageSearch
{
    public static readonly List<string> AllowedExtensions = [".png", ".jpg", ".jpeg", ".webp"];
    public const int MaxFileSize = 10 * 1024 * 1024;

    public bool IsLoading;
    public List<ProductImageFileDto> Images = [];
    public SearchProductsByImageVm Result;

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        await Task.CompletedTask;
    }

    public async Task OnFilesSelected(FileSelectEventArgs args)
    {
        try
        {
            foreach (var file in args.Files)
            {
                if (Images.Count >= ProductPatternHelper.MaxImages)
                {
                    break;
                }

                var contentType = ContentTypeOf(file.Extension);

                if (contentType is null || file.Size > MaxFileSize)
                {
                    Notification.Show(TextResources.APP_StringKeys_ProductPattern_UnsupportedImage, "error");

                    continue;
                }

                using var ms = new MemoryStream();

                await file.Stream.CopyToAsync(ms);

                Images.Add(new ProductImageFileDto
                {
                    FileName = file.Name,
                    ContentType = contentType,
                    Base64 = Convert.ToBase64String(ms.ToArray())
                });
            }

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void OnFileRemoved(FileSelectEventArgs args)
    {
        foreach (var file in args.Files)
        {
            Images.RemoveAll(i => i.FileName == file.Name);
        }
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
            var result = (await Api.SendAsyncObjectByUri<SearchProductsByImageVm>(HttpMethod.Post
                , "Product/SearchProductsByImage"
                , new SearchProductsByImageQuery { Images = Images }))?.Value;

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
        catch (Exception)
        {
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

    public static string ContentTypeOf(string extension)
        => extension?.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => null
        };
}
