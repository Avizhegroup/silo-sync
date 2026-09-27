using Microsoft.Extensions.Logging;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class GalleryGrid
{
    public GetGalleryMediasDto SelectedGalleryMedia = new();

    public List<TelerikContextMenuItem> ContextMenuItems = new()
    {
        new()
        {
            Text = TextResources.APP_StringKeys_Download,
            Icon = "download"
        },
        new()
        {
            Text = TextResources.APP_StringKeys_Delete,
            Icon = "delete"
        },
        new()
        {
            Text = "استخراج متن از تصویر",
            Icon = "ai",
            Items = new()
            {
                new()
                {
                    Text = "پلاک",
                    Icon = "plaque"
                },
                new()
                {
                    Text = "کدملی",
                    Icon = "nc"
                }
            }
        }
    };

    [Parameter] public List<GetGalleryMediasDto> GalleryMedias { get; set; }
    [Parameter] public bool Readonly { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<bool> IsLoadingChanged { get; set; }
    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }
    [Parameter] public EventCallback OnRequestCloseModal { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public ILogger<GalleryGrid> Logger { get; set; }

    public TelerikContextMenu<TelerikContextMenuItem> ContextMenu { get; set; }

    public Dictionary<int, string> ImagePreviews { get; } = new();

    private readonly HashSet<int> loadingPreviewIds = new();

    protected override async Task OnParametersSetAsync()
    {
        await LoadImagePreviews();
    }

    private async Task LoadImagePreviews()
    {
        if (GalleryMedias is null)
        {
            return;
        }

        var mediasToLoad = GalleryMedias
            .Where(m => m.Extension == GalleryExtension.Image
                && !ImagePreviews.ContainsKey(m.Id)
                && !loadingPreviewIds.Contains(m.Id))
            .ToList();

        foreach (var media in mediasToLoad)
        {
            loadingPreviewIds.Add(media.Id);

            var imageFile = await Api.PostAsync("Gallery/GetGalleryImageFile", new GetGalleryImageFileQuery()
            {
                Id = media.Id
            });

            if (imageFile is not null && imageFile.Length > 0)
            {
                ImagePreviews[media.Id] = $"data:image/jpeg;base64,{Convert.ToBase64String(imageFile)}";

                StateHasChanged();
            }

            loadingPreviewIds.Remove(media.Id);
        }
    }

    private async Task OnItemClick(MouseEventArgs e, GetGalleryMediasDto media)
    {
        SelectedGalleryMedia = media;

        await ContextMenu.ShowAsync(e.ClientX, e.ClientY);
    }

    private async Task OnContextMenuItemClick(TelerikContextMenuItem item)
    {
        if (item.Text == TextResources.APP_StringKeys_Download)
        {
            await Download();
        }

        if (item.Text == TextResources.APP_StringKeys_Delete)
        {
            await Delete();
        }

        if (item.Icon == "plaque")
        {
            await Ocr(GalleryOcrTypes.Plaque);
        }

        if (item.Icon == "nc")
        {
            await Ocr(GalleryOcrTypes.NationalCard);
        }
    }

    private async Task SetIsLoading(bool value)
    {
        IsLoading = value;

        await IsLoadingChanged.InvokeAsync(value);
    }

    private async Task Delete()
    {
        bool result = (await Api.PostAsync<bool>("SRemoveGalleryMedia"
         , new KeyValuePair<string, object>("mediaId", SelectedGalleryMedia.Id))).Value;

        if (result)
        {
            GalleryMedias.Remove(SelectedGalleryMedia);

            ImagePreviews.Remove(SelectedGalleryMedia.Id);

            SelectedGalleryMedia = new();

            StateHasChanged();
        }
    }

    private async Task Download()
    {
        await SetIsLoading(true);

        var imageFile = await Api.PostAsync("Gallery/GetGalleryImageFile", new GetGalleryImageFileQuery()
        {
            Id = SelectedGalleryMedia.Id
        });

        await SetIsLoading(false);

        if (imageFile is not null && imageFile.Length > 0)
        {
            byte[] data = imageFile;

            using MemoryStream stream = new(data);

            await Export.ExportAndDownload(stream, SelectedGalleryMedia.MediaName);
        }
    }

    private async Task Ocr(GalleryOcrTypes type)
    {
        if (SelectedGalleryMedia.Extension != GalleryExtension.Image)
        {
            return;
        }

        try
        {
            await SetIsLoading(true);

            StateHasChanged();

            var ocrResult = await Api.SendAsyncObjectByUri<GetOcrDataForGalleryMediaVm>(HttpMethod.Get
                , "Agent/GetOcrDataForGalleryMedia"
                , new GetOcrDataForGalleryMediaQuery()
            {
                GalleryId = SelectedGalleryMedia.Id,
                OcrType = type
            });

            await SetIsLoading(false);

            await OnRequestCloseModal.InvokeAsync();

            StateHasChanged();

            if (ocrResult.Value.Result.HasValue())
            {
                await OnOcrTextExtracted.InvokeAsync(new GalleryOcrExtractedTextDto
                {
                    ExtractedText = ocrResult.Value.Result,
                    OcrType = type,
                    MediaId = SelectedGalleryMedia.Id
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }
        finally
        {
            await SetIsLoading(false);
            StateHasChanged();
        }
    }
}
