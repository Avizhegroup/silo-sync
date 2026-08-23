using Silo.Application.Features;

namespace Silo.Pages.Account;

public partial class Index
{
    public bool IsLoading = true;
    public List<GetAllUsersVm> Users;
    public List<GetAllUsersVm> ShownUsers;
    public string SearchText;

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Users = (await Api.PostAsyncByContext<List<GetAllUsersVm>>("GetAllUser"
            , new GetAllUsersVmContext()
            , new KeyValuePair<string, object>("userToken", "Ceramic client user"))).Value;

        ShownUsers = Users;

        IsLoading = false;
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (SearchText.HasNoValue())
        {
            ShownUsers = Users;

            return;
        }

        ShownUsers = Users.Where(p=>p.Username.Contains(SearchText) || p.Name.Contains(SearchText))
                          .ToList();
    }
}
