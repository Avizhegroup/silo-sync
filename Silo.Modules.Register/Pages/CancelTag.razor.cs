using System.Text.Json;
using System.Text.RegularExpressions;
using Silo.Application.Features;
using Silo.Modules.Register.Models;
using Silo.Shared.Components;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Cancels a registered tag (legacy F9). The tag is found by reading it (single-read mode) or by typing its serial or
/// EPC. The server decides whether the warehouse the tag is in allows cancelling (SCancelRegisterTagByDestinationAccess),
/// so the permission of a previously read tag can never leak into the next cancel (a legacy defect).
/// </summary>
public partial class CancelTag
{
    private static readonly Regex CodeRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private string _userId = string.Empty;

    public bool IsLoading { get; set; }
    public string Input { get; set; } = string.Empty;
    public GetRevokeTagsVm? Product { get; set; }
    public string Epc { get; set; } = string.Empty;
    public bool ConfirmPending { get; set; }
    public string Message { get; set; } = "Read a tag or type its serial or EPC.";
    public string MessageKind { get; set; } = "info";
    public List<GetRevokeTagsVm> ProductList => Product is null ? [] : [Product];

    public LocalWebSocketComponent? Socket { get; set; }
    public int DevicePort { get; set; } = 5005;
    public bool IsDeviceConnected { get; set; }

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        _userId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();
    }

    public async Task OnDeviceConnectionChanged(bool connected)
    {
        IsDeviceConnected = connected;

        if (connected)
        {
            await RestartReaderAsync();
        }

        StateHasChanged();
    }

    public void OnDeviceError(string message) => Notification.Show(message, "error");

    public async Task OnDeviceMessageReceived(string json)
    {
        DeviceMessage? message;

        try
        {
            message = JsonSerializer.Deserialize<DeviceMessage>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return;
        }

        if (message?.Type == "epc" && message.Value is not null && !IsLoading && !ConfirmPending)
        {
            var epc = message.Value.Trim();
            if (BarcodeParser.IsValidEpc(epc))
            {
                await LookupAsync(BarcodeParser.NormalizeEpc(epc));
                StateHasChanged();
            }
        }
        else if (message?.Type == "error")
        {
            Notification.Show(message.Value ?? string.Empty, "error");
        }
    }

    public async Task OnInputKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnSearchClick();
        }
    }

    public async Task OnSearchClick()
    {
        if (IsLoading || ConfirmPending)
        {
            return;
        }

        await LookupAsync(Input.Trim());
        Input = string.Empty;
    }

    private async Task LookupAsync(string serialOrEpc)
    {
        if (!CodeRegex.IsMatch(serialOrEpc))
        {
            SetMessage("Enter a serial or a 24-character EPC (letters and digits only).", "danger");
            return;
        }

        IsLoading = true;
        try
        {
            Product = null;
            Epc = string.Empty;

            var state = (await Api.PostAsync<List<TagStateInfo>>("SPGetProductInfo"
                , new KeyValuePair<string, object>("tagEpc", serialOrEpc))).Value?.FirstOrDefault();

            if (state is null || string.IsNullOrWhiteSpace(state.ProductSerial) || state.TagEpc == "-1")
            {
                SetMessage("This tag is not registered.", "danger");
                return;
            }

            Product = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial"
                , new GetRevokeTagsVmContext()
                , new KeyValuePair<string, object>("serial", state.ProductSerial))).Value?.FirstOrDefault();

            if (Product is null)
            {
                SetMessage("The product of this tag could not be loaded.", "danger");
                return;
            }

            Epc = state.TagEpc;
            SetMessage("Check the product, then confirm the cancellation.", "warning");
        }
        catch
        {
            Product = null;
            SetMessage(TextResources.APP_StringKeys_Alert_Fail, "danger");
        }
        finally
        {
            IsLoading = false;
        }

        // Legacy: the reader is stopped after a lookup; here it is restarted so the next tag can be read.
        await RestartReaderAsync();
    }

    public void OnCancelClick()
    {
        if (Product is not null && !IsLoading)
        {
            ConfirmPending = true;
        }
    }

    public void OnConfirmNo() => ConfirmPending = false;

    public async Task OnConfirmYes()
    {
        ConfirmPending = false;

        if (Product is null || !BarcodeParser.IsValidEpc(Epc) || IsLoading)
        {
            return;
        }

        IsLoading = true;
        try
        {
            // Empty string = cancelled; otherwise the server's reason (for example the warehouse does not allow cancelling).
            var reason = (await Api.PostAsync<string>("SCancelRegisterTagByDestinationAccess"
                , new KeyValuePair<string, object>("TagEpc", Epc)
                , new KeyValuePair<string, object>("username", _userId)
                , new KeyValuePair<string, object>("deviceId", "0")
                , new KeyValuePair<string, object>("deviceIp", "0"))).Value;

            if (string.IsNullOrWhiteSpace(reason))
            {
                SetMessage($"The registration of {Product.ProductSerial} was cancelled.", "success");
                Product = null;
                Epc = string.Empty;
            }
            else
            {
                SetMessage(reason, "danger");
            }
        }
        catch
        {
            SetMessage(TextResources.APP_StringKeys_Alert_Fail, "danger");
        }
        finally
        {
            IsLoading = false;
        }

        await RestartReaderAsync();
    }

    public void OnClearClick()
    {
        Product = null;
        Epc = string.Empty;
        ConfirmPending = false;
        SetMessage("Read a tag or type its serial or EPC.", "info");
    }

    private void SetMessage(string message, string kind)
    {
        Message = message;
        MessageKind = kind;
    }

    private async Task RestartReaderAsync()
    {
        if (!IsDeviceConnected || Socket is null)
        {
            return;
        }

        await Socket.SendCommandAsync(new { type = "stopReader" });
        await Socket.SendCommandAsync(new { type = "startReader", mode = 1 });
    }

    public record DeviceMessage(string Type, string? Value);
}
