using Silo.Application;

namespace Silo.Pages.Settings;

public partial class ManageCache
{
    public bool IsLoading = false;

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    [Inject] public IFormalDataCache FormalCache { get; set; }

    public async Task OnHardRefreshClick()
    {
        var confirmed = await Dialog.ConfirmAsync(
            TextResources.APP_StringKeys_Message_Delete,
            TextResources.APP_StringKeys_Attention,
            TextResources.APP_StringKeys_Confirm,
            TextResources.APP_StringKeys_Disconfirm);

        if (!confirmed)
        {
            return;
        }

        IsLoading = true;

        try
        {
            await FormalCache.HardRefreshCache();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
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
}
