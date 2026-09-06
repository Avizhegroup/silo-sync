using System.Net.Http;
using Silo.Application.Features;
using Silo.Infrastructure.Web;

namespace Silo.Pages.Sync;

public partial class Failures
{
    public bool IsLoading = true;
    public bool IsRetrying = false;
    public List<GetOpenSyncFailuresVm> FailureList { get; set; } = new();
    public List<GetOpenSyncFailuresVm> SelectedFailures { get; set; } = new();
    public TelerikGrid<GetOpenSyncFailuresVm> FailureGrid { get; set; } = null!;
    public string SelectedStatus { get; set; } = "Pending";
    public List<string> StatusOptions { get; set; } = new() { "Pending", "Resolved", "All" };

    [Inject] public RfidConnectApi Api { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        await LoadFailuresAsync();
        IsLoading = false;
    }

    private async Task LoadFailuresAsync()
    {
        IsLoading = true;
        var status = SelectedStatus == "All" ? null : SelectedStatus;
        var response = await Api.SendAsyncObjectByUri<ApiResponse<List<GetOpenSyncFailuresVm>>>(HttpMethod.Get
            , $"SyncAdmin/failures?status={Uri.EscapeDataString(status ?? "Pending")}");
        FailureList = response?.Value ?? new List<GetOpenSyncFailuresVm>();
        SelectedFailures = new List<GetOpenSyncFailuresVm>();
        IsLoading = false;
    }

    private async Task OnFilterClick()
    {
        await LoadFailuresAsync();
    }

    private async Task OnRetryClick(GetOpenSyncFailuresVm? failure)
    {
        if (failure is null || IsRetrying)
        {
            return;
        }

        IsRetrying = true;
        var result = await Api.SendAsyncObjectByUri<ApiResponse<RetrySyncRowFailureVm>>(HttpMethod.Post
            , $"SyncAdmin/failures/{failure.Id}/retry");
        IsRetrying = false;

        if (result?.Value?.Success == true)
        {
            await LoadFailuresAsync();
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("alert", $"Retry failed: {result?.Value?.ErrorMessage ?? result?.Message}");
            await LoadFailuresAsync();
        }
    }

    private async Task OnBulkRetryClick()
    {
        if (SelectedFailures.Count == 0 || IsRetrying)
        {
            return;
        }

        IsRetrying = true;
        foreach (var failure in SelectedFailures)
        {
            await Api.SendAsyncObjectByUri<ApiResponse<RetrySyncRowFailureVm>>(HttpMethod.Post
                , $"SyncAdmin/failures/{failure.Id}/retry");
        }
        IsRetrying = false;

        await LoadFailuresAsync();
    }

    private void OnSelectedFailuresChanged(IEnumerable<GetOpenSyncFailuresVm> items)
    {
        SelectedFailures = items.ToList();
    }
}
