using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Silo.Pages.Account;

public partial class Access
{
    public bool IsLoading = true;
    public bool IsDialogVisible = false;
    public bool IsEditable = true;
    public bool IsBulkMode = false;
    public string Title = string.Empty;
    public string ThisUserId = string.Empty;
    public string DialogMessage = string.Empty;
    public List<ChoosableKeyValue> AndroidPages;
    public List<ChoosableKeyValue> WebPages;
    public List<ChoosableKeyValue> WebAccesses;
    public List<ChoosableKeyValue> AndroidAccesses;
    public List<ChoosableKeyValue> AllUsers = new();
    public List<IdentityRole> Roles = new();

    public Modal ModalMessage { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }

    [Parameter] public string? AccountId { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        var links = (await Api.PostAsyncByContext<List<GetAllMenuLinksVm>>("SGetAllMenuLinks", new GetAllMenuLinksVmContext())).Value;

        WebPages = links
            .Where(p => p.Level == 3 && p.IsShown)
            .Select(p => new ChoosableKeyValue()
            {
                Key = p.Url,
                Value = p.Title
            }).ToList();

        AndroidPages = links
            .Where(p => p.Level == 10 && p.IsShown)
            .Select(p => new ChoosableKeyValue()
            {
                Key = p.Url,
                Value = p.Title
            }).ToList();

        if (AccountId.HasValue())
        {
            ApplicationUser user = (await Api.PostAsync<ApplicationUser[]>("GetUserByUsername",
                    new("username", AccountId),
                    new("userToken", "-1"))).Value.First();

            if (user.RoleName.ToLower().Contains("shop"))
            {
                NavigationManager.NavigateTo("/account/deadend", true);
            }

            Title = string.Format(TextResources.APP_StringKeys_Claim_AddTo_User, user.Name);

            AccountId = user.Id;

            ThisUserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

            Roles = (await Api.PostAsync<List<IdentityRole>>("GetUserRoles",
                new("userId", AccountId),
                new("userToken", ThisUserId))).Value;

            List<ClaimVm> claims = (await Api.PostAsync<List<ClaimVm>>("GetUserClaims"
                , new("userId", AccountId)
                , new("userToken", ThisUserId))).Value;

            foreach (var view in WebPages)
            {
                foreach (var claim in claims)
                {
                    if (!claim.Value.Contains(".cshtml"))
                    {
                        claim.Value = $"/Views{claim.Value}.cshtml";
                    }

                    if (claim.Type == ClaimTypes.Authentication
                        && claim.Value.ToLower().Equals($"/views{view.Key.ToLower()}.cshtml"))
                    {
                        view.IsChoosed = true;
                    }
                }
            }

            foreach (var view in AndroidPages)
            {
                foreach (var claim in claims)
                {
                    if (!claim.Value.Contains(".cshtml"))
                    {
                        claim.Value = $"/Views{claim.Value}.cshtml";
                    }

                    if (claim.Type == ClaimTypes.AuthenticationInstant)
                    {
                        if (claim.Value.ToLower().Equals($"/views{view.Key.ToLower()}.cshtml"))
                        {
                            view.IsChoosed = true;
                        }
                    }
                }
            }

            WebAccesses = WebPages.OrderBy(p => p.Value).ToList();

            AndroidAccesses = AndroidPages.OrderBy(p => p.Value).ToList();

            IsEditable = !Roles.Any(p => p.Name.ToLower() == "admin");
        }
        else
        {
            IsBulkMode = true;

            IsEditable = true;

            Title = TextResources.APP_StringKeys_Claim_ListViews;

            ThisUserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

            var users = (await Api.PostAsyncByContext<List<GetAllUsersVm>>("GetAllUser",
                new GetAllUsersVmContext(),
                new KeyValuePair<string, object>("userToken", "Ceramic client user"))).Value;


            users = users.Where(p => p.IsActive).ToList();
            //AllUsers = users
            //    .Where(p => p.IsActive)
            //    .Select(p => new ChoosableKeyValue()
            //    {
            //        Key = p.Id.ToString(),
            //        Value = p.Name,
            //        IsEditable = true
            //    }).ToList();

            foreach (var user in users)
            {
                var roles = (await Api.PostAsync<List<IdentityRole>>("GetUserRoles",
                                                                 new("userId", user.Id),
                                                                 new("userToken", ThisUserId))).Value;

                var isEditable = !roles.Any(p => p.Name.ToLower() == "admin");

                AllUsers.Add(new()
                {
                    Key = user.Id.ToString(),
                    Value = user.Name,
                    IsEditable = isEditable,
                    IsChoosed = false
                });
            }

            WebAccesses = WebPages.OrderBy(p => p.Value).ToList();

            AndroidAccesses = AndroidPages.OrderBy(p => p.Value).ToList();
        }

        IsLoading = false;
    }

    public async Task OnWebSaveClick(MouseEventArgs e)
    {
        if (!IsEditable)
        {
            return;
        }

        var userIds = IsBulkMode
            ? AllUsers.Where(p => p.IsChoosed).Select(p => p.Key).ToList()
            : new() { AccountId };

        var claims = WebPages.Where(p => p.IsChoosed)
            .Select(p => new Silo.Application.Dto.Claim { Value = p.Key, Type = ClaimTypes.Authentication })
            .Concat(AndroidPages.Where(p => p.IsChoosed)
            .Select(p => new Silo.Application.Dto.Claim { Value = p.Key, Type = ClaimTypes.AuthenticationInstant }))
            .ToList();

        if (IsBulkMode && userIds.Neither())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ChooseUser, "error");

            return;
        }

        if (claims.Neither())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        IsLoading = true;

        var result = await Api.SendAsyncObjectByUri<AddBulkUserClaimsVm>(
            HttpMethod.Post,
            "Account/AddBulkUserClaims",
            new AddBulkUserClaimsCommand
            {
                UserIds = userIds,
                Claims = claims
            });

        Notification.Show(result?.Successful == true
            ? TextResources.APP_StringKeys_Alert_Success
            : TextResources.APP_StringKeys_Alert_Fail,
            result?.Successful == true ? "success" : "error");

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        AndroidAccesses.ForEach(p => p.IsChoosed = false);

        WebAccesses.ForEach(p => p.IsChoosed = false);

        AllUsers.ForEach(p => p.IsChoosed = false);
    }
}
