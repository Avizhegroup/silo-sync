using System.Security.Cryptography;

namespace Silo.Components;
public partial class AppErrorBoundary
{
    public bool ShowException = false;

    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ILogger<AppErrorBoundary> Logger { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

#if DEBUG
    protected override async Task OnInitializedAsync()
    {
        ShowException = true;
        await base.OnInitializedAsync();
    }
#endif

    protected override async Task OnErrorAsync(Exception ex)
    {
#if DEBUG
        System.Diagnostics.Debugger.Break();
#endif
        Logger.LogWarning(ex, ex.Message);

        if (ex is CryptographicException)
        {
            await JSRuntime.InvokeVoidAsync("localStorage.clear");
            NavigationManager.NavigateTo("/account/login", true);
        }
    }

    public void GoHome(MouseEventArgs e)
    {
        NavigationManager.NavigateTo("/", true);
    }
}
