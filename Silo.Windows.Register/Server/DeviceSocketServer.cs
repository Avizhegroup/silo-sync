using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Silo.Windows.Register.Config;

namespace Silo.Windows.Register.Server;

public sealed record SocketCommand(string Type, int? Mode, string? Printer, string? Data);

/// <summary>
/// Loopback WebSocket server (ws://127.0.0.1:{port}/?token=...). One browser client; HTTPS origin allowlist.
/// Outgoing: {"type":"epc|barcode|status|pong","value":"..."}.
/// Incoming: {"type":"startReader","mode":1|2}, {"type":"stopReader"}, {"type":"print","printer":"","data":"ZPL"}, {"type":"ping"}.
/// </summary>
public sealed class DeviceSocketServer(RegisterSettings settings) : IAsyncDisposable
{
    private const int MaximumMessageBytes = 64 * 1024;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private HttpListener? _listener;
    private CancellationTokenSource? _shutdown;
    private Task? _loop;
    private WebSocket? _client;

    public event EventHandler<SocketCommand>? CommandReceived;
    public event EventHandler<string>? Log;
    public event EventHandler<bool>? ClientConnectionChanged;

    public bool IsRunning => _loop is not null;

    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.SocketToken))
        {
            throw new InvalidOperationException("A socket token must be configured.");
        }
        if (settings.AllowedOrigins.Count == 0 || settings.AllowedOrigins.Any(o =>
                !Uri.TryCreate(o, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("Configure at least one HTTPS allowed origin (and only HTTPS origins).");
        }

        var listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{settings.SocketPort}/");
        try
        {
            listener.Start();
        }
        catch (HttpListenerException exception)
        {
            throw new InvalidOperationException($"Cannot listen on 127.0.0.1:{settings.SocketPort}. Is the port in use?", exception);
        }

        _listener = listener;
        _shutdown = new CancellationTokenSource();
        _loop = AcceptAsync(listener, _shutdown.Token);
        Log?.Invoke(this, $"Server listening on 127.0.0.1:{settings.SocketPort}");
    }

    public async Task StopAsync()
    {
        if (_loop is null)
        {
            return;
        }

        _shutdown?.Cancel();
        _listener?.Stop();
        _client?.Abort();
        try
        {
            await _loop;
        }
        catch (Exception exception) when (exception is OperationCanceledException or HttpListenerException or ObjectDisposedException)
        {
        }
        _listener?.Close();
        _shutdown?.Dispose();
        _listener = null;
        _shutdown = null;
        _loop = null;
        Log?.Invoke(this, "Server stopped");
    }

    public Task SendAsync(string type, string value, CancellationToken cancellationToken = default)
        => SendRawAsync(JsonSerializer.Serialize(new { type, value }), cancellationToken);

    public async Task SendRawAsync(string json, CancellationToken cancellationToken = default)
    {
        var client = _client;
        if (client?.State != WebSocketState.Open)
        {
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(json);
        if (bytes.Length > MaximumMessageBytes)
        {
            throw new ArgumentException("Message too large.", nameof(json));
        }

        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            await client.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    private async Task AcceptAsync(HttpListener listener, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync().WaitAsync(token);
                if (!IsAuthorized(context))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.Close();
                    Log?.Invoke(this, "Rejected a connection (origin/token)");
                    continue;
                }
                if (_client?.State == WebSocketState.Open)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    context.Response.Close();
                    continue;
                }

                var accepted = await context.AcceptWebSocketAsync(null);
                _client = accepted.WebSocket;
                ClientConnectionChanged?.Invoke(this, true);
                Log?.Invoke(this, "Browser connected");
                try
                {
                    await ReceiveAsync(accepted.WebSocket, token);
                }
                catch (WebSocketException)
                {
                }
                finally
                {
                    _client.Dispose();
                    _client = null;
                    ClientConnectionChanged?.Invoke(this, false);
                    Log?.Invoke(this, "Browser disconnected");
                }
            }
        }
        catch (Exception exception) when (exception is OperationCanceledException or HttpListenerException or ObjectDisposedException)
        {
        }
    }

    private bool IsAuthorized(HttpListenerContext context)
    {
        var supplied = context.Request.QueryString["token"];
        var origin = context.Request.Headers["Origin"] ?? string.Empty;
        return context.Request.IsWebSocketRequest
            && settings.AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(supplied)
            && CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(supplied), Encoding.UTF8.GetBytes(settings.SocketToken));
    }

    private async Task ReceiveAsync(WebSocket socket, CancellationToken token)
    {
        var buffer = new byte[8192];
        using var message = new MemoryStream();
        while (socket.State == WebSocketState.Open && !token.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(buffer, token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closed", token);
                return;
            }
            if (result.MessageType != WebSocketMessageType.Text)
            {
                await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Text only", token);
                return;
            }
            if (message.Length + result.Count > MaximumMessageBytes)
            {
                await socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Too large", token);
                return;
            }

            message.Write(buffer, 0, result.Count);
            if (!result.EndOfMessage)
            {
                continue;
            }

            var json = Encoding.UTF8.GetString(message.ToArray());
            message.SetLength(0);
            Dispatch(json);
        }
    }

    private void Dispatch(string json)
    {
        try
        {
            var command = JsonSerializer.Deserialize<SocketCommand>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (command is not null && !string.IsNullOrWhiteSpace(command.Type))
            {
                CommandReceived?.Invoke(this, command);
            }
        }
        catch (JsonException)
        {
            Log?.Invoke(this, "Ignored invalid message");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _sendLock.Dispose();
    }
}
