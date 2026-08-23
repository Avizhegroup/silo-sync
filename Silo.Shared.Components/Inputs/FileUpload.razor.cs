using Microsoft.AspNetCore.Components.Forms;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class FileUpload
{
    public string Id = Guid.NewGuid().ToString();
    public long MaxAllowedSizeBytes => MaxAllowedSizeMB * 1024 * 1024;

    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public string ButtonTitle { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public string ButtonClass { get; set; }
    [Parameter] public string MaterialIconClass { get; set; }
    [Parameter] public string ButtonImageUrl { get; set; }
    [Parameter] public string AllowedExtensions { get; set; }
    [Parameter] public string UploadUrl { get; set; }
    [Parameter] public bool ShowFileName { get; set; } = false;
    [Parameter] public long MaxAllowedSizeMB { get; set; } = 20;
    [Parameter] public bool ShowDropZone { get; set; } = false;
    [Parameter] public string DropZoneText { get; set; } = "";

    [Parameter] public EventCallback<string> OnCompleteUpload { get; set; }
    [Parameter] public EventCallback OnStartUpload { get; set; }
    [Parameter] public EventCallback OnFailUpload { get; set; }
    [Parameter] public EventCallback<IBrowserFile> OnUpload { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var objectRf = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("onUploaderInit", objectRf, Id, UploadUrl);
        }
    }

    public async Task OnClickButton(MouseEventArgs e)
    {
        await JSRuntime.InvokeVoidAsync("onUploaderClick", Id);
    }

    [JSInvokable]
    public async Task InformUploadPath(string path)
    {
        await OnCompleteUpload.InvokeAsync(path);
    }

    [JSInvokable]
    public async Task InformStartUpload()
    {
        await OnStartUpload.InvokeAsync();
    }

    [JSInvokable]
    public async Task InformFailUpload()
    {
        await OnFailUpload.InvokeAsync();
    }

    public async Task OnUploadFileExcel(InputFileChangeEventArgs e)
    {
        if (e.File.Size > MaxAllowedSizeBytes)
        {
            Notification.Show(
                string.Format(TextResources.APP_StringKeys_Validation_Max_Size, MaxAllowedSizeMB + "mb")
                , "error");


            await OnUpload.InvokeAsync(null);

            return;
        }

        MemoryStream stream = new();

        await e.File.OpenReadStream(maxAllowedSize: MaxAllowedSizeBytes).CopyToAsync(stream);

        await OnUpload.InvokeAsync(e.File);
    }
}
