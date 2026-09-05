using Silo.Application.Features;
using Silo.Services;

namespace Silo.Pages.Sync;

public partial class RunHistory
{
    public bool IsLoading = true;
    public List<GetSyncRunHistoryVm> Runs { get; set; } = new();

    [Inject] public SyncAdminApiClient SyncClient { get; set; } = null!;

    protected override async Task SiloInitializer()
    {
        IsLoading = true;
        Runs = await SyncClient.GetRunHistoryAsync(new GetSyncRunHistoryQuery());
        IsLoading = false;
    }
}
