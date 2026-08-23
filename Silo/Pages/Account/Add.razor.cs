using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Silo.Pages.Account;
public partial class Add
{
    public bool IsLoading = true;
    public List<IdentityRole> Roles;
    public AddAccountCommand User = new();
    public string ThisUserId;

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        var roles = (await Api.PostAsync<List<IdentityRole>>("GetAllRoles",
                new KeyValuePair<string, object>[] { new("userToken", "") })).Value;

        Roles = roles.Where(p => p.Name.ToLower() != "shop").ToList();

        ThisUserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = false;
    }

    public async Task OnSubmit()
    {
        if (CheckEmptiness())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }

        IsLoading = true;

        if ((await Api.PostAsync<Int64>("CheckFieldUniqueness",
                new("tableName", "tbl_User"),
                new("fieldName", "Username"),
                new("value", User.UserName),
                new("userToken", "Ceramic identified client"))).Value > 0)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Remote_Uniqueness
                , TextResources.APP_StringKeys_Username), "error");

            return;
        }

        var user = Mapper.Map<ApplicationUser>(User);

        user.CreatorCode = ThisUserId;

        var result = (await Api.PostAsync<string>("AddNewUserAndRole",
               new("username", user.UserName),
               new("password", user.Password),
               new("creatorUserId", user.CreatorCode)
               , new("isActive", true),
               new("persianName", user.Name),
               new("role", user.Role),
               new("detailsJson",
                   JsonConvert.SerializeObject(new List<string>())))).Value;

        IsLoading = false;

        if (result != "0")
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public bool CheckEmptiness()
    {
        if (User.Name.HasNoValue())
        {
            return true;
        }

        if (User.UserName.HasNoValue())
        {
            return true;
        }

        if (User.Password.HasNoValue())
        {
            return true;
        }

        if (User.Role.HasNoValue())
        {
            return true;
        }

        return false;
    }
}
