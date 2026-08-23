using Silo.Shared.Components;

namespace Silo.Pages.Reports;
public partial class RevokeAction
{
    public bool IsLoading = false;
    public string Serial = string.Empty;
    public GetRevokeTagsVm Product ;

    [Inject] public RfidConnectApi Api { get; set; }

    public Modal ErrorModal { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;
    }

    public async Task OnAddSerialClick(MouseEventArgs e)
    {
        await GetProduct(Serial);
    }

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await GetProduct(Serial);
        }
    }

    public async Task OnHandHeldTagReadClick(MouseEventArgs e)
    {
        IsLoading = true;

        var result = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetLastReadedTagsForRevoke"
            , new GetRevokeTagsVmContext())).Value;

        if (result is null)
        {
            IsLoading = false;

            return;
        }

        if (result.Any())
        {
            Product = result.First();
        }

        Serial = string.Empty;

        IsLoading = false;
    }

    public async Task OnModalOk(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SCancelRegisterTag"
            , new("TagEpc", Product.TagEpc)
            , new("username", "")
            , new("deviceId", "")
            , new("deviceIp", ""))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Product = null;
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "error");
        }

        IsLoading = false;
    }

    private async Task GetProduct(string serial)
    {
        if (Serial.HasNoValue())
        {
            return;
        }

        IsLoading = true;

        var products = (await Api.PostAsyncByContext<List<GetRevokeTagsVm>>("SGetProductBySerial"
             , new GetRevokeTagsVmContext()
             , new KeyValuePair<string, object>("serial", serial))).Value;

        if (products.Any())
        {
            Product = products.First();
        }

        Serial = string.Empty;

        IsLoading = false;
    }
}
