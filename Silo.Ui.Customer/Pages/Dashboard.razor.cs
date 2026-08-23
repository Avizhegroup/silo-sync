using Silo.Application.Features;
using Silo.Identity.Client;

namespace Silo.Ui.Customer.Pages;
public partial class Dashboard
{
    public bool IsLoading = true;
    public string Company;
    public CustomerDashboardPageSection ActiveSection = CustomerDashboardPageSection.Main;

    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Company = Configuration["Settings:Company"];

        IsLoading = false;
    }

    public async Task OnLogoutClick()
    {
        IsLoading = true;

        await AuthStateProvider.SetUserLoggedOut();

        IsLoading = false;
    }
}
