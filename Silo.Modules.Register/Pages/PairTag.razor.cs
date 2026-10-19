using System.Text.Json;
using Microsoft.JSInterop;
using Silo.Modules.Register.Models;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

public enum PairResultKind
{
    Success,
    Warning,
    Error
}

public sealed record PairRow(int Index, string Serial, string Epc, string Status, PairResultKind Kind)
{
    public string CssClass => Kind switch
    {
        PairResultKind.Success => "table-success",
        PairResultKind.Warning => "table-warning",
        _ => "table-danger"
    };
}

/// <summary>
/// Pairs a scanned barcode serial with a read RFID tag (legacy F8, "barfid"). The barcode is typed by the scanner into
/// the page (or arrives from the device host), the EPC arrives from the desk reader through the device host. When both
/// are captured the decision table of the legacy form runs; any failure stops the run until the operator starts again.
/// </summary>
public partial class PairTag
{
    private static readonly TimeSpan UnmatchedTimeout = TimeSpan.FromSeconds(15);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly TimeSpan ListTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PrintTimeout = TimeSpan.FromSeconds(60);

    private readonly DeviceRpc _rpc = new();
    private string _userId = string.Empty;
    private string _barcode = string.Empty;
    private string _epc = string.Empty;
    private bool _saving;
    private CancellationTokenSource? _clearCts;
    private TaskCompletionSource<bool>? _confirm;

    public record DeviceMessage(string Type, string? Value);

    public bool IsOperationOn { get; set; }
    public bool IsSaving => _saving;
    public string Separator { get; set; } = ",";
    public string BarcodeInput { get; set; } = string.Empty;
    public string EpcInput { get; set; } = string.Empty;
    public string Message { get; set; } = "Press Start to begin.";
    public string MessageKind { get; set; } = "info";
    public string? ConfirmMessage { get; set; }
    public List<PairRow> Results { get; set; } = [];
    public bool HasBarcode => _barcode.Length > 0;
    public bool HasEpc => _epc.Length > 0;

