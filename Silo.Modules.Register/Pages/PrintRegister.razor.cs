using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Silo.Modules.Register.Models;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Registers the tags of a print operation and prints their labels (legacy forms F6/F7). Labels are printed on the
/// workstation that runs Silo.Windows.Register: ZPL, ZMIN or FastReport (<c>.frx</c>) templates; FastReport output
/// can also be one merged PDF that opens on that workstation (legacy "merge to PDF" and the preview).
/// Three modes replace the legacy <c>KindRegister</c> setting:
/// <list type="bullet">
/// <item><b>register</b> (1): per serial get/reuse the EPC, register the tag on the server, print, mark the print row;</item>
/// <item><b>print</b> (2 and 4): only print; the tags are paired afterwards on <c>/register/pair</c>;</item>
/// <item><b>autoregister</b> (3): print the barcode, then register the tag with the serial's digits as EPC (letters removed).</item>
/// </list>
/// A PDF is never registered or marked.
/// </summary>
public partial class PrintRegister
{
    /// <summary>The steps of the "how to use this page" panel.</summary>
    public string[] HowToSteps { get; } =
    [
        RegisterTexts.PrintHowTo1,
        RegisterTexts.PrintHowTo2,
        RegisterTexts.PrintHowTo3,
        RegisterTexts.PrintHowTo4
    ];

