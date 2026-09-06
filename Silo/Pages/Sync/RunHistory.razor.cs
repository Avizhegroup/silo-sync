using System.Net.Http;
using Silo.Application.Features;
using Silo.Infrastructure.Web;

namespace Silo.Pages.Sync;

public partial class RunHistory
{
    public bool IsLoading = true;
    public List<GetSyncRunHistoryVm> RunList { get; set; } = new();
    public GetSyncRunHistoryQuery Filter { get; set; } = new();

    [Inject] public RfidConnectApi Api { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        await LoadRunsAsync();
        IsLoading = false;
    }

    private async Task LoadRunsAsync()
    {
        IsLoading = true;

        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(Filter.SourceKey))
        {
            query.Add($"sourceKey={Uri.EscapeDataString(Filter.SourceKey)}");
        }
        if (Filter.From.HasValue)
        {
            query.Add($"from={Uri.EscapeDataString(Filter.From.Value.ToString("O"))}");
        }
        if (Filter.To.HasValue)
        {
            query.Add($"to={Uri.EscapeDataString(Filter.To.Value.ToString("O"))}");
        }

        var uri = "SyncAdmin/runs";
        if (query.Count > 0)
        {
            uri += "?" + string.Join("&", query);
        }

        var response = await Api.SendAsyncObjectByUri<List<GetSyncRunHistoryVm>>(HttpMethod.Get, uri);
        RunList = response?.Value ?? new List<GetSyncRunHistoryVm>();
        IsLoading = false;
    }

    private async Task OnFilterClick()
    {
        await LoadRunsAsync();
    }

    private async Task OnClearClick()
    {
        Filter = new GetSyncRunHistoryQuery();
        await LoadRunsAsync();
    }
}
