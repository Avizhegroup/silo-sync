using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace Silo.Shared.Components;

public partial class LocalWebSocketComponent : IAsyncDisposable
{
    private const string ModulePath = "./_content/Silo.Shared.Components/LocalWebSocket/localWebSocket.js";

    private const string TokenStorageKey = "Silo.LocalWebSocket.Token";

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ProtectedLocalStorage LocalStorage { get; set; } = default!;
    [Parameter] public int Port { get; set; } = 5005;

    /// <summary>Connect on first render when a token was saved by an earlier successful connection.</summary>
    [Parameter] public bool AutoConnect { get; set; } = true;

    /// <summary>Connection attempts before an error is reported (the host accepts one browser at a time).</summary>
    [Parameter] public int ConnectAttempts { get; set; } = 4;

    /// <summary>Delay between connection attempts.</summary>
    [Parameter] public int RetryDelayMilliseconds { get; set; } = 750;

    /// <summary>Raised with the text of every message received from the local device host.</summary>
    [Parameter] public EventCallback<string> OnMessageReceived { get; set; }

    /// <summary>Raised with true when connected and false when disconnected.</summary>
    [Parameter] public EventCallback<bool> OnConnectionChanged { get; set; }

    [Parameter] public EventCallback<string> OnError { get; set; }

    public bool IsConnected { get; private set; }
    public string TokenInput { get; set; } = string.Empty;
    public bool IsConnectRequested { get; private set; }
    public string Error { get; private set; } = string.Empty;

    private IJSObjectReference? _module;
    private DotNetObjectReference<LocalWebSocketComponent>? _self;
    private bool _disposed;
    private string _token = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            var stored = await LocalStorage.GetAsync<string>(TokenStorageKey);
            if (stored.Success && !string.IsNullOrWhiteSpace(stored.Value))
            {
                TokenInput = stored.Value;
                StateHasChanged();

                if (AutoConnect && !IsConnectRequested)
                {
                    await OnConnectClick();
                }
            }
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            await LocalStorage.DeleteAsync(TokenStorageKey);
        }
    }

    public async Task OnConnectClick()
    {
        _token = TokenInput.Trim();
        Error = string.Empty;
        IsConnectRequested = true;
        await ConnectAsync();
    }

    public async Task OnDisconnectClick()
    {
        IsConnectRequested = false;
        await DisconnectAsync();
        if (IsConnected)
        {
            await OnSocketStateChanged(false);
        }
    }

    public async Task ConnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _module ??= await JSRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
        _self ??= DotNetObjectReference.Create(this);
        await _module.InvokeVoidAsync("connect", $"ws://127.0.0.1:{Port}/?token={Uri.EscapeDataString(_token)}", _self,
            Math.Max(1, ConnectAttempts), Math.Max(100, RetryDelayMilliseconds));
    }

    public async Task<bool> IsConnectedAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _module is not null && await _module.InvokeAsync<bool>("isConnected");
    }

    /// <summary>Sends raw text to the device host.</summary>
    public async Task SendAsync(string data)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_module is null || !IsConnected)
        {
            throw new InvalidOperationException("The local WebSocket is not connected.");
        }
        await _module.InvokeVoidAsync("send", data);
    }

    /// <summary>Sends a JSON command such as {"type":"startReader","mode":1}.</summary>
    public Task SendCommandAsync(object command)
        => SendAsync(System.Text.Json.JsonSerializer.Serialize(command));

    public async Task DisconnectAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_module is not null)
        {
            await _module.InvokeVoidAsync("disconnect");
        }
    }

    [JSInvokable]
    public async Task OnSocketStateChanged(bool connected)
    {
        IsConnected = connected;
        if (connected && !string.IsNullOrEmpty(_token))
        {
            await LocalStorage.SetAsync(TokenStorageKey, _token);
        }
        await OnConnectionChanged.InvokeAsync(connected);
        StateHasChanged();
    }

    [JSInvokable]
    public Task OnSocketMessage(string message) => OnMessageReceived.InvokeAsync(message);

    [JSInvokable]
    public async Task OnSocketError(string message)
    {
        Error = message;
        IsConnectRequested = false;
        await OnError.InvokeAsync(message);
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("disconnect");
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }
        _self?.Dispose();
    }
}
