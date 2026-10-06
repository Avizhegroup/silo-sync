using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Silo.Application;
using Silo.Application.Features;
using Silo.Identity.Client;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;
public partial class Gallery
{
    public bool IsLoading = false;
    public string UserId;
    public GalleryUsageType UsageType;
    public string UsageId;
    public GetGalleryMediasDto SelectedGalleryMedia = new();
    public List<GetGalleryMediasDto> GalleryMedias;
    public GalleryOcrTypes OcrType = GalleryOcrTypes.None;
    public long MaxAllowedSizeBytes => MaxAllowedSizeMB * 1024 * 1024;

    [Parameter] public bool Readonly { get; set; } = false;
    [Parameter] public EventCallback<GalleryFileUploadedDto> OnCompleteUpload { get; set; }
    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public string AllowedExtensions { get; set; } = "image/png, image/jpeg";

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public ILogger<Gallery> Logger { get; set; }

    public Modal Modal { get; set; }
    public FileUpload FileUploadComponent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        UserId = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId();
    }

    public async Task OnFileUpload(IBrowserFile file)
    {
        if (file is not null)
        {
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

            if (galleryExtension is not null && file.Size <= MaxAllowedSizeBytes)
            {
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
                    "Gallery/SaveGalleryMediaWithFile",
                    content
                )).Value;

                if (media is not null)
                {
                    GetGalleryMediasDto uploadMedia = Mapper.Map<GetGalleryMediasDto>(media);

                    if (GalleryMedias is not null)
                    {
                        GalleryMedias.Add(uploadMedia);

                        StateHasChanged();
                    }

                    GalleryFileUploadedDto completeUpload = new()
                    {
                        Id = media.Id,
                        Base64Image = null
                    };

                    if (galleryExtension.Equals(GalleryExtension.Image))
                    {
                        var fileBytes = memoryStream.ToArray();
                        completeUpload.Base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(fileBytes)}";
                    }

                    await OnCompleteUpload.InvokeAsync(completeUpload);
                }
            }
            else
            {
                if (galleryExtension is null)
                {
                    Notification.Show(TextResources.APP_StringKeys_File_Extention_Error, "error");
                }

                if (file.Size > MaxAllowedSizeBytes)
                {
                    Notification.Show(
                        string.Format(TextResources.APP_StringKeys_Validation_Max_Size, MaxAllowedSizeMB + "mb")
                        , "error");
                }
            }
        }

        IsLoading = false;
    }

    /// <summary>
    /// Upload a single file, without opens gallery modal 
    /// </summary>
    public async Task ShowFileUploader(GalleryUsageType usageType, string usageId)
    {
        UsageType = usageType;

        UsageId = usageId;

        await FileUploadComponent.OnClickButton(new());
    }

    #region Show gallery modal methods
    /// <summary>
    /// Shows gallery modal first, then user can open uploader or etc
    /// </summary>
    public async Task Show(GalleryUsageType usageType, string usageId)
    {
        IsLoading = true;

        UsageType = usageType;

        UsageId = usageId;

        GalleryMedias = (await Api.PostAsync<List<GetGalleryMediasDto>>("SGetUserMediasByUsage"
            , new("usageType", UsageType)
            , new("usageId", UsageId)
            , new("userId", UserId))).Value;

        IsLoading = false;

        await Modal.Open(new());
    }

    /// <summary>
    /// Shows gallery modal first, then user can open uploader or etc
    /// </summary>
    public async Task Show(string userId
        , GalleryUsageType usageType)
    {
        IsLoading = true;

        UsageType = usageType;

        UsageId = null;

        UserId = userId;

        GalleryMedias = (await Api.PostAsync<List<GetGalleryMediasDto>>("SGetUserMediasByUserId"
            , new("usageType", UsageType)
            , new("userId", UserId))).Value;

        IsLoading = false;

        await Modal.Open(new());
    }

    /// <summary>
    /// Shows gallery modal first, then user can open uploader or etc
    /// </summary>
    public async Task Show(string userId
        , GalleryUsageType usageType
        , string usageId
        , GalleryOcrTypes ocrType = GalleryOcrTypes.None)
    {
        IsLoading = true;

        UsageType = usageType;

        UsageId = usageId;

        UserId = userId;

        OcrType = ocrType;

        GalleryMedias = (await Api.PostAsync<List<GetGalleryMediasDto>>("SGetUserMediasByUsageNoUserId"
            , new("usageType", UsageType)
            , new("usageId", UsageId))).Value;

        IsLoading = false;

        await Modal.Open(new());
    }
    #endregion
}
