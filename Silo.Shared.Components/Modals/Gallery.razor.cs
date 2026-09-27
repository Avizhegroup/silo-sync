using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Identity.Client;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class Gallery
{
    public bool IsLoading { get; set; }
    public string UserId { get; set; }
    public GalleryUsageType UsageType { get; set; }
    public string UsageId { get; set; }
    public GalleryOcrTypes OcrType { get; set; } = GalleryOcrTypes.None;
    public bool UseNoUserIdApi { get; set; }
    public long MaxAllowedSizeBytes =>  MaxAllowedSizeMB * 1024 * 1024;


    [Parameter] public bool Readonly { get; set; }
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public string AllowedExtensions { get; set; } = "image/png, image/jpeg";
    [Parameter] public EventCallback<GalleryFileUploadedDto> OnCompleteUpload { get; set; }

    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject]  public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }
    [Inject]  public IExport Export { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }


    public GalleryContent GalleryContentRef { get; set; }
    public FileUpload FileUploadComponent { get; set; }
    public Modal Modal { get; set; }


    protected override async Task OnInitializedAsync()
    {
        UserId = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = true;
    }


    public async Task Show(GalleryUsageType usageType,string usageId)
    {
        UsageType = usageType;

        UsageId = usageId;

        UserId = null;

        OcrType = GalleryOcrTypes.None;

        UseNoUserIdApi = false;

        await Modal.Open(new());      

    }


    public async Task Show(string userId,GalleryUsageType usageType)
    {
        UserId = userId;

        UsageType = usageType;

        UsageId = null;

        OcrType = GalleryOcrTypes.None;

        UseNoUserIdApi = false;

        await Modal.Open(new());
    
    }


    public async Task Show( string userId, GalleryUsageType usageType, string usageId, GalleryOcrTypes ocrType = GalleryOcrTypes.None)
    {
        UserId = userId;

        UsageType = usageType;

        UsageId = usageId;

        OcrType = ocrType;

        UseNoUserIdApi = true;

        await Modal.Open(new());
              
    }

    public async Task ShowFileUploader( GalleryUsageType usageType, string usageId)
    {
        UsageType = usageType;

        UsageId = usageId;

        StateHasChanged();
        
        await FileUploadComponent.OnClickButton(new());
        
    }


    public async Task OnFileUpload(IBrowserFile file)
    {
        if (file is null)
        {
            return;
        }

        var fileName = Path.GetFileNameWithoutExtension(file.Name);

        var extension = Path.GetExtension(file.Name)? .ToLower().Remove(0, 1);

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
            Notification.Show( TextResources.APP_StringKeys_File_Extention_Error, "error");

            IsLoading = false;

            return;
        }

        if (file.Size > MaxAllowedSizeBytes)
        {
            Notification.Show( string.Format(TextResources.APP_StringKeys_Validation_Max_Size, MaxAllowedSizeMB + "mb"),"error");

            IsLoading = false;

            return;
        }

            IsLoading = true;

            StateHasChanged();

            await using var memoryStream = new MemoryStream();

            await file.OpenReadStream( maxAllowedSize: MaxAllowedSizeBytes).CopyToAsync(memoryStream);

            using var content = new MultipartFormDataContent();

            content.Add(new StreamContent(new MemoryStream(memoryStream.ToArray())), "File", file.Name);

            content.Add(new StringContent(UserId ?? ""), "UserId");

            content.Add( new StringContent(fileName),"MediaName");

            content.Add( new StringContent(galleryExtension.Value.ToString()),"Extension");

            content.Add(new StringContent(UsageId ?? ""),"UsageId");

            content.Add( new StringContent(((int)UsageType).ToString()),"UsageType");


            var media = (await Api.PostMultipartContentAsync< SaveGalleryMediaWithFileVm>( "Gallery/SaveGalleryMediaWithFile", content)).Value;


            if (media is not null)
            {
                var completeUpload =
                    new GalleryFileUploadedDto
                    {
                        Id = media.Id,

                        Base64Image =
                            galleryExtension ==
                            GalleryExtension.Image
                                ? $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}"
                                : null
                    };

                await OnCompleteUpload.InvokeAsync(completeUpload);

                await GalleryContentRef.Refresh();

                StateHasChanged();
            }
      
            IsLoading = false;

            StateHasChanged();
        
    }
}
