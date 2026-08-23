using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Print;
public partial class SelectPrintFormat
{
    public List<GetPrintFormatsByPageTitleDto> PrintFormats;

    [Parameter] public EventCallback<GetPrintFormatsByPageTitleDto> OnFileNameSelect { get; set; }

    [CascadingParameter] public bool IsLoading { get; set; } = true;
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    public Modal Modal { get; set; }
    public TelerikGrid<GetPrintFormatsByPageTitleDto> FileNameGridRef { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadPrintFormatsAsync();
    }

    private async Task LoadPrintFormatsAsync()
    {
        string currentPath = $"-{NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Replace("/", "-")}";

        PrintFormats = (await Api.SendAsyncObjectByUri<GetPrintFormatsByPageTitleVm>(HttpMethod.Get
            , $"PrintFormat/GetPrintFormatsByPageTitle?pageTitle={currentPath}")).Value?.List;

        IsLoading = false;
    }

    public async Task ShowPrintFormatsAsync(MouseEventArgs e)
    {
        if (PrintFormats?.Count == 1)
        {
            await OnFileNameSelect.InvokeAsync(PrintFormats.First());
        }
        else if (PrintFormats?.Count > 1)
        {
            await Modal.Open(new());
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Print_Format_NotFound, "error");
        }
    }

    public async Task OnSelectClick(GetPrintFormatsByPageTitleDto format)
    {
        await OnFileNameSelect.InvokeAsync(format);

        await Modal.Close(new());
    }
}
