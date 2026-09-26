using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;

public partial class GalleryContent
{
    [Parameter] public bool Readonly { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public List<GetGalleryMediasDto> GalleryMedias { get; set; }
    [Parameter] public GetGalleryMediasDto SelectedGalleryMedia { get; set; }
    [Parameter] public long MaxAllowedSizeMB { get; set; }
    [Parameter] public string AllowedExtensions { get; set; }

    [Parameter] public EventCallback OnStartUpload { get; set; }
    [Parameter] public EventCallback<IBrowserFile> OnFileUpload { get; set; }
    [Parameter] public EventCallback<GetGalleryMediasDto> OnClick { get; set; }
    [Parameter] public EventCallback<(MouseEventArgs Args, GetGalleryMediasDto Media)> OnRightClick { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    public FileUpload FileUploadComponent { get; set; }
}
