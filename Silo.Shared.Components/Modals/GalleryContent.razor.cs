using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Identity.Client;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class GalleryContent
{
    public bool IsLoading;
    public GetGalleryMediasDto SelectedGalleryMedia;
    public List<GetGalleryMediasDto> GalleryMedias;
    public List<TelerikContextMenuItem> ContextMenuItems { get; set; } = new()
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

    public long MaxAllowedSizeBytes => MaxAllowedSizeMB * 1024 * 1024;


    [Parameter] public bool Readonly { get; set; }
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public string UserId { get; set; }
    [Parameter] public GalleryUsageType UsageType { get; set; }
    [Parameter] public string UsageId { get; set; }
    [Parameter] public GalleryOcrTypes OcrType { get; set; } = GalleryOcrTypes.None;
    [Parameter] public bool UseNoUserIdApi { get; set; }
    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject]  public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject]  public IExport Export { get; set; }
    [Inject] public ILogger<GalleryContent> Logger { get; set; }


    public TelerikContextMenu<TelerikContextMenuItem> ContextMenu { get; set; }


    protected override async Task OnParametersSetAsync()
    {
        await LoadGalleryMediasAsync();
    }

    public async Task Refresh()
    {
        await LoadGalleryMediasAsync();
    }

    private async Task LoadGalleryMediasAsync()
    {
        IsLoading = true;

        StateHasChanged();          
      
            List<GetGalleryMediasDto> result = null;

            if (UseNoUserIdApi && UsageId.HasValue())
            {
                var response =
                    await Api.PostAsync<List<GetGalleryMediasDto>>(
                        "SGetUserMediasByUsageNoUserId",
                        new("usageType", UsageType),
                        new("usageId", UsageId));

                result = response.Value;
            }
            else if (UsageId.HasValue())
            {
                var response =
                    await Api.PostAsync<List<GetGalleryMediasDto>>(
                        "SGetUserMediasByUsage",
                        new("usageType", UsageType),
                        new("usageId", UsageId),
                        new("userId", UserId));

                result = response.Value;
            }
            else
            {
                var response =
                    await Api.PostAsync<List<GetGalleryMediasDto>>(
                        "SGetUserMediasByUserId",
                        new("usageType", UsageType),
                        new("userId", UserId));

                result = response.Value;
            }

        GalleryMedias = result ?? new List<GetGalleryMediasDto>();

            await LoadImagesBase64Async();
        
            IsLoading = false;

            StateHasChanged();
        
    }


    private async Task LoadImagesBase64Async()
    {
        if (GalleryMedias is null)
        {
            return;
        }

        foreach (var media in GalleryMedias.Where(m => m.Extension == GalleryExtension.Image && m.Base64Image.HasNoValue()))
        {
            var imageFile = await Api.PostAsync(
                "Gallery/GetGalleryImageFile",
                new GetGalleryImageFileQuery
                {
                    Id = media.Id
                });

            if (imageFile is not null && imageFile.Length > 0)
            {
                media.Base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(imageFile)}";
            }
        }
    }

    private async Task OnMediaDoubleClick(GetGalleryMediasDto media)
    {
        SelectedGalleryMedia = media;

        await Download();
    }

    private async Task OnGalleryMediaRightClick( MouseEventArgs e, GetGalleryMediasDto media)
    {
        SelectedGalleryMedia = media;

        await ContextMenu.ShowAsync(e.ClientX, e.ClientY);
    }

    private async Task OnContextMenuItemClick( TelerikContextMenuItem item)
    {
        if (SelectedGalleryMedia is null)
            return;

        if (item.Text == TextResources.APP_StringKeys_Download)
        {
            await Download();

            return;
        }

        if (item.Text == TextResources.APP_StringKeys_Delete)
        {
            await Delete();

            return;
        }

        if (item.Icon == "plaque")
        {
            await Ocr(GalleryOcrTypes.Plaque);

            return;
        }

        if (item.Icon == "nc")
        {
            await Ocr(GalleryOcrTypes.NationalCard);
        }
    }

    private async Task Download()
    {
        if (SelectedGalleryMedia is null)
        { 
        return;
        }
            IsLoading = true;

            var imageFile = await Api.PostAsync("Gallery/GetGalleryImageFile",
                new GetGalleryImageFileQuery
                {
                    Id = SelectedGalleryMedia.Id
                });

            if (imageFile is not null && imageFile.Length > 0)
            {
                using MemoryStream stream = new(imageFile);

                await Export.ExportAndDownload(stream,SelectedGalleryMedia.MediaName);
            }
        
            IsLoading = false;
    }

    private async Task Delete()
    {
        if (SelectedGalleryMedia is null)
        {
            return;
        }    

        var result = ( await Api.PostAsync<bool>("SRemoveGalleryMedia",new KeyValuePair<string, object>("mediaId",SelectedGalleryMedia.Id))).Value;

        if (result)
        {
            GalleryMedias.Remove(SelectedGalleryMedia);

            SelectedGalleryMedia = null;

            StateHasChanged();
        }
    }
    private async Task Ocr(GalleryOcrTypes type)
    {
        if (SelectedGalleryMedia is null)
        {
            return;
        }

        if (SelectedGalleryMedia.Extension != GalleryExtension.Image)
        {
            return;
        }

            IsLoading = true;

            var ocrResult = await Api.SendAsyncObjectByUri<GetOcrDataForGalleryMediaVm>(HttpMethod.Get,"Agent/GetOcrDataForGalleryMedia",
                    new GetOcrDataForGalleryMediaQuery
                    {
                        GalleryId =
                            SelectedGalleryMedia.Id,

                        OcrType = type
                    });

            if (ocrResult.Value?.Result.HasValue() == true)
            {
                await OnOcrTextExtracted.InvokeAsync(new GalleryOcrExtractedTextDto
                    {
                        ExtractedText = ocrResult.Value.Result,

                        OcrType = type,

                        MediaId = SelectedGalleryMedia.Id
                    });
            }

            IsLoading = false;
        
    }
}
