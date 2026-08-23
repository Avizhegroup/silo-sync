using Microsoft.Extensions.Configuration;
using Silo.Application.Features;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Product.Pages;
public partial class EditProductCode
{
    public bool IsLoading = false;
    public bool IsFromProductCodeModalFor = true;
    public UpdateProductCodeCommand Request = new();

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ApproveModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    protected override async Task SiloInitializer()
    {
    }

    public async Task OnValidSubmit()
    {
        IsLoading = true;

        Request.FromProductCodeCount = (await Api.PostAsync<int>("SCountProductCode",
            new KeyValuePair<string, object>("productCode", Request.FromProductCode) )).Value;

        IsLoading = false;

        await ApproveModal.Open(new());
    }

    public async Task ApproveUpdateProductCode()
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SUpdateProductCodeForSendToApi",
                 new("request", new SearchApiSyncSto()
                 {
                     ProductCode = Request.FromProductCode,
                 }),
                 new("newCode", Request.ToProductCode) )).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnOpenModalProductCode(bool isFromProductCodeModalFor)
    {
        IsFromProductCodeModalFor = isFromProductCodeModalFor;

        await ProductCodeModal.Show();
    }

    public async Task OnClickFromProductCode(string code)
    {
        if (IsFromProductCodeModalFor)
        {
            Request.FromProductCode = code;
        }
        else
        {
            Request.ToProductCode = code;
        }
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();
    }
}
