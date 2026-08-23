using Microsoft.AspNetCore.Identity;
using Silo.Application.Features;
using Silo.Identity.Client;

namespace Silo.Ui.Customer.Components.Dashboard;

public partial class Profile
{
    [Parameter] public bool IsComponentShown { get; set; } = false;

    public bool IsLoading = true;
    public GetUserProfileVm Request = new();
    public List<IdentityRole> Roles;
    public bool IsPasswordHide = true;
    public bool IsNewPasswordHide = true;
    public bool IsReNewPasswordHide = true;
    internal bool mustCheckAccess = false;

    [Inject] public ISiloApiClient Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthState { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var state = (await AuthState.GetAuthenticationStateAsync()).User;

        Request.Username = state.GetUsername();

        Request.PersianName = state.GetUserPersianName();

        Request.Role = state.GetUserRoleName();

        Request.Image = state.GetUserImage();

        Roles = (await Api.PostAsync<List<IdentityRole>>("GetAllRoles",
                new KeyValuePair<string, object>[] { new("userToken", "") })).Value;

        IsLoading = false;
    }

    public async Task OnValidSubmit()
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("Account/PostObject"
            , "IUpdateProfile"
            , new KeyValuePair<string, object>("profile", new UpdateUserProfileCommand()
            {
                NewPassword = Request.PasswordNew,
                Password = Request.Password
            }))).Value;

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

    public async Task OnClearClick()
    {
        Request = new()
        {
            Username = Request.Username,
            PersianName = Request.PersianName,
            Role = Request.Role
        };
    }


}
