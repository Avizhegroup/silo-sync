using Silo.Application.Features;
using Silo.Services;

namespace Silo.Pages.Sync;

public partial class Failures
{
    public bool IsLoading = true;
    public bool IsRetrying = false;
    public List<GetOpenSyncFailuresVm> Failures { get; set; } = new();

    [Inject] public SyncAdminApiClient SyncClient { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        await LoadFailuresAsync();
        IsLoading = false;
    }

    private async Task LoadFailuresAsync()
    {
        IsLoading = true;
        Failures = await SyncClient.GetFailuresAsync("Pending");
        IsLoading = false;
    }

    private async Task OnRetryClick(GetOpenSyncFailuresVm? failure)
    {
        if (failure is null || IsRetrying)
        {
            return;
        }

        IsRetrying = true;
        var result = await SyncClient.RetryFailureAsync((int)failure.Id);
        IsRetrying = false;

        if (result.Success)
        {
            await LoadFailuresAsync();
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("alert", $"Retry failed: {result.ErrorMessage}");
            var updated = await SyncClient.GetFailuresAsync("Pending");
            Failures = updated;
        }
    }
}
