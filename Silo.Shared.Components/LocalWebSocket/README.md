# Local WebSocket component

`LocalWebSocketComponent` connects the browser page to the local device host (`Silo.Windows.Register`, default `ws://127.0.0.1:5005/?token=...`) and exposes it through normal Blazor events.

```razor
<LocalWebSocketComponent @ref="_socket" Port="5005" SessionToken="@Token"
                         OnConnectionChanged="OnSocketConnectionChanged"
                         OnMessageReceived="OnSocketMessageReceived"
                         OnError="OnSocketError" />
```

- `OnConnectionChanged` (`EventCallback<bool>`): true when connected, false when closed.
- `OnMessageReceived` (`EventCallback<string>`): raw JSON text from the host, e.g. `{"type":"epc","value":"..."}` or `{"type":"barcode","value":"..."}`.
- `OnError` (`EventCallback<string>`): connection errors.
- `SendAsync(string)` / `SendCommandAsync(object)`: send to the host, e.g. `SendCommandAsync(new { type = "startReader", mode = 1 })`.
- `ConnectAsync()`, `DisconnectAsync()`, `IsConnectedAsync()`, `IsConnected`; set `AutoConnect="false"` to connect manually.

The host requires the page's exact HTTPS origin in its allowed origins and the same token as `SessionToken`. Only one browser can be connected at a time and messages are limited to 64 KiB.

Payloads pass through the Blazor Server circuit (SignalR), so scanned data (EPCs, barcodes) reaches the server. Avoid using this for sensitive data. Use a short-lived per-session token, because it is delivered to the browser through rendering. Browser policies for `ws://127.0.0.1` from HTTPS pages (mixed content, private network access) must be verified on the target browsers.
