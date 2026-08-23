using Microsoft.AspNetCore.Identity;
using Silo.Application.Features;

namespace Silo.Pages.Account;

public partial class Profile
{
    public bool IsLoading = true;
    public GetUserProfileVm Request = new();
    public List<IdentityRole> Roles;
    public bool IsPasswordHide = true;
    public bool IsNewPasswordHide = true;
    public bool IsReNewPasswordHide = true;
    internal new bool mustCheckAccess = false;

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        var state = (await AuthState.GetAuthenticationStateAsync()).User;

        Request.Username = state.GetUsername();

        Request.PersianName = state.GetUserPersianName();

        Request.Role = state.GetUserRoleName();
        
        Request.Image = state.GetUserImage();

        var roles = (await Api.PostAsync<List<IdentityRole>>("GetAllRoles",
                new KeyValuePair<string, object>[] { new("userToken", "") })).Value;

        Roles = roles.Where(p => p.Name.ToLower() != "shop").ToList();

        IsLoading = false;
    }

    public async Task OnValidSubmit()
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("account/PostObject"
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
        Request = new();
    }

    public async Task OnUploadImageComplete(string fileName)
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("account/PostObject"
           , "IUpdateProfile"
           , new KeyValuePair<string, object>("profile", new UpdateUserProfileCommand()
           {
               Image = fileName
           }))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Request.Image = fileName;
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }
}
