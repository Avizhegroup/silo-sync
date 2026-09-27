using Microsoft.AspNetCore.Components;
using Silo.Application.Dto;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class Gallery
{
    [Parameter] public bool Readonly { get; set; } = false;
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public string AllowedExtensions { get; set; } = "image/png, image/jpeg";
    [Parameter] public EventCallback<GalleryFileUploadedDto> OnCompleteUpload { get; set; }
    [Parameter] public EventCallback<GalleryOcrExtractedTextDto> OnOcrTextExtracted { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    public GalleryContent GalleryContentComponent { get; set; }
    public Modal Modal { get; set; }

    private string UserId;
    private GalleryUsageType UsageType;
    private string UsageId;
    private GalleryOcrTypes OcrType = GalleryOcrTypes.None;
    private bool UseNoUserIdApi;   

    public async Task Show(GalleryUsageType usageType, string usageId)
    {
        UsageType = usageType;
        UsageId = usageId;
        UserId = null;
        OcrType = GalleryOcrTypes.None;
        UseNoUserIdApi = false;

        await OpenModal();
    }

    public async Task Show(string userId, GalleryUsageType usageType)
    {
        UserId = userId;
        UsageType = usageType;
        UsageId = null;
        OcrType = GalleryOcrTypes.None;
        UseNoUserIdApi = false;

        await OpenModal();
    }

    public async Task Show(string userId, GalleryUsageType usageType, string usageId, GalleryOcrTypes ocrType = GalleryOcrTypes.None)
    {
        UserId = userId;               
        UsageType = usageType;
        UsageId = usageId;
        OcrType = ocrType;
        UseNoUserIdApi = true;        

        await OpenModal();
    }

    private async Task OpenModal()
    {
        StateHasChanged();
        await Task.Delay(30);           
        await Modal.Open(new());
    }

    public async Task ShowFileUploader(GalleryUsageType usageType, string usageId)
    {
        UsageType = usageType;
        UsageId = usageId;
        StateHasChanged();

        if (GalleryContentComponent is not null)
            await GalleryContentComponent.OpenFileUploader();
    }
}
