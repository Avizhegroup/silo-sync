using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Silo.Pages.Account;
public partial class Edit
{
    public bool IsAdmin = false;
    public bool IsLoading = true;
    public UpdateUserByIdCommand User = new();
    public List<IdentityRole> Roles;
    public List<GetUserTokensDto> UserTokens = new();

    public Modal Modal { get; set; }

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
   
    [CascadingParameter] public DialogFactory Dialog { get; set; }

    [Parameter] public string Username { get; set; }

    protected override async Task SiloInitializer()
    {
        var roles = (await Api.PostAsync<List<IdentityRole>>("GetAllRoles",
                new KeyValuePair<string, object>[] { new("userToken", "") })).Value;

        Roles = roles.Where(p => p.Name.ToLower() != "shop").ToList();

        User = Mapper.Map<UpdateUserByIdCommand>((await Api.PostAsyncByContext<List<GetUserByUsernameVm>>("GetUserByUsername"
                   , new GetUserByUsernameVmContext()
                   , new("username", Username)
                   , new("userToken", "-1"))).Value.First());

        IsAdmin = User.Username.ToLower().Contains("admin");

        if (User.RoleName.ToLower().Contains("shop"))
        {
            NavigationManager.NavigateTo("/account/deadend", true);
        }

        await LoadUserTokens();

        IsLoading = false;
    }

    public async Task OnGenerateToken()
    {
        IsLoading = true;

        GenerateUserTokenCommand command = new ()
        {
            UserId = User.Id 
        };
        
        var response = await Api.SendAsyncObjectByUri<GenerateUserTokenVm>(HttpMethod.Post
            ,"Account/GenerateUserToken"
            ,command);

        if (response.Successful)
        {
            Notification.Show("توکن جدید با موفقیت ایجاد شد", "success");
        
            await LoadUserTokens();
        }
        else
        {
            Notification.Show("خطا در ایجاد توکن", "error");
        }

        IsLoading = false;
    }

    public async Task OnDeleteToken(int tokenId)
    {
        var dialogResult = await Dialog.ConfirmAsync(TextResources.APP_StringKeys_Message_Delete
            , TextResources.APP_StringKeys_Attention
            , TextResources.APP_StringKeys_Delete
            , TextResources.APP_StringKeys_Ignore);

        if (!dialogResult)
        {
            return;
        }

        IsLoading = true;

        var command = new DeleteUserTokenCommand { TokenId = tokenId };
       
        var response = await Api.SendAsyncObjectByUri<DeleteUserTokenVm>(HttpMethod.Delete,
            "Account/DeleteUserToken",
            command);

        if (response.Value.Result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            await LoadUserTokens();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Error_Unexpected, "error");
        }

        IsLoading = false;
    }

    public async Task OnCopyToken(string tokenValue)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", tokenValue);

            Notification.Show("متن کپی شد", "success");
        }
        catch
        {
        }
    }

    public async Task OnSubmit(MouseEventArgs e)
    {
        IsLoading = true;

        User.Details = JsonConvert.SerializeObject(new List<string>());

        bool result = (await Api.PostAsync<bool>("UpdateUserData",
                new KeyValuePair<string, object>("command", User))).Value;

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

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (User.Username.ToLower().Equals("admin"))
        {
            Notification.Show("امکان حذف کاربر ادمین وجود ندارد", "error");

            return;
        }

        var dialogResult = await Dialog.ConfirmAsync(TextResources.APP_StringKeys_Message_Delete
            , TextResources.APP_StringKeys_Attention
            , TextResources.APP_StringKeys_Delete
            , TextResources.APP_StringKeys_Ignore);

        if (!dialogResult)
        {
            return;
        }

        bool result = (await Api.PostAsync<bool>("SRemoveUser",
                        new KeyValuePair<string, object>[] { new("username", Username) })).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            NavigationManager.NavigateTo("/account/index");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    private async Task LoadUserTokens()
    {
        GetUserTokensQuery query = new ()
        { 
            UserId = User.Id 
        };

        var response = await Api.SendAsyncObjectByUri<GetUserTokensVm>(HttpMethod.Get,
            "Account/GetUserTokens",
            query);

        UserTokens = response.Value.Result;
    }
}
