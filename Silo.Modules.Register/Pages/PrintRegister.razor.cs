using System.Text.Json;
using System.Text.RegularExpressions;
using Silo.Modules.Register.Models;
using Silo.Modules.Register.Services;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Registers the tags of a print operation and prints their labels (legacy forms F6/F7).
/// Per serial: get/reuse the EPC, register the tag on the server, print the label through the device host
/// (ZPL or ZMIN) and mark the print row. The labels are printed on the workstation that runs Silo.Windows.Register.
/// </summary>
public partial class PrintRegister
{
    private static readonly Regex ActionIdRegex = new("^[0-9]+$", RegexOptions.Compiled);
    private static readonly Regex EpcRegex = new("^[0-9A-F]{24}$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan ListTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PrintTimeout = TimeSpan.FromSeconds(60);

    private readonly DeviceRpc _rpc = new();
    private string _userId = string.Empty;

    public bool IsLoading { get; set; }
    public bool IsBusy { get; set; }
    public string ActionId { get; set; } = string.Empty;
    public List<PrintLabelRow> Rows { get; set; } = [];

    public List<DeviceFormat> Formats { get; set; } = [];
    public List<string> Printers { get; set; } = [];
    public string SelectedFormatKey { get; set; } = string.Empty;
    public string SelectedPrinter { get; set; } = string.Empty;
    public string EpcType { get; set; } = "2";
    public bool WriteRfid { get; set; }
    public bool AllowReprint { get; set; }

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "actionId")] public string? ActionIdQuery { get; set; }

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    public DeviceFormat? SelectedFormat => Formats.FirstOrDefault(p => p.Key == SelectedFormatKey);

    public bool CanPrint => IsDeviceConnected && !IsBusy && !IsLoading && SelectedFormat is not null && Rows.Any(p => p.IsSelected);

    protected override async Task SiloInitializer()
    {
        _userId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        if (!string.IsNullOrWhiteSpace(ActionIdQuery))
        {
            ActionId = ActionIdQuery.Trim();
            await LoadAsync();
        }
    }

    public async Task OnActionKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await LoadAsync();
        }
    }

    public async Task LoadAsync()
    {
        var actionId = ActionId.Trim();
        if (!ActionIdRegex.IsMatch(actionId))
        {
            Notification.Show("Enter a numeric operation code.", "error");
            return;
        }

        IsLoading = true;
        try
        {
            var rows = (await Api.PostAsync<List<PrintLabelRow>>("SGetPrintListByPrintActionId"
                , new KeyValuePair<string, object>("PrintActionId", actionId)
                , new KeyValuePair<string, object>("userToken", _userId))).Value ?? [];

            rows.ForEach(p => p.IsSelected = !p.RegisterFlag);
            Rows = rows;

            if (Rows.Count == 0)
            {
                Notification.Show("No serial was found for this operation.", "warning");
            }
        }
        catch
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SelectAll(bool selected) => Rows.ForEach(p => p.IsSelected = selected);

    public string RowClass(PrintLabelRow row) => row.Succeeded switch
    {
        true => "table-success",
        false => "table-danger",
        _ => string.Empty
    };

    #region Device host

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;

        if (connected)
        {
            await RefreshDeviceAsync();
        }
        else
        {
            _rpc.FailAll("The device host disconnected.");
            Formats = [];
            Printers = [];
        }

        StateHasChanged();
    }

    public Task OnDeviceMessageReceived(string json)
    {
        _rpc.TryComplete(json);
        return Task.CompletedTask;
    }

    public void OnDeviceError(string message) => Notification.Show(message, "error");

    public async Task RefreshDeviceAsync()
    {
        try
        {
            var formats = await _rpc.RequestAsync(Socket!, id => new { type = "listFormats", id }, ListTimeout);
            Formats = JsonSerializer.Deserialize<List<DeviceFormat>>(formats.Value ?? "[]", JsonOptions) ?? [];

            var printers = await _rpc.RequestAsync(Socket!, id => new { type = "listPrinters", id }, ListTimeout);
            Printers = JsonSerializer.Deserialize<List<string>>(printers.Value ?? "[]", JsonOptions) ?? [];

            if (SelectedFormat is null)
            {
                SelectedFormatKey = Formats.FirstOrDefault()?.Key ?? string.Empty;
            }

            if (Formats.Count == 0)
            {
                Notification.Show("The device host has no label format. Add templates to its Formats folder.", "warning");
            }
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException or JsonException)
        {
            Notification.Show(exception.Message, "error");
        }
    }

    public async Task CheckPrinterAsync()
    {
        var format = SelectedFormat;
        if (format is null)
        {
            return;
        }

        try
        {
            var reply = await _rpc.RequestAsync(Socket!, id => new
            {
                type = "printerStatus",
                id,
                kind = format.Kind,
                printer = SelectedPrinter
            }, ListTimeout);

            Notification.Show(reply.Value ?? string.Empty, reply.Ok ? "success" : "error");
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            Notification.Show(exception.Message, "error");
        }
    }

    #endregion

    #region Register and print

    public async Task PrintSelectedAsync()
    {
        var format = SelectedFormat;
        var selected = Rows.Where(p => p.IsSelected).ToList();
        if (format is null || selected.Count == 0)
        {
            return;
        }

        var registered = selected.Count(p => p.RegisterFlag);
        if (registered > 0 && !AllowReprint)
        {
            Notification.Show($"{registered} selected serial(s) are already registered. Tick 'Allow reprint' to print them again.", "warning");
            return;
        }

        IsBusy = true;
        try
        {
            if (!await IsPrinterReadyAsync(format))
            {
                return;
            }

            foreach (var row in selected)
            {
                row.Succeeded = null;
                row.Status = "Working...";
                StateHasChanged();

                var (ok, message, stopBatch) = await ProcessAsync(row, format);

                row.Succeeded = ok;
                row.Status = message;
                StateHasChanged();

                if (!ok && stopBatch)
                {
                    break;
                }
            }

            var done = selected.Count(p => p.Succeeded == true);
            Notification.Show($"{done} of {selected.Count} label(s) printed.", done == selected.Count ? "success" : "warning");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<bool> IsPrinterReadyAsync(DeviceFormat format)
    {
        try
        {
            var status = await _rpc.RequestAsync(Socket!, id => new
            {
                type = "printerStatus",
                id,
                kind = format.Kind,
                printer = SelectedPrinter
            }, ListTimeout);

            if (!status.Ok)
            {
                Notification.Show(status.Value ?? "The printer is not ready.", "error");
            }

            return status.Ok;
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            Notification.Show(exception.Message, "error");
            return false;
        }
    }

    private async Task<(bool Ok, string Message, bool StopBatch)> ProcessAsync(PrintLabelRow row, DeviceFormat format)
    {
        try
        {
            var epc = (row.TagEpc ?? string.Empty).Trim().ToUpperInvariant();

            // 1. The tag: reuse the registered EPC (reprint) or create and register a new one.
            if (!EpcRegex.IsMatch(epc))
            {
                epc = (await Api.PostAsync<string>("SCreateEpc"
                    , new KeyValuePair<string, object>("serial", row.ProductSerial ?? string.Empty)
                    , new KeyValuePair<string, object>("productCode", row.ProductCode ?? string.Empty)
                    , new KeyValuePair<string, object>("refCode", row.ProductRegCode ?? string.Empty)
                    , new KeyValuePair<string, object>("epc", string.Empty)
                    , new KeyValuePair<string, object>("type", EpcType))).Value?.Trim().ToUpperInvariant() ?? string.Empty;

                if (!EpcRegex.IsMatch(epc))
                {
                    return (false, $"The server did not return a valid EPC for type '{EpcType}'.", false);
                }

                var registered = (await Api.PostAsync<bool>("SRegisterTag"
                    , new KeyValuePair<string, object>("productSerial", row.ProductSerial ?? string.Empty)
                    , new KeyValuePair<string, object>("productCode", row.ProductCode ?? string.Empty)
                    , new KeyValuePair<string, object>("productCount", row.ProductCount ?? string.Empty)
                    , new KeyValuePair<string, object>("epcList", new[] { epc })
                    , new KeyValuePair<string, object>("username", _userId)
                    , new KeyValuePair<string, object>("deviceId", "0")
                    , new KeyValuePair<string, object>("deviceIp", "0")
                    , new KeyValuePair<string, object>("RegisterProperties", string.Empty))).Value;

                if (!registered)
                {
                    return (false, "The tag could not be registered.", false);
                }

                row.TagEpc = epc;
            }

            // 2. The label.
            if (!LabelFieldBuilder.TryBuild(row, epc, out var fields, out var technicalData, out var error))
            {
                return (false, error, false);
            }

            var reply = await _rpc.RequestAsync(Socket!, id => new
            {
                type = "printFormat",
                id,
                kind = format.Kind,
                format = format.Name,
                printer = SelectedPrinter,
                writeRfid = WriteRfid,
                fields,
                technicalData
            }, PrintTimeout);

            if (!reply.Ok)
            {
                // The tag stays registered with its EPC: after fixing the printer the same row can simply be printed again.
                return (false, $"Registered, but printing failed: {reply.Value}", true);
            }

            // 3. Mark the print row as registered.
            var marked = (await Api.PostAsync<bool>("PrintUpdatePrintStatus"
                , new KeyValuePair<string, object>("productSerial", row.ProductSerial ?? string.Empty))).Value;

            row.RegisterFlag = true;
            return (true, marked ? "Registered and printed." : "Printed, but the print status was not updated.", false);
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            return (false, exception.Message, true);
        }
        catch
        {
            return (false, TextResources.APP_StringKeys_Alert_Fail, false);
        }
    }

    #endregion
}
