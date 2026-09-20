using Silo.Service.Sharif.State;

namespace Silo.Service.Sharif.Components.Pages;

public partial class LiveTags : IDisposable
{
    private const int RefreshThrottleMs = 250;

    [Inject]
    private ReaderStateStore State { get; set; } = null!;

    private TelerikGrid<TagReadEntry>? GridRef { get; set; }

    private List<TagReadEntry> Tags { get; set; } = new();

    private bool IsLoading { get; set; }

    private System.Threading.Timer? _refreshTimer;
    private bool _refreshPending;

    protected override void OnInitialized()
    {
        RefreshTags();
        State.OnChange += OnStateChanged;
    }

    private void OnStateChanged()
    {
        _refreshPending = true;
        _refreshTimer ??= new System.Threading.Timer(_ =>
        {
            InvokeAsync(() =>
            {
                if (_refreshPending)
                {
                    _refreshPending = false;
                    RefreshTags();
                    StateHasChanged();
                }
            });
        }, null, RefreshThrottleMs, RefreshThrottleMs);
    }

    private void RefreshTags()
    {
        Tags = State.Tags
            .OrderByDescending(t => t.ReadUtc)
            .ToList();
    }

    private void OnClearClick()
    {
        State.ClearTags();
    }

    private async Task OnExportExcelClick()
    {
        IsLoading = true;
        if (GridRef != null)
        {
            await GridRef.SaveAsExcelFileAsync();
        }
        IsLoading = false;
    }

    public void Dispose()
    {
        State.OnChange -= OnStateChanged;
        _refreshTimer?.Dispose();
    }
}
