using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Silo.Ui.Gate;

public sealed class LocalWebSocketServer : IAsyncDisposable
{
    private const int MaximumMessageBytes = 64 * 1024;
    private static readonly SemaphoreSlim InstanceLock = new(1, 1);
    private static LocalWebSocketServer? _instance;
    private readonly HttpListener _listener = new();
    private readonly int _port;
    private readonly HashSet<string> _allowedOrigins;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private CancellationTokenSource? _shutdown;
    private Task? _acceptLoop;
    private WebSocket? _browser;

    public LocalWebSocketServer(IEnumerable<string> allowedOrigins, int port = 5005)
    {
        ArgumentNullException.ThrowIfNull(allowedOrigins);
        if (port is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }

        _port = port;
        _allowedOrigins = new HashSet<string>(allowedOrigins, StringComparer.OrdinalIgnoreCase);
        if (_allowedOrigins.Any(origin =>
                !Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
                uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("At least one HTTPS browser origin must be configured.", nameof(allowedOrigins));
        }
    }

    public event EventHandler<string>? MessageReceived;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_allowedOrigins.Count == 0)
        {
            throw new InvalidOperationException("Configure SILO_LOCAL_WS_ORIGINS with at least one allowed HTTPS origin.");
        }
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SILO_LOCAL_WS_TOKEN")))
        {
            throw new InvalidOperationException("Configure SILO_LOCAL_WS_TOKEN before starting the local WebSocket server.");
        }

        await InstanceLock.WaitAsync(cancellationToken);
        try
        {
            if (_instance is not null && !ReferenceEquals(_instance, this))
            {
                throw new InvalidOperationException("A local WebSocket server is already running in this process.");
            }
            if (_acceptLoop is not null)
            {
                return;
            }

            _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
            try
            {
                _listener.Start();
            }
            catch (HttpListenerException exception)
            {
                throw new InvalidOperationException(
                    $"Could not start the local WebSocket server on 127.0.0.1:{_port}. Check whether the port is in use.", exception);
            }

            _instance = this;
            _shutdown = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _acceptLoop = AcceptConnectionsAsync(_shutdown.Token);
        }
        finally
        {
            InstanceLock.Release();
        }
    }

    public async Task SendAsync(string data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        var browser = _browser;
        if (browser?.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("The browser is not connected to the local WebSocket server.");
        }

        var bytes = Encoding.UTF8.GetBytes(data);
        if (bytes.Length > MaximumMessageBytes)
        {
            throw new ArgumentException($"Messages cannot exceed {MaximumMessageBytes} UTF-8 bytes.", nameof(data));
        }

        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            await browser.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync().WaitAsync(cancellationToken);
                var configuredToken = Environment.GetEnvironmentVariable("SILO_LOCAL_WS_TOKEN");
                var suppliedToken = context.Request.QueryString["token"];
                if (!_allowedOrigins.Contains(context.Request.Headers["Origin"] ?? string.Empty) ||
                    !context.Request.IsWebSocketRequest ||
                    string.IsNullOrWhiteSpace(configuredToken) ||
                    string.IsNullOrEmpty(suppliedToken) ||
                    !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                        Encoding.UTF8.GetBytes(suppliedToken), Encoding.UTF8.GetBytes(configuredToken)))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.Close();
                    continue;
                }

                if (_browser?.State == WebSocketState.Open)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    context.Response.Close();
                    continue;
                }

                var accepted = await context.AcceptWebSocketAsync(null);
                _browser = accepted.WebSocket;
                await ReceiveMessagesAsync(accepted.WebSocket, cancellationToken);
                _browser = null;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            _browser?.Abort();
            _browser?.Dispose();
            _browser = null;
        }
    }

    private async Task ReceiveMessagesAsync(WebSocket socket, CancellationToken cancellationToken)
    {
        var buffer = new byte[8192];
        using var message = new MemoryStream();
        while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(buffer, cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", cancellationToken);
                return;
            }
            if (result.MessageType != WebSocketMessageType.Text)
            {
                await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Text messages only", cancellationToken);
                return;
            }
            if (message.Length + result.Count > MaximumMessageBytes)
            {
                await socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message too large", cancellationToken);
                return;
            }

            message.Write(buffer, 0, result.Count);
            if (result.EndOfMessage)
            {
                try
                {
                    MessageReceived?.Invoke(this, new UTF8Encoding(false, true).GetString(message.ToArray()));
                }
                catch (DecoderFallbackException)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid UTF-8", cancellationToken);
                    return;
                }
                message.SetLength(0);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await InstanceLock.WaitAsync(cancellationToken);
        try
        {
            if (_acceptLoop is null)
            {
                return;
            }
            _shutdown?.Cancel();
            _listener.Stop();
            _browser?.Abort();
            await _acceptLoop.WaitAsync(cancellationToken);
            _shutdown?.Dispose();
            _shutdown = null;
            _acceptLoop = null;
            _listener.Close();
            if (ReferenceEquals(_instance, this))
            {
                _instance = null;
            }
        }
        finally
        {
            InstanceLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _sendLock.Dispose();
    }
}