    private static readonly Regex ActionIdRegex = new("^[0-9]+$", RegexOptions.Compiled);
    private static readonly Regex EpcRegex = new("^[0-9A-F]{24}$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan ListTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PrintTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PdfTimeout = TimeSpan.FromSeconds(180);
    private static readonly Regex LetterRegex = new("[A-Za-z]", RegexOptions.Compiled);

    private const int PdfChunkSize = 25;
    private const string ModeStorageKey = "Silo.Register.PrintMode";

    public IReadOnlyList<OptionItem> ModeOptions { get; } =
    [
        new("register", RegisterTexts.PrintModeRegister),
        new("print", RegisterTexts.PrintModePrint),
        new("autoregister", RegisterTexts.PrintModeAutoRegister)
    ];

    public IReadOnlyList<OptionItem> OutputOptions { get; } =
    [
        new("print", RegisterTexts.PrintFrxPrinter),
        new("pdf", RegisterTexts.PrintFrxPdf)
    ];

    public IReadOnlyList<OptionItem> EpcTypeOptions { get; } =
    [
        new("2", RegisterTexts.PrintEpcType2),
        new("1", RegisterTexts.PrintEpcType1),
        new("kdt", "kdt")
    ];

    public const string ModeRegister = "register";
    public const string ModePrint = "print";
    public const string ModeAutoRegister = "autoregister";

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

    /// <summary>register | print | autoregister (see the class summary).</summary>
    public string PrintMode { get; set; } = ModeRegister;

    /// <summary>"print" sends the FastReport label to a printer, "pdf" opens one merged PDF on the workstation.</summary>
    public string FrxOutput { get; set; } = "print";

    /// <summary>Legacy "control print format by brand": FastReport files named title_printer_brand are filtered by the product brand.</summary>
    public bool FilterByBrand { get; set; }

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "actionId")] public string? ActionIdQuery { get; set; }

    [Inject] public RfidConnectApi Api { get; set; } = default!;
    [Inject] public ProtectedLocalStorage LocalStorage { get; set; } = default!;

    public DeviceFormat? SelectedFormat => VisibleFormats.FirstOrDefault(p => p.Key == SelectedFormatKey);

    public bool IsPdf => SelectedFormat?.IsFastReport == true && FrxOutput == "pdf";

    public bool NeedsPrinter => SelectedFormat is { Kind: "zpl" } || (SelectedFormat?.IsFastReport == true && !IsPdf);

    /// <summary>The brand of the first selected serial (the filter applies to one product at a time).</summary>
    public string? SelectedBrand => Rows.FirstOrDefault(p => p.IsSelected)?.ProductBrand?.Trim();

    public IEnumerable<DeviceFormat> VisibleFormats => !FilterByBrand || string.IsNullOrEmpty(SelectedBrand)
        ? Formats
        : Formats.Where(p => p.Brand is null || p.Brand == SelectedBrand);

    public bool CanPrint => IsDeviceConnected && !IsBusy && !IsLoading && SelectedFormat is not null && Rows.Any(p => p.IsSelected);

    public string PrintButtonText => IsPdf
        ? RegisterTexts.PrintExportPdf
        : PrintMode switch
        {
            ModePrint => TextResources.APP_StringKeys_Print,
            ModeAutoRegister => RegisterTexts.PrintAndRegister,
            _ => RegisterTexts.PrintRegisterAndPrint
        };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            var stored = await LocalStorage.GetAsync<string>(ModeStorageKey);
            if (stored.Success && (stored.Value is ModeRegister or ModePrint or ModeAutoRegister) && stored.Value != PrintMode)
            {
                PrintMode = stored.Value!;
                StateHasChanged();
            }
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.Security.Cryptography.CryptographicException or JSDisconnectedException)
        {
            // The remembered mode is a convenience only.
        }
    }

    public async Task OnModeChanged(string value)
    {
        PrintMode = value is ModePrint or ModeAutoRegister ? value : ModeRegister;
        try
        {
            await LocalStorage.SetAsync(ModeStorageKey, PrintMode);
        }
        catch (Exception exception) when (exception is InvalidOperationException or JSDisconnectedException)
        {
        }
    }

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
            Notification.Show(TextResources.APP_StringKeys_Validation_Required_OperationCode, "error");
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
                Notification.Show(TextResources.APP_StringKeys_Error_SerialsNotFound, "warning");
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

    public string? RowKind(PrintLabelRow row) => row.Succeeded switch
    {
        true => "success",
        false => "danger",
        _ => null
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
            _rpc.FailAll(RegisterTexts.DeviceHostDisconnected);
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
                SelectedFormatKey = VisibleFormats.FirstOrDefault()?.Key ?? string.Empty;
            }

            if (Formats.Count == 0)
            {
                Notification.Show(RegisterTexts.PrintNoFormat, "warning");
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
                format = format.Name,
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

        var registersTags = !IsPdf && PrintMode != ModePrint;
        var registered = selected.Count(p => p.RegisterFlag);
        if (registersTags && registered > 0 && !AllowReprint)
        {
            Notification.Show($"{registered} selected serial(s) are already registered. Tick 'Allow reprint' to print them again.", "warning");
            return;
        }

        IsBusy = true;
        try
        {
            if (IsPdf)
            {
                await ExportPdfAsync(selected, format);
                return;
            }

            if (!await IsPrinterReadyAsync(format))
            {
                return;
            }

            foreach (var row in selected)
            {
                row.Succeeded = null;
                row.Status = TextResources.APP_StringKeys_Wait;
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

    /// <summary>Legacy preview: the first selected label as a PDF that opens on the workstation.</summary>
    public async Task PreviewAsync()
    {
        var format = SelectedFormat;
        var first = Rows.FirstOrDefault(p => p.IsSelected);
        if (format is not { IsFastReport: true } || first is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            await ExportPdfAsync([first], format);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// One merged PDF of all rows (legacy "merge print to PDF"). The labels travel in chunks of one job so a message
    /// stays small; the host opens the PDF after the last chunk. Nothing is registered or marked.
    /// </summary>
    private async Task ExportPdfAsync(IReadOnlyList<PrintLabelRow> rows, DeviceFormat format)
    {
        var job = Guid.NewGuid().ToString("N");

        try
        {
            for (var index = 0; index < rows.Count; index += PdfChunkSize)
            {
                var items = new List<object>();
                foreach (var row in rows.Skip(index).Take(PdfChunkSize))
                {
                    if (!LabelFieldBuilder.TryBuild(row, (row.TagEpc ?? string.Empty).Trim().ToUpperInvariant(), out var fields, out var technicalData, out var error))
                    {
                        Notification.Show($"{row.ProductSerial}: {error}", "error");
                        return;
                    }

                    items.Add(new { fields, technicalData });
                }

                var more = index + PdfChunkSize < rows.Count;
                var reply = await _rpc.RequestAsync(Socket!, id => new
                {
                    type = "printFormat",
                    id,
                    kind = format.Kind,
                    format = format.Name,
                    output = "pdf",
                    job,
                    more,
                    items
                }, more ? PrintTimeout : PdfTimeout);

                if (!reply.Ok)
                {
                    Notification.Show(reply.Value ?? RegisterTexts.PrintPdfFailed, "error");
                    return;
                }

                if (!more)
                {
                    Notification.Show(reply.Value ?? RegisterTexts.PrintPdfOpened, "success");
                }
            }
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            Notification.Show(exception.Message, "error");
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
                format = format.Name,
                printer = SelectedPrinter
            }, ListTimeout);

            if (!status.Ok)
            {
                Notification.Show(status.Value ?? RegisterTexts.PrinterNotReady, "error");
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
            var registeredBefore = row.RegisterFlag;

            // 1. Mode "register": the tag exists before the label is printed (the label may carry the EPC / write the chip).
            if (PrintMode == ModeRegister && !EpcRegex.IsMatch(epc))
            {
                epc = (await Api.PostAsync<string>("SCreateEpc"
                    , new KeyValuePair<string, object>("serial", row.ProductSerial ?? string.Empty)
                    , new KeyValuePair<string, object>("productCode", row.ProductCode ?? string.Empty)
                    , new KeyValuePair<string, object>("refCode", row.ProductRegCode ?? string.Empty)
                    , new KeyValuePair<string, object>("epc", string.Empty)
                    , new KeyValuePair<string, object>("type", EpcType))).Value?.Trim().ToUpperInvariant() ?? string.Empty;

                if (!EpcRegex.IsMatch(epc))
                {
                    return (false, RegisterTexts.PrintInvalidEpc(EpcType), false);
                }

                if (!await RegisterTagAsync(row, epc))
                {
                    return (false, TextResources.APP_StringKeys_Save_Failed, false);
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
                output = "print",
                fields,
                technicalData
            }, PrintTimeout);

            if (!reply.Ok)
            {
                // In mode "register" the tag stays registered with its EPC: after fixing the printer the same row can simply be printed again.
                return (false, PrintMode == ModeRegister ? RegisterTexts.PrintRegisteredButFailed(reply.Value ?? string.Empty) : RegisterTexts.PrintFailed(reply.Value ?? string.Empty), true);
            }

            if (PrintMode == ModePrint)
            {
                return (true, RegisterTexts.PrintedPair, false);
            }

            // 3a. Mode "autoregister" (legacy KindRegister 3): the printed barcode is the identity, the EPC is the serial's digits.
            if (PrintMode == ModeAutoRegister)
            {
                if (registeredBefore)
                {
                    return (true, RegisterTexts.PrintedAlready, false);
                }

                var autoEpc = LetterRegex.Replace(row.ProductSerial ?? string.Empty, string.Empty);
                if (autoEpc.Length == 0)
                {
                    return (false, RegisterTexts.PrintedNoDigits, true);
                }

                if (!await RegisterTagAsync(row, autoEpc))
                {
                    return (false, RegisterTexts.PrintedNotRegistered, true);
                }

                row.TagEpc = autoEpc;
            }

            // 3b. Mark the print row as registered.
            var marked = (await Api.PostAsync<bool>("PrintUpdatePrintStatus"
                , new KeyValuePair<string, object>("productSerial", row.ProductSerial ?? string.Empty))).Value;

            row.RegisterFlag = true;
            return (true, PrintMode == ModeAutoRegister
                ? (marked ? RegisterTexts.PrintedAndRegistered : RegisterTexts.PrintedRegisteredStatusNotUpdated)
                : (marked ? RegisterTexts.RegisteredAndPrinted : RegisterTexts.PrintedStatusNotUpdated), false);
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

    private async Task<bool> RegisterTagAsync(PrintLabelRow row, string epc)
        => (await Api.PostAsync<bool>("SRegisterTag"
            , new KeyValuePair<string, object>("productSerial", row.ProductSerial ?? string.Empty)
            , new KeyValuePair<string, object>("productCode", row.ProductCode ?? string.Empty)
            , new KeyValuePair<string, object>("productCount", row.ProductCount ?? string.Empty)
            , new KeyValuePair<string, object>("epcList", new[] { epc })
            , new KeyValuePair<string, object>("username", _userId)
            , new KeyValuePair<string, object>("deviceId", "0")
            , new KeyValuePair<string, object>("deviceIp", "0")
            , new KeyValuePair<string, object>("RegisterProperties", string.Empty))).Value;

    #endregion
}
