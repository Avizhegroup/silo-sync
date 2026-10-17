using Silo.Application.Features;

namespace Silo.Modules.Register.Pages;

public partial class RegisterCancel
{
    private string Serial = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private bool Succeeded;
    private GetRevokeTagsVm? Tag;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private async Task LoadTag()
    {
        Tag = null;
        Message = string.Empty;

        if (string.IsNullOrWhiteSpace(Serial))
        {
            Message = TextResources.APP_StringKeys_Required;
            return;
        }

        IsLoading = true;
        try
        {
            var result = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial",
                new GetRevokeTagsVmContext()
                , new KeyValuePair<string, object>("serial", Serial.Trim()))).Value;

            Tag = result?.FirstOrDefault();
            if (Tag is null)
            {
                Message = TextResources.APP_StringKeys_Validation_NotFound;
            }
            else
            {
            }
        }
        catch
        {
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CancelRegistration()
    {
        if (Tag is null || string.IsNullOrWhiteSpace(Tag.TagEpc))
        {
            return;
        }

        IsLoading = true;
        try
        {
            Succeeded = (await Api.PostAsync<bool>("SCancelRegisterTag",
                new("TagEpc", Tag.TagEpc),
                new("username", (await AuthState.GetAuthenticationStateAsync()).User.Identity?.Name ?? ""),
                new("deviceId", "0"),
                new("deviceIp", "0"))).Value;

            Message = Succeeded ? TextResources.APP_StringKeys_Alert_Success : TextResources.APP_StringKeys_Alert_Fail;
            if (Succeeded)
            {
                Tag = null;
                Serial = string.Empty;
            }
        }
        catch
        {
            Succeeded = false;
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
