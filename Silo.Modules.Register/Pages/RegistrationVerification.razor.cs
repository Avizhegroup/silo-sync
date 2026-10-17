namespace Silo.Modules.Register.Pages;

public partial class RegistrationVerification
{
    private string Serial = string.Empty;
    private string Epc = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private bool Succeeded;
    private GetRevokeTagsVm? Tag;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private async Task Verify()
    {
        Tag = null;
        Message = string.Empty;
        Succeeded = false;
        if (string.IsNullOrWhiteSpace(Serial) || string.IsNullOrWhiteSpace(Epc))
        {
            Message = TextResources.APP_StringKeys_Required;
            return;
        }

        IsLoading = true;
        try
        {
            Tag = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial",
                new GetRevokeTagsVmContext(), new("serial", Serial.Trim()))).Value?.FirstOrDefault();

            if (Tag is null)
            {
                Message = TextResources.APP_StringKeys_Validation_NotFound;
                return;
            }

            Succeeded = string.Equals(Tag.TagEpc?.Trim(), Epc.Trim(), StringComparison.OrdinalIgnoreCase);
            Message = Succeeded
                ? TextResources.APP_StringKeys_Alert_Success
                : TextResources.APP_StringKeys_Alert_Fail;
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
}
