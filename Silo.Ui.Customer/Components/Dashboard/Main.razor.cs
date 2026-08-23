using Silo.Application.Features;
using Silo.Identity.Client;

namespace Silo.Ui.Customer.Components.Dashboard;
public partial class Main
{
    public GetSalesShopByShopCodeVm? SalesShop;
    public GetSalesInstallerByCodeVm? SalesInstaller;

    [Parameter] public bool IsComponentShown { get; set; } = false;

    [Inject] public ISiloApiClient Api { get; set; } = default!;
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        string userName = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        SalesShop = (await Api.PostAsync<GetSalesShopByShopCodeVm>("SGetSalesShopByShopCode"
            , new KeyValuePair<string, object>("shopCode", userName)
            )).Value;

        SalesInstaller = (await Api.PostAsync<GetSalesInstallerByCodeVm>("SGetSalesInstallerByCode"
            , new KeyValuePair<string, object>("installerCode", userName)
            )).Value;
    }
}