    public ElementReference BarcodeBox { get; set; }
    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    /// <summary>Legacy "auto print after register": a FastReport receipt (serial and RFID) after each successful pairing.</summary>
    public bool AutoPrint { get; set; }
    public List<DeviceFormat> ReceiptFormats { get; set; } = [];
    public List<string> Printers { get; set; } = [];
    public string ReceiptFormatName { get; set; } = string.Empty;
    public string ReceiptPrinter { get; set; } = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        _userId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();
    }

    #region Operation

    public async Task StartOperationAsync()
    {
        IsOperationOn = true;
        ClearCapture();
        SetMessage("Hold the product sheet, barcode facing up, near the scanner.", "info");
        await RestartReaderAsync();
        await FocusBarcodeAsync();
    }

    public async Task StopOperationAsync()
    {
        IsOperationOn = false;
        CancelClearTimer();
        AnswerConfirm(false);
        ClearCapture();
        SetMessage("Stopped.", "secondary");
        await SendReaderCommandAsync("stopReader");
    }

    public async Task ClearCaptureAsync()
    {
        ClearCapture();
        SetMessage("Cleared. Scan the barcode and read the tag.", "info");
        await RestartReaderAsync();
        await FocusBarcodeAsync();
    }

    private void ClearCapture()
    {
        CancelClearTimer();
        _barcode = string.Empty;
        _epc = string.Empty;
        BarcodeInput = string.Empty;
        EpcInput = string.Empty;
    }

    #endregion

    #region Capture

    public async Task OnBarcodeKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await AcceptBarcodeAsync(BarcodeInput);
        }
    }

    public async Task OnEpcKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || BarcodeParser.NormalizeEpc(EpcInput).Length == BarcodeParser.EpcLength)
        {
            await AcceptEpcAsync(EpcInput);
        }
    }

    private async Task AcceptBarcodeAsync(string raw)
    {
        if (!IsOperationOn || _saving)
        {
            return;
        }

        var serial = BarcodeParser.ExtractSerial(raw, Separator);
        if (!BarcodeParser.IsValidSerial(serial))
        {
            BarcodeInput = string.Empty;
            SetMessage("The barcode is not a valid serial.", "danger");
            return;
        }

        _barcode = serial;
        BarcodeInput = serial;
        SetMessage(HasEpc ? "Saving..." : "Waiting for the tag...", "info");
        await TryPairAsync();
    }

    private async Task AcceptEpcAsync(string raw)
    {
        if (!IsOperationOn || _saving)
        {
            return;
        }

        var epc = BarcodeParser.NormalizeEpc(raw);
        if (!BarcodeParser.IsValidEpc(epc))
        {
            EpcInput = string.Empty;
            SetMessage("The EPC must be 24 hexadecimal characters.", "danger");
            return;
        }

        _epc = epc;
        EpcInput = epc;
        SetMessage(HasBarcode ? "Saving..." : "Waiting for the barcode...", "info");
        await TryPairAsync();
    }

    /// <summary>Starts the save when both values are present, otherwise arms the timer that drops a lone capture.</summary>
    private async Task TryPairAsync()
    {
        if (!HasBarcode || !HasEpc)
        {
            ScheduleClear();
            return;
        }

        CancelClearTimer();
        var serial = _barcode;
        var epc = _epc;
        ClearCapture();

        _saving = true;
        bool ok;
        try
        {
            ok = await SaveRegisterAsync(serial, epc);
        }
        finally
        {
            _saving = false;
        }

        if (!IsOperationOn)
        {
            return;
        }

        if (ok)
        {
            SetMessage("Next: scan the barcode and read the tag.", "info");
            await RestartReaderAsync();
            await FocusBarcodeAsync();
        }
        else
        {
            // Legacy rule: an error stops the pairing loop; the operator starts again deliberately.
            IsOperationOn = false;
            await SendReaderCommandAsync("stopReader");
        }
    }

    private void ScheduleClear()
    {
        CancelClearTimer();
        var source = _clearCts = new CancellationTokenSource();
        _ = ClearLaterAsync(source.Token);
    }

    private void CancelClearTimer()
    {
        _clearCts?.Cancel();
        _clearCts?.Dispose();
        _clearCts = null;
    }

    private async Task ClearLaterAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(UnmatchedTimeout, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        await InvokeAsync(async () =>
        {
            if (_saving || !IsOperationOn || (HasBarcode && HasEpc))
            {
                return;
            }

            ClearCapture();
            SetMessage("The capture timed out and was cleared. Scan the barcode and read the tag again.", "warning");
            await RestartReaderAsync();
            StateHasChanged();
        });
    }

    #endregion

    #region Register (legacy decision table)

    /// <returns>false stops the run.</returns>
    private async Task<bool> SaveRegisterAsync(string serial, string epc)
    {
        try
        {
            // 1. The tag already belongs to a serial: moving it needs the operator's consent.
            var boundSerial = (await Api.PostAsync<string>("SCheckTagEPCReturnProductSerial"
                , new KeyValuePair<string, object>("TagEpc", epc))).Value?.Trim() ?? string.Empty;

            if (boundSerial.Length > 0
                && !await ConfirmAsync($"This tag is already registered for serial {boundSerial}. Remove it from that serial and assign it to {serial}?"))
            {
                return Report(serial, epc, "The tag is registered for another serial; not changed.", PairResultKind.Error);
            }

            // 2. Where is the serial?
            var state = (await Api.PostAsync<string>("SIsProductSerialExist"
                , new KeyValuePair<string, object>("ProductSerial", serial)
                , new KeyValuePair<string, object>("username", string.Empty)
                , new KeyValuePair<string, object>("deviceIp", string.Empty)
                , new KeyValuePair<string, object>("deviceId", string.Empty))).Value?.Trim();

            switch (state)
            {
                case "0":
                    return Report(serial, epc, "No information was found to register this serial.", PairResultKind.Error);

                case "1":
                    return await RegisterAsync(serial, epc, "Tag registered.", "The tag could not be registered.");

                case "2":
                    if (!await ConfirmAsync($"Serial {serial} is already registered. Replace its tag with this one?"))
                    {
                        // The legacy form treated "No" as success: the run continues.
                        Report(serial, epc, "Already registered; left unchanged.", PairResultKind.Warning);
                        return true;
                    }

                    return await RegisterAsync(serial, epc, "Tag registration was updated.", "The tag registration could not be updated.");

                default:
                    return Report(serial, epc, "Unexpected answer from the server.", PairResultKind.Error);
            }
        }
        catch
        {
            return Report(serial, epc, TextResources.APP_StringKeys_Alert_Fail, PairResultKind.Error);
        }
    }

    private async Task<bool> RegisterAsync(string serial, string epc, string successText, string failureText)
    {
        var registered = (await Api.PostAsync<bool>("SRegisterTag"
            , new KeyValuePair<string, object>("productSerial", serial)
            , new KeyValuePair<string, object>("productCode", string.Empty)
            , new KeyValuePair<string, object>("productCount", string.Empty)
            , new KeyValuePair<string, object>("epcList", new[] { epc })
            , new KeyValuePair<string, object>("username", _userId)
            , new KeyValuePair<string, object>("deviceId", "0")
            , new KeyValuePair<string, object>("deviceIp", "0")
            , new KeyValuePair<string, object>("RegisterProperties", string.Empty))).Value;

        var ok = Report(serial, epc, registered ? successText : failureText, registered ? PairResultKind.Success : PairResultKind.Error);
        if (registered && AutoPrint)
        {
            await PrintReceiptAsync(serial, epc);
        }

        return ok;
    }

    /// <summary>
    /// Prints the receipt template with the serial and the tag EPC (legacy PrintTestAfterRegister.frx: parameters
    /// "سریال محموله" and "RFID"). A failure is shown as a warning and does not stop the pairing run.
    /// </summary>
    private async Task PrintReceiptAsync(string serial, string epc)
    {
        if (Socket is null || !IsDeviceConnected || string.IsNullOrEmpty(ReceiptFormatName))
        {
            Notification.Show("Auto print is on, but no receipt format is available.", "warning");
            return;
        }

        try
        {
            var reply = await _rpc.RequestAsync(Socket, id => new
            {
                type = "printFormat",
                id,
                kind = "frx",
                format = ReceiptFormatName,
                printer = ReceiptPrinter,
                output = "print",
                fields = new Dictionary<string, string>
                {
                    ["سریال_محموله"] = serial,
                    ["RFID"] = epc,
                    ["شناسه_تگ"] = epc
                }
            }, PrintTimeout);

            if (!reply.Ok)
            {
                Notification.Show($"The receipt was not printed: {reply.Value}", "warning");
            }
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            Notification.Show($"The receipt was not printed: {exception.Message}", "warning");
        }
    }

    public async Task RefreshReceiptFormatsAsync()
    {
        if (Socket is null || !IsDeviceConnected)
        {
            return;
        }

        try
        {
            var formats = await _rpc.RequestAsync(Socket, id => new { type = "listFormats", id }, ListTimeout);
            ReceiptFormats = (JsonSerializer.Deserialize<List<DeviceFormat>>(formats.Value ?? "[]", JsonOptions) ?? [])
                .Where(p => p.IsFastReport).ToList();

            var printers = await _rpc.RequestAsync(Socket, id => new { type = "listPrinters", id }, ListTimeout);
            Printers = JsonSerializer.Deserialize<List<string>>(printers.Value ?? "[]", JsonOptions) ?? [];

            if (!ReceiptFormats.Any(p => p.Name == ReceiptFormatName))
            {
                ReceiptFormatName = ReceiptFormats.FirstOrDefault(p => p.Name.StartsWith("PrintTestAfterRegister", StringComparison.OrdinalIgnoreCase))?.Name
                    ?? ReceiptFormats.FirstOrDefault()?.Name ?? string.Empty;
            }
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException or JsonException)
        {
            Notification.Show(exception.Message, "error");
        }
    }

    private bool Report(string serial, string epc, string status, PairResultKind kind)
    {
        Results.Insert(0, new PairRow(Results.Count + 1, serial, epc, status, kind));
        SetMessage(status, kind switch
        {
            PairResultKind.Success => "success",
            PairResultKind.Warning => "warning",
            _ => "danger"
        });

        return kind != PairResultKind.Error;
    }

    private Task<bool> ConfirmAsync(string question)
    {
        _confirm = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        ConfirmMessage = question;
        StateHasChanged();
        return _confirm.Task;
    }

    public void AnswerConfirm(bool answer)
    {
        ConfirmMessage = null;
        _confirm?.TrySetResult(answer);
        _confirm = null;
    }

    #endregion

    #region Device host

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;
        if (connected && IsOperationOn)
        {
            await RestartReaderAsync();
        }

        if (connected)
        {
            await RefreshReceiptFormatsAsync();
        }
        else
        {
            _rpc.FailAll("The device host disconnected.");
        }

        if (!connected && IsOperationOn)
        {
            SetMessage("The device host disconnected. Reconnect, then press Start again.", "danger");
            IsOperationOn = false;
        }

        StateHasChanged();
    }

    public async Task OnDeviceMessageReceived(string json)
    {
        if (_rpc.TryComplete(json))
        {
            return;
        }

        DeviceMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<DeviceMessage>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return;
        }

        switch (message?.Type)
        {
            case "epc" when message.Value is not null:
                await AcceptEpcAsync(message.Value);
                break;
            case "barcode" when message.Value is not null:
                await AcceptBarcodeAsync(message.Value);
                break;
            case "error" when message.Value is not null:
                SetMessage(message.Value, "danger");
                break;
        }

        StateHasChanged();
    }

    public void OnDeviceError(string message) => Notification.Show(message, "error");

    private Task RestartReaderAsync() => SendReaderCommandAsync("startReader", 1);

    private async Task SendReaderCommandAsync(string type, int? mode = null)
    {
        if (Socket is null || !IsDeviceConnected)
        {
            return;
        }

        try
        {
            await Socket.SendCommandAsync(mode is null ? new { type } : new { type, mode });
        }
        catch (InvalidOperationException)
        {
            SetMessage("The device host is not connected.", "danger");
        }
    }

    private async Task FocusBarcodeAsync()
    {
        try
        {
            await BarcodeBox.FocusAsync();
        }
        catch (Exception exception) when (exception is InvalidOperationException or JSDisconnectedException)
        {
        }
    }

    #endregion

    private void SetMessage(string text, string kind)
    {
        Message = text;
        MessageKind = kind;
    }

    public void Dispose()
    {
        CancelClearTimer();
        _confirm?.TrySetResult(false);
    }
}
