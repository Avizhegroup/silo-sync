# Local WebSocket component

`LocalWebSocketComponent` connects the browser page to the local device host (`Silo.Windows.Register`, default `ws://127.0.0.1:5005/?token=...`) and exposes it through normal Blazor events. It is a text transport only: it does not interpret messages, so new host commands need no change here. `DeviceRpc` (same folder) adds request/reply on top of it.

```razor
<LocalWebSocketComponent @ref="Socket" Port="5005"
                         OnConnectionChanged="OnDeviceConnectionChanged"
                         OnMessageReceived="OnDeviceMessageReceived"
                         OnError="OnDeviceError" />
```

## Parameters

| Parameter | Default | Meaning |
|---|---|---|
| `Port` | 5005 | Port of the device host. |
| `AutoConnect` | `true` | On first render, connect with the token saved by the last successful connection. Set `false` to connect only from the button or `ConnectAsync()`. |
| `ConnectAttempts` | 4 | Attempts before an error is reported. The host accepts one browser at a time, so connecting right after leaving another page can be refused for a moment (HTTP 409) until the old socket has closed. |
| `RetryDelayMilliseconds` | 750 | Delay between attempts. |
| `OnConnectionChanged` | | `EventCallback<bool>`: true when connected, false when closed or when the final attempt failed. |
| `OnMessageReceived` | | `EventCallback<string>`: raw JSON text from the host. |
| `OnError` | | `EventCallback<string>`: reported once, after the last failed attempt. |

Methods: `SendAsync(string)`, `SendCommandAsync(object)` (serialized to JSON), `ConnectAsync()`, `DisconnectAsync()`, `IsConnectedAsync()`, property `IsConnected`.

The token is typed once in the component, then stored (encrypted) in the browser's protected local storage and reused by every page that hosts the component. The host must list the page's exact HTTPS origin in its allowed origins and use the same token. Messages are limited to 64 KiB and must be text.

## Request/reply with `DeviceRpc`

Create one `DeviceRpc` per page, pass every received message to `TryComplete`, and call `FailAll` when the connection drops:

```csharp
private readonly DeviceRpc _rpc = new();

public Task OnDeviceMessageReceived(string json) { _rpc.TryComplete(json); return Task.CompletedTask; }

var reply = await _rpc.RequestAsync(Socket!, id => new { type = "printerStatus", id, kind = "zmin" }, TimeSpan.FromSeconds(10));
// reply.Ok, reply.Value, reply.Code
```

Messages without an `id` (EPC and barcode events) are not replies; handle them in `OnMessageReceived` as before.

## Messages

Host to browser (`{"type","value"}`; replies also carry `id`, `ok`, `code`):

| type | meaning |
|---|---|
| `epc` | A 24-hex tag EPC was read (value). |
| `barcode` | A barcode was scanned (value). |
| `pong` | Answer to `ping`. |
| `error` | The host failed to run a command (value is the message). |
| `printers` / `formats` | Reply to `listPrinters` / `listFormats`; `value` is a JSON array (printer names / `{kind,name}`). |
| `printResult` | Result of `print` or `printFormat` (`ok`, `value` = message, `code` = printer status code). |
| `printerStatus` | Result of `printerStatus`. |

Browser to host:

| type | fields |
|---|---|
| `startReader` / `stopReader` | `mode`: 1 = single read, 2 = continuous. |
| `ping` | |
| `print` | `id`, `printer`, `data`: raw ZPL sent as is. |
| `listPrinters`, `listFormats` | `id`. |
| `printerStatus` | `id`, `kind` (`zpl` or `zmin`), `printer`. |
| `printFormat` | `id`, `kind`, `format` (template name), `printer`, `writeRfid`, `fields` (token to value), `technicalData` (name to value). |

## Notes

Payloads pass through the Blazor Server circuit (SignalR), so scanned data (EPCs, barcodes) reaches the server. Avoid using this for sensitive data. Browser policies for `ws://127.0.0.1` from HTTPS pages (mixed content, private network access) must be verified on the target browsers.
