using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
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
        new() { Text = TextResources.APP_StringKeys_Download, Icon = "download" },
        new() { Text = TextResources.APP_StringKeys_Delete, Icon = "delete" },
        new()
        {
            Text = "استخراج متن از تصویر",
            Icon = "ai",
            Items = new()
            {
                new() { Text = "پلاک", Icon = "plaque" },
                new() { Text = "کدملی", Icon = "nc" }
            }
        }
    };
    public long MaxAllowedSizeBytes => MaxAllowedSizeMB * 1024 * 1024;
    public string lastLoadKey;

    [Parameter] public bool Readonly { get; set; }
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public string AllowedExtensions { get; set; } = "image/png, image/jpeg";
    [Parameter] public string UserId { get; set; }
    [Parameter] public GalleryUsageType UsageType { get; set; }
    [Parameter] public string UsageId { get; set; }
    [Parameter] public GalleryOcrTypes OcrType { get; set; } = GalleryOcrTypes.None;
    [Parameter] public EventCallback<GalleryFileUploadedDto> OnCompleteUpload { get; set; }
    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }
    [Parameter] public bool UseNoUserIdApi { get; set; } = false;

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public ILogger<GalleryContent> Logger { get; set; }

   
    public FileUpload FileUploadComponent { get; set; }
    public TelerikContextMenu<TelerikContextMenuItem> ContextMenu { get; set; }




    protected override async Task OnInitializedAsync()
    {

        UserId = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = true;
    }

    protected override async Task OnParametersSetAsync()
    {

        var currentKey = $"{UserId}|{(int)UsageType}|{UsageId ?? ""}|{UseNoUserIdApi}";

        if (currentKey != lastLoadKey)
        {
            lastLoadKey = currentKey;
            await LoadGalleryMediasAsync();
        }
    }

    public async Task LoadGalleryMediasAsync()
    {
        IsLoading = true;
        StateHasChanged();


        List<GetGalleryMediasDto> result = null;

        if (UseNoUserIdApi && UsageId.HasValue())
        {
            var response = await Api.PostAsync<List<GetGalleryMediasDto>>(
                "SGetUserMediasByUsageNoUserId",
                new("usageType", UsageType),
                new("usageId", UsageId));

            result = response.Value;
        }
        else if (UsageId.HasValue())
        {
            var response = await Api.PostAsync<List<GetGalleryMediasDto>>(
                "SGetUserMediasByUsage",
                new("usageType", UsageType),
                new("usageId", UsageId),
                new("userId", UserId));

            result = response.Value;
        }
        else
        {
            var response = await Api.PostAsync<List<GetGalleryMediasDto>>(
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
        if (GalleryMedias is null) return;

        foreach (var media in GalleryMedias.Where(m => m.Extension == GalleryExtension.Image && string.IsNullOrEmpty(m.Base64Image)))
        {
            var imageFile = await Api.PostAsync("Gallery/GetGalleryImageFile", new GetGalleryImageFileQuery { Id = media.Id });

            if (imageFile is not null && imageFile.Length > 0)
            {
                media.Base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(imageFile)}";
            }
        }
    }

    public async Task OnGalleryMediaClick(GetGalleryMediasDto media)
    {
        SelectedGalleryMedia = media;
        await Download();
    }

    public async Task OnGalleryMediaRightClick(MouseEventArgs e, GetGalleryMediasDto media)
    {
        SelectedGalleryMedia = media;
        await ContextMenu.ShowAsync(e.ClientX, e.ClientY);
    }

    public async Task OnContextMenuItemClick(TelerikContextMenuItem item)
    {
        if (item.Text == TextResources.APP_StringKeys_Download)
            await Download();

        if (item.Text == TextResources.APP_StringKeys_Delete)
            await Delete();

        if (item.Icon == "plaque")
            await Ocr(GalleryOcrTypes.Plaque);

        if (item.Icon == "nc")
            await Ocr(GalleryOcrTypes.NationalCard);
    }

    public async Task OnFileUpload(IBrowserFile file)
    {
        if (file is null) return;

        var fileName = Path.GetFileNameWithoutExtension(file.Name);
        var extension = Path.GetExtension(file.Name)?.ToLower().Remove(0, 1);

        GalleryExtension? galleryExtension = extension switch
        {
            "pdf" => GalleryExtension.Pdf,
            "xlx" or "xlsx" => GalleryExtension.Excel,
            "zip" or "rar" => GalleryExtension.Zip,
            "jpg" or "png" or "jpeg" => GalleryExtension.Image,
            "doc" or "docx" => GalleryExtension.Word,
            _ => null
        };

        if (galleryExtension is null)
        {
            Notification.Show(TextResources.APP_StringKeys_File_Extention_Error, "error");
            IsLoading = false;
            return;
        }

        if (file.Size > MaxAllowedSizeBytes)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Max_Size, MaxAllowedSizeMB + "mb"), "error");
            IsLoading = false;
            return;
        }

       
            IsLoading = true;
            StateHasChanged();

            await using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: MaxAllowedSizeBytes).CopyToAsync(memoryStream);

            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(new MemoryStream(memoryStream.ToArray())), "File", file.Name);
            content.Add(new StringContent(UserId ?? ""), "UserId");
            content.Add(new StringContent(fileName), "MediaName");
            content.Add(new StringContent(galleryExtension.Value.ToString()), "Extension");
            content.Add(new StringContent(UsageId ?? ""), "UsageId");
            content.Add(new StringContent(((int)UsageType).ToString()), "UsageType");

            var media = (await Api.PostMultipartContentAsync<SaveGalleryMediaWithFileVm>(
                "Gallery/SaveGalleryMediaWithFile", content)).Value;

            if (media is not null)
            {
                var uploadMedia = Mapper.Map<GetGalleryMediasDto>(media);

                if (galleryExtension == GalleryExtension.Image)
                {
                    var fileBytes = memoryStream.ToArray();
                    uploadMedia.Base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(fileBytes)}";
                }

                GalleryMedias ??= new();
                GalleryMedias.Add(uploadMedia);
                StateHasChanged();

                var completeUpload = new GalleryFileUploadedDto
                {
                    Id = media.Id,
                    Base64Image = galleryExtension == GalleryExtension.Image
                        ? $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}"
                        : null
                };

                await OnCompleteUpload.InvokeAsync(completeUpload);
            }
        
            IsLoading = false;
            StateHasChanged();
        
    }

    private async Task Delete()
    {
        var result = (await Api.PostAsync<bool>("SRemoveGalleryMedia",
            new KeyValuePair<string, object>("mediaId", SelectedGalleryMedia.Id))).Value;

        if (result)
        {
            GalleryMedias.Remove(SelectedGalleryMedia);
            SelectedGalleryMedia = new();
            StateHasChanged();
        }
    }

    private async Task Download()
    {
        IsLoading = true;
        StateHasChanged();

       
            var imageFile = await Api.PostAsync("Gallery/GetGalleryImageFile", new GetGalleryImageFileQuery
            {
                Id = SelectedGalleryMedia.Id
            });

            if (imageFile is not null && imageFile.Length > 0)
            {
                using var stream = new MemoryStream(imageFile);
                await Export.ExportAndDownload(stream, SelectedGalleryMedia.MediaName);
            }
        
            IsLoading = false;
            StateHasChanged();
        
    }

    private async Task Ocr(GalleryOcrTypes type)
    {
        if (SelectedGalleryMedia.Extension != GalleryExtension.Image)
            return;

        try
        {
            IsLoading = true;
            StateHasChanged();

            var ocrResult = await Api.SendAsyncObjectByUri<GetOcrDataForGalleryMediaVm>(
                HttpMethod.Get,
                "Agent/GetOcrDataForGalleryMedia",
                new GetOcrDataForGalleryMediaQuery
                {
                    GalleryId = SelectedGalleryMedia.Id,
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
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    public async Task OpenFileUploader()
    {
        if (FileUploadComponent is not null)
            await FileUploadComponent.OnClickButton(new());
    }
}
