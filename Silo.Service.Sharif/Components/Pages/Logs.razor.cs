using Silo.Service.Sharif.Services;

namespace Silo.Service.Sharif.Components.Pages;

public partial class Logs : IDisposable
{
    private const int AutoRefreshMs = 5000;

    [Inject]
    private LogTailService LogTail { get; set; } = null!;

    private List<LogKindItem> LogKinds { get; set; } = new()
    {
        new LogKindItem { Kind = LogKind.Info, Text = "اطلاعات" },
        new LogKindItem { Kind = LogKind.Exception, Text = "خطاها" }
    };

    private LogKind SelectedKind { get; set; } = LogKind.Info;

    private List<LogFileItem> LogFiles { get; set; } = new();

    private string SelectedFile { get; set; } = string.Empty;

    private int LineCount { get; set; } = 200;

    private string LogContent { get; set; } = string.Empty;

    private bool IsLoading { get; set; }

    private bool AutoRefresh { get; set; }

    private System.Threading.Timer? _autoRefreshTimer;

    protected override async Task OnInitializedAsync()
    {
        await LoadFileListAsync();
        await RefreshAsync();
    }

    private async Task OnLogKindChange()
    {
        await LoadFileListAsync();
        await RefreshAsync();
    }

    private async Task OnRefreshClick()
    {
        await RefreshAsync();
    }

    private async Task LoadFileListAsync()
    {
        var files = LogTail.GetLogFiles(SelectedKind);
        LogFiles = files
            .Select(f => new LogFileItem { Name = Path.GetFileName(f), Path = f })
            .ToList();

        if (LogFiles.Any() && !LogFiles.Any(f => f.Path == SelectedFile))
        {
            SelectedFile = LogFiles.First().Path;
        }
    }

    private async Task RefreshAsync()
    {
        IsLoading = true;
        LogContent = await LogTail.GetTailAsync(SelectedKind, LineCount, SelectedFile);
        IsLoading = false;
    }

    protected override void OnParametersSet()
    {
        _autoRefreshTimer ??= new System.Threading.Timer(async _ =>
        {
            await InvokeAsync(async () =>
            {
                if (AutoRefresh)
                {
                    await RefreshAsync();
                    StateHasChanged();
                }
            });
        }, null, AutoRefreshMs, AutoRefreshMs);
    }

    public void Dispose()
    {
        _autoRefreshTimer?.Dispose();
    }

    private sealed class LogKindItem
    {
        public LogKind Kind { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    private sealed class LogFileItem
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
