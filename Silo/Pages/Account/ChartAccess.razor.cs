using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Silo.Application.Shared.Features;

namespace Silo.Pages.Account;

public partial class ChartAccess
{
    public bool IsLoading = true;
    public bool IsEditable = true;
    public bool IsBulkMode = false;
    public string ChartAccessTitle = string.Empty;
    public string ThisUserId = string.Empty;
    public List<DashboardAccessItemVm> AvailableItems = new();
    public List<IdentityRole> Roles = new();

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }

    [Parameter] public string? AccountId { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        if (AccountId.HasValue())
        {
            ApplicationUser user = (await Api.PostAsync<ApplicationUser[]>("GetUserByUsername",
                    new("username", AccountId),
                    new("userToken", "-1"))).Value.First();

            if (user.RoleName.ToLower().Contains("shop"))
            {
                NavigationManager.NavigateTo("/account/deadend", true);
            }

            ChartAccessTitle = string.Format(TextResources.APP_StringKeys_Claim_AddTo_User, user.Name);

            AccountId = user.Id;

            ThisUserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

            Roles = (await Api.PostAsync<List<IdentityRole>>("GetUserRoles",
                new("userId", AccountId),
                new("userToken", ThisUserId))).Value;

            var claims = (await Api.PostAsync<List<ClaimVm>>("GetUserClaims",
                new("userId", AccountId),
                new("userToken", ThisUserId))).Value;

            var dashboardClaims = claims
                .Where(c => c.Type == "dashboard")
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            IsEditable = !Roles.Any(p => p.Name.ToLower() == "admin");

            var items = (await Api.SendAsyncObjectByUri<List<GetDashboardItemsVm>>(HttpMethod.Get, "Dashboard/GetDashboardItems")).Value;

            AvailableItems = items.Select(item => new DashboardAccessItemVm
            {
                ItemCode = item.ItemCode,
                ItemTitle = item.ItemTitle,
                ItemType = item.ItemType,
                HasAccess = dashboardClaims.Contains(item.ItemCode)
            }).ToList();
        }

        else
        {
            NavigationManager.NavigateTo("/account/index", true);
        }

        IsLoading = false;
    }

    public async Task OnSaveClick(MouseEventArgs e)
    {
        if (!IsEditable)
        {
            return;
        }

        var selectedCharts = AvailableItems
            .Where(p => p.HasAccess)
            .Select(p => new Silo.Application.Dto.Claim
            {
                Value = p.ItemCode,
                Type = "dashboard"
            })
            .ToList();

        IsLoading = true;

        var result = await Api.SendAsyncObjectByUri<AddBulkUserClaimsVm>(HttpMethod.Post,"Account/AddBulkUserClaims",
            new AddBulkUserClaimsCommand
            {
                UserIds = new() { AccountId },
                Claims = selectedCharts,
                ReplaceClaimTypes = new() { "dashboard" }
            });

        Notification.Show( result?.Successful == true
                ? TextResources.APP_StringKeys_Alert_Success
                : TextResources.APP_StringKeys_Alert_Fail,
            result?.Successful == true ? "success" : "error");

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        AvailableItems.ForEach(p => p.HasAccess = false);
    }

    private void OnRowRender(GridRowRenderEventArgs args)
    {
        var item = (DashboardAccessItemVm)args.Item;

        args.Class = item.HasAccess ? "access-row-active" : "";
    }
}
