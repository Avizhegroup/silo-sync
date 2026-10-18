using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Silo.Modules.Register.Models;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

public partial class ParentPackage
{
    private static readonly Regex CodeRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);

    public bool IsLoading { get; set; }
    public string Serial { get; set; } = string.Empty;
    public string PackageCode { get; set; } = string.Empty;
    public string PackageEpc { get; set; } = string.Empty;
    public List<PackageItem> Items { get; set; } = [];

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan ListTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PrintTimeout = TimeSpan.FromSeconds(60);
    private const int MaxListedSerials = 200;

    private readonly DeviceRpc _rpc = new();

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }
    public bool IsReading { get; set; }
    public bool IsPrinting { get; set; }
    public List<DeviceFormat> Formats { get; set; } = [];
    public List<string> Printers { get; set; } = [];
    public string SelectedFormatKey { get; set; } = string.Empty;
    public string SelectedPrinter { get; set; } = string.Empty;
    public DeviceFormat? SelectedFormat => Formats.FirstOrDefault(p => p.Key == SelectedFormatKey);
    public decimal TotalValue => Items.Sum(p => p.ProductCount);

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();
    }

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnAddClick();
        }
    }

    public async Task OnPackageCodeKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnLoadPackageClick();
        }
    }

    public async Task OnAddClick()
    {
        string serial = Serial.Trim();

        if (!CodeRegex.IsMatch(serial))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            var rows = (await Api.PostAsync<List<PackageItem>>("SPGetProductInfo"
                , new KeyValuePair<string, object>("tagEpc", serial))).Value;

            if (rows is null || rows.Count == 0)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
                return;
            }

            foreach (var row in rows.Where(r => Items.All(i => i.ProductSerial != r.ProductSerial)))
            {
                Items.Add(row);
            }

            Serial = string.Empty;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnLoadPackageClick()
    {
        string code = PackageCode.Trim();
        string epc = PackageEpc.Trim();

        if ((code.Length == 0 || code == "0") && epc.Length == 0)
        {
            return;
        }

        if ((code.Length > 0 && !CodeRegex.IsMatch(code)) || (epc.Length > 0 && !CodeRegex.IsMatch(epc)))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            var rows = await GetChildrenAsync(code == "0" ? string.Empty : code, epc);

            Items = rows;

            if (rows.Count > 0)
            {
                PackageCode = rows[0].TagTreeParentSerial;
                PackageEpc = rows[0].TagTreeParentsEpc;
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnSaveClick()
    {
        string epc = PackageEpc.Trim();

        if (epc.Length == 0 || Items.Count == 0)
        {
            return;
        }

        if (!CodeRegex.IsMatch(epc))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;

        try
        {
            bool ok = (await Api.PostAsync<bool>("SRegisterNextlevelProductUnit"
                , new KeyValuePair<string, object>("productUnitEpc", epc)
                , new KeyValuePair<string, object>("listEpc", Items.Select(i => i.ProductSerial).ToList())
                , new KeyValuePair<string, object>("userToken", UserId))).Value;

            if (ok)
            {
                var rows = await GetChildrenAsync(string.Empty, epc);
                PackageCode = rows.Count > 0 ? rows[0].TagTreeParentSerial : string.Empty;
            }
            else
            {
                PackageCode = "0";
            }

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnRemoveClick(PackageItem item)
    {
        IsLoading = true;

        try
        {
            bool ok = (await Api.PostAsync<bool>("SRegisterNextlevelProductUnit"
                , new KeyValuePair<string, object>("productUnitEpc", "-1")
                , new KeyValuePair<string, object>("listEpc", new List<string> { item.ProductSerial })
                , new KeyValuePair<string, object>("userToken", UserId))).Value;

            if (ok)
            {
                Items.Remove(item);
            }

            Notification.Show(ok ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail
                , ok ? "success" : "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void OnClearClick()
    {
        IsReading = false;
        Items = [];
        Serial = PackageCode = PackageEpc = string.Empty;
    }

    private string UserId { get; set; } = string.Empty;

    #region Device host (reader and label printer)

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;

        if (connected)
        {
            await RefreshDeviceAsync();
        }
        else
        {
            IsReading = false;
            _rpc.FailAll("The device host disconnected.");
            Formats = [];
            Printers = [];
        }

        StateHasChanged();
    }

    public void OnDeviceError(string message) => Notification.Show(message, "error");

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

        if (message?.Type == "epc" && IsReading && message.Value is not null)
        {
            await OnPackageEpcReadAsync(message.Value);
            StateHasChanged();
        }
        else if (message?.Type == "error")
        {
            Notification.Show(message.Value ?? string.Empty, "error");
        }
    }

    /// <summary>Legacy "1 - read tag": single read of the package's own tag.</summary>
    public async Task OnReadTagClick()
    {
        if (!IsDeviceConnected || Socket is null)
        {
            return;
        }

        IsReading = true;
        await Socket.SendCommandAsync(new { type = "stopReader" });
        await Socket.SendCommandAsync(new { type = "startReader", mode = 1 });
    }

    private async Task OnPackageEpcReadAsync(string value)
    {
        var epc = BarcodeParser.NormalizeEpc(value);
        if (!BarcodeParser.IsValidEpc(epc))
        {
            return;
        }

        IsReading = false;
        if (Socket is not null)
        {
            await Socket.SendCommandAsync(new { type = "stopReader" });
        }

        // Legacy: the grid is cleared and the children already assigned to this package tag are loaded.
        PackageEpc = epc;
        PackageCode = string.Empty;
        Items = [];

        IsLoading = true;
        try
        {
            Items = await GetChildrenAsync(string.Empty, epc);
            if (Items.Count > 0)
            {
                PackageCode = Items[0].TagTreeParentSerial;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

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
                SelectedFormatKey = (Formats.FirstOrDefault(p => p.Name.Contains("package", StringComparison.OrdinalIgnoreCase)
                    || p.Name.Contains("بسته", StringComparison.Ordinal)) ?? Formats.FirstOrDefault())?.Key ?? string.Empty;
            }
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException or JsonException)
        {
            Notification.Show(exception.Message, "error");
        }
    }

    /// <summary>
    /// Prints the package label through the device host. Legacy used a FastReport template (PrintPackFormat.frx, not in the
    /// repository); here any ZPL/ZMIN template of the host can use these tokens: [کد_بسته_بندی] package code,
    /// [شناسه_تگ] package EPC, [تعداد_اقلام] item count, [جمع_مقدار_اقلام] sum of values, [لیست_سریال_ها] "serial - title" lines.
    /// </summary>
    public async Task OnPrintLabelClick()
    {
        var format = SelectedFormat;
        var code = PackageCode.Trim();

        if (code is "" or "-1" or "0")
        {
            Notification.Show("Save the package first.", "warning");
            return;
        }

        if (format is null || !IsDeviceConnected || Socket is null || Items.Count == 0)
        {
            return;
        }

        // ZPL breaks lines inside a field block with \&; ZMIN text objects take a plain line break.
        var separator = format.Kind == "zpl" ? "\\&" : "\n";
        var lines = Items.Take(MaxListedSerials).Select(p => $"{p.ProductSerial} - {p.ProductName}").ToList();
        if (Items.Count > MaxListedSerials)
        {
            lines.Add($"... +{Items.Count - MaxListedSerials}");
        }

        var fields = new Dictionary<string, string>
        {
            ["کد_بسته_بندی"] = code,
            ["شناسه_تگ"] = PackageEpc.Trim(),
            ["تعداد_اقلام"] = Items.Count.ToString(CultureInfo.InvariantCulture),
            ["جمع_مقدار_اقلام"] = TotalValue.ToString("0.##", CultureInfo.InvariantCulture),
            ["لیست_سریال_ها"] = string.Join(separator, lines)
        };

        IsPrinting = true;
        try
        {
            var reply = await _rpc.RequestAsync(Socket, id => new
            {
                type = "printFormat",
                id,
                kind = format.Kind,
                format = format.Name,
                printer = SelectedPrinter,
                writeRfid = false,
                fields,
                technicalData = new Dictionary<string, string>()
            }, PrintTimeout);

            Notification.Show(reply.Ok ? TextResources.APP_StringKeys_Alert_Success : reply.Value ?? TextResources.APP_StringKeys_Alert_Fail
                , reply.Ok ? "success" : "error");
        }
        catch (Exception exception) when (exception is TimeoutException or InvalidOperationException)
        {
            Notification.Show(exception.Message, "error");
        }
        finally
        {
            IsPrinting = false;
        }
    }

    public record DeviceMessage(string Type, string? Value);

    #endregion

    private async Task<List<PackageItem>> GetChildrenAsync(string parentSerial, string parentEpc)
    {
        var rows = (await Api.PostAsync<List<PackageItem>>("SPGetProductListByParentSerial"
            , new KeyValuePair<string, object>("parentSerial", parentSerial)
            , new KeyValuePair<string, object>("parentEpc", parentEpc))).Value;

        return rows ?? [];
    }

    public class PackageItem
    {
        public string TagEpc { get; set; } = string.Empty;
        public string ProductSerial { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductCount { get; set; }
        public string TagTreeParentSerial { get; set; } = string.Empty;
        public string TagTreeParentsEpc { get; set; } = string.Empty;
    }
}
