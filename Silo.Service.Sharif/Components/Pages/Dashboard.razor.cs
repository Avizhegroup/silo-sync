using Microsoft.Extensions.Options;
using Silo.Service.Sharif.Configuration;
using Silo.Service.Sharif.Services;
using Silo.Service.Sharif.State;

namespace Silo.Service.Sharif.Components.Pages;

public partial class Dashboard : IDisposable
{
    [Inject]
    private ReaderStateStore State { get; set; } = null!;

    [Inject]
    private ReaderControlService ReaderControl { get; set; } = null!;

    [Inject]
    private IOptionsMonitor<RfidWorkerOptions> Options { get; set; } = null!;

    private bool IsLoading { get; set; }

    private string StationCode => Options.CurrentValue.StationCode;

    private string GateType => Options.CurrentValue.GateType;

    protected override void OnInitialized()
    {
        State.OnChange += OnStateChanged;
    }

    private async void OnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnConnectClick()
    {
        IsLoading = true;
        await InvokeAsync(StateHasChanged);
        await ReaderControl.ConnectAsync();
        IsLoading = false;
    }

    private async Task OnDisconnectClick()
    {
        IsLoading = true;
        await InvokeAsync(StateHasChanged);
        await ReaderControl.DisconnectAsync();
        IsLoading = false;
    }

    private async Task OnStartInventoryClick()
    {
        IsLoading = true;
        await InvokeAsync(StateHasChanged);
        await ReaderControl.StartInventoryAsync();
        IsLoading = false;
    }

    private async Task OnStopInventoryClick()
    {
        IsLoading = true;
        await InvokeAsync(StateHasChanged);
        await ReaderControl.StopInventoryAsync();
        IsLoading = false;
    }

    public void Dispose()
    {
        State.OnChange -= OnStateChanged;
    }
}
