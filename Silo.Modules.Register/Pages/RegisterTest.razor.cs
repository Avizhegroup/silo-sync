using System.Text.Json;
using System.Text.RegularExpressions;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

public partial class RegisterTest
{
    /// <summary>The steps of the "how to use this page" panel.</summary>
    public string[] HowToSteps { get; } =
    [
        RegisterTexts.TestHowTo1,
        RegisterTexts.TestHowTo2,
        RegisterTexts.TestHowTo3
    ];

    private const int EpcLength = 24;
    private static readonly Regex CodeRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);

    public enum TestResultState
    {
        None,
        Match,
        Mismatch,
        TagNotRegistered
    }

    public bool IsLoading { get; set; }
    public string BarcodeInput { get; set; } = string.Empty;
    public string EpcInput { get; set; } = string.Empty;
    public TagInfo? LabelSide { get; set; }
    public TagInfo? TagSide { get; set; }
    public TestResultState ResultState { get; set; }
    public string ResultMessage { get; set; } = string.Empty;

    public List<ComparisonRow> ComparisonRows =>
    [
        new(TextResources.APP_StringKeys_ProductSerial, LabelSide?.ProductSerial, TagSide?.ProductSerial),
        new(TextResources.APP_StringKeys_ProductCode, LabelSide?.ProductCode, TagSide?.ProductCode),
        new(TextResources.APP_StringKeys_ProductName, LabelSide?.ProductName, TagSide?.ProductName),
        new(TextResources.APP_StringKeys_Product_Size, LabelSide?.ProductSize, TagSide?.ProductSize),
        new(TextResources.APP_StringKeys_Chart_Regcode, LabelSide?.RegCode, TagSide?.RegCode),
        new(TextResources.APP_StringKeys_Status, LabelSide?.ProductStatusTitle, TagSide?.ProductStatusTitle)
    ];

    public record ComparisonRow(string Field, string? Label, string? Tag);

    public record DeviceMessage(string Type, string? Value);

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;

        if (connected)
        {
            await Socket!.SendCommandAsync(new { type = "startReader", mode = 2 });
        }

        StateHasChanged();
    }

    public void OnDeviceError(string message)
    {
        Notification.Show(message, "error");
    }

    public async Task OnDeviceMessageReceived(string json)
    {
        DeviceMessage? message;

        try
        {
            message = JsonSerializer.Deserialize<DeviceMessage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return;
        }

        switch (message?.Type)
        {
            case "epc" when message.Value is not null:
                await OnTagEpcReceived(message.Value);
                break;
            case "barcode" when message.Value is not null:
                await OnBarcodeReceived(message.Value);
                break;
            case "error":
                Notification.Show(message.Value ?? string.Empty, "error");
                break;
        }
    }

    // Called for "barcode" messages from the Windows device host.
    public async Task OnBarcodeReceived(string serial)
    {
        serial = serial.Trim();

        if (!CodeRegex.IsMatch(serial))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        LabelSide = await GetInfoAsync(serial);

        if (LabelSide is null)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        BarcodeInput = string.Empty;
        Compare();
        StateHasChanged();
    }

    // Called for "epc" messages from the Windows device host.
    public async Task OnTagEpcReceived(string epc)
    {
        epc = epc.Trim();

        if (epc.Length != EpcLength || !CodeRegex.IsMatch(epc))
        {
            return;
        }

        TagSide = await GetInfoAsync(epc);

        if (TagSide is null)
        {
            ResultState = TestResultState.TagNotRegistered;
            ResultMessage = TextResources.APP_StringKeys_Tag_Not_Register;
        }
        else
        {
            Compare();
        }

        EpcInput = string.Empty;
        StateHasChanged();
    }

    public async Task OnBarcodeKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnBarcodeReceived(BarcodeInput);
        }
    }

    public async Task OnEpcKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnTagEpcReceived(EpcInput);
        }
    }

    public async Task OnResetClick()
    {
        BarcodeInput = EpcInput = string.Empty;
        LabelSide = TagSide = null;
        ResultState = TestResultState.None;
        ResultMessage = string.Empty;

        if (IsDeviceConnected && Socket is not null)
        {
            await Socket.SendCommandAsync(new { type = "stopReader" });
            await Socket.SendCommandAsync(new { type = "startReader", mode = 2 });
        }
    }

    private void Compare()
    {
        if (LabelSide is null || TagSide is null)
        {
            return;
        }

        bool same = LabelSide.ProductSerial == TagSide.ProductSerial;
        ResultState = same ? TestResultState.Match : TestResultState.Mismatch;
        ResultMessage = same ? RegisterTexts.TestMatch : RegisterTexts.TestMismatch;
    }

    private async Task<TagInfo?> GetInfoAsync(string serialOrEpc)
    {
        IsLoading = true;

        try
        {
            var rows = (await Api.PostAsync<List<TagInfo>>("SPGetProductInfo"
                , new KeyValuePair<string, object>("tagEpc", serialOrEpc))).Value;

            return rows?.FirstOrDefault();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public class TagInfo
    {
        public string TagEpc { get; set; } = string.Empty;
        public string ProductSerial { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSize { get; set; } = string.Empty;
        public string RegCode { get; set; } = string.Empty;
        public string ProductStatusTitle { get; set; } = string.Empty;
    }
}
