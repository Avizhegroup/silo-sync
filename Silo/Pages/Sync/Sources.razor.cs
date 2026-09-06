using Silo.Application.Features;
using Silo.Services;

namespace Silo.Pages.Sync;

public partial class Sources
{
    public bool IsLoading = true;
    public List<GetSyncSourcesVm> SourceList { get; set; } = new();

    [Inject] public SyncAdminApiClient SyncClient { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        await LoadSourcesAsync();
        IsLoading = false;
    }

    private async Task LoadSourcesAsync()
    {
        IsLoading = true;
        SourceList = await SyncClient.GetSourcesAsync();
        IsLoading = false;
    }

    private async Task OnAddClick()
    {
        // Placeholder for add modal; minimal implementation for build
        await JsRuntime.InvokeVoidAsync("alert", "Add source dialog not yet implemented.");
    }

    private async Task OnEditClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsync("alert", "Edit source dialog not yet implemented.");
    }

    private async Task OnDeleteClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        await SyncClient.DeleteSourceAsync(source.Id);
        await LoadSourcesAsync();
    }

    private async Task OnTestClick(GetSyncSourcesVm? source)
    {
        if (source is null)
        {
            return;
        }

        IsLoading = true;
        var result = await SyncClient.TestSourceAsync(source.Id);
        IsLoading = false;

        var message = result.Success
            ? $"Test succeeded. Columns: {string.Join(", ", result.Columns)}"
            : $"Test failed: {result.ErrorMessage}";
        await JsRuntime.InvokeVoidAsync("alert", message);
    }
}
