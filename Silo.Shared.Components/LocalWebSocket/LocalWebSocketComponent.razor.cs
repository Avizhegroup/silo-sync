using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Silo.Shared.Components;

public partial class LocalWebSocketComponent : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Parameter] public int Port { get; set; } = 5005;
    [Parameter, EditorRequired] public string SessionToken { get; set; } = string.Empty;

    private IJSObjectReference? _module;
    private bool _disposed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Silo.Shared.Components/LocalWebSocket/localWebSocket.js");
        await _module.InvokeVoidAsync("connect", $"ws://127.0.0.1:{Port}/?token={Uri.EscapeDataString(SessionToken)}");
    }

    public async Task ConnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Silo.Shared.Components/LocalWebSocket/localWebSocket.js");
        await _module.InvokeVoidAsync("connect", $"ws://127.0.0.1:{Port}/?token={Uri.EscapeDataString(SessionToken)}");
    }

    public async Task<bool> IsConnectedAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_module is null)
        {
            return false;
        }
        return await _module.InvokeAsync<bool>("isConnected");
    }

    public async Task DisconnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("disconnect");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_module is null)
        {
            return;
        }

        try
        {
            await _module.InvokeVoidAsync("disconnect");
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
