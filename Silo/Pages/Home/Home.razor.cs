
using Silo.Application.Shared.Features;

namespace Silo.Pages.Home;

public partial class Home : IDisposable
{
    public bool IsLoading = true;
    public bool IsDashboardShown = false;
    public bool IsDashboardPermissionsLoaded = false;

    public decimal BriefDaily = 0;
    public decimal BriefMonthly = 0;
    public decimal BriefYearly = 0;

    public List<GetTagStatsOnProductCodeVm> OnProducts = new();
    public List<GetTagStatsOnProductDateVm> OnDates = new();
    public List<GetTagsStatsOnShiftVm> OnShifts = new();
    public List<GetTagStatsOnLineVm> OnLines = new();
    public List<GetDashboardItemsVm> DashboardItems { get; set; } = new();
    public List<GetUserQuickAccessVm> QuickAccessLinks = new();
    public GetUserQuickAccessVm QuickAccessContextTarget;
    public List<TelerikContextMenuItem> QuickContextMenuItems = new()
    {
        new TelerikContextMenuItem { Text = "حذف از دسترسی سریع", Icon = MaterialIconsHelper.BookmarkRemove }
    };
    public string PieSeriesLabel = "#=category#\n #=value# %";

    public int VisibleKpiCount => DashboardItems.Count(x => x.ItemType == "Card");
    public bool ShowRow1Charts => HasDashboardAccess("ProductDate") || HasDashboardAccess("ProductCode");
    public bool ShowRow2Charts => HasDashboardAccess("Shift") || HasDashboardAccess("ProductLine");
    public bool ShowAnyKpi => VisibleKpiCount > 0;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }

    [CascadingParameter] public SiloComponentsContext SiloContext { get; set; }

    public TelerikContextMenu<TelerikContextMenuItem> QuickAccessContextMenuRef { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsDashboardShown = bool.Parse(Configuration["DashboardShown"].ToString());

        QuickAccessLinks = await ClaimManager.GetQuickAccessLinks();

        SiloContext.QuickAccessChanged += OnQuickAccessChange;

        if (IsDashboardShown)
        {
            await LoadDashboardData();
        }

        IsLoading = false;
    }

    public async Task OnQuickAccessChange()
    {
        QuickAccessLinks = await ClaimManager.GetQuickAccessLinks();

        await InvokeAsync(StateHasChanged);
    }

    public async Task OnQuickAccessRightClick(GetUserQuickAccessVm item, MouseEventArgs e)
    {
        QuickAccessContextTarget = item;

        if (QuickAccessContextMenuRef is not null)
        {
            await QuickAccessContextMenuRef.ShowAsync(e.ClientX, e.ClientY);
        }
    }

    public async Task OnQuickAccessContextMenuItemClick(TelerikContextMenuItem item)
    {
        if (QuickAccessContextTarget is null) return;

        await ClaimManager.RemoveQuickAccessLink(QuickAccessContextTarget.Id);

        QuickAccessLinks = await ClaimManager.GetQuickAccessLinks();

        QuickAccessContextTarget = null;

        StateHasChanged();
    }

    private async Task LoadDashboardData()
    {
        IsLoading = true;

        var isAdmin = await ClaimManager.IsUserAdmin();

        var allDashboardItems = (await Api.SendAsyncObjectByUri<List<GetDashboardItemsVm>>(HttpMethod.Get, "Dashboard/GetDashboardItems")).Value;

        if (isAdmin)
        {
            DashboardItems = allDashboardItems;
        }

        else
        {
            var claims = await ClaimManager.GetUserClaims();

            var allowedCodes = claims
                .Where(c => c.Type == "dashboard")
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            DashboardItems = allDashboardItems
                .Where(item => allowedCodes.Contains(item.ItemCode))
                .ToList();
        }

        IsDashboardPermissionsLoaded = true;

        if (ShowAnyKpi)
        {
            var onBrief = (await Api.PostAsyncByContext<List<OnBrief>>("SReportOnBrief", new GetOnBriefContext())).Value?.FirstOrDefault();

            if (onBrief is not null)
            {
                BriefDaily = onBrief.Today;
                BriefMonthly = onBrief.Monthly;
                BriefYearly = onBrief.Yearly;
            }
        }

        var defaultRequest = new GetTagStatsQuery
        {
            FromDate = PersianCalendarTools.GregorianToPersian(DateTime.Now.AddMonths(-1)),
            ToDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
            Pl = "-1",
            Shift = "-1",
            Regcode = "-1"
        };

        if (HasDashboardAccess("ProductCode"))
        {
            OnProducts = (await Api.PostAsyncByContext<List<GetTagStatsOnProductCodeVm>>("SReportOnProductCode",
                  new GetOnProductReportContext(),
                  new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();

            OnProducts = OnProducts
                .OrderByDescending(p => p.Percent)
                .Take(5)
                .ToList();
        }

        if (HasDashboardAccess("ProductDate"))
        {
            OnDates = (await Api.PostAsyncByContext<List<GetTagStatsOnProductDateVm>>("SReportOnProductDate",
                    new GetOnProductDateContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        if (HasDashboardAccess("Shift"))
        {
            OnShifts = (await Api.PostAsyncByContext<List<GetTagsStatsOnShiftVm>>("SReportOnShift",
                    new GetOnShiftContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        if (HasDashboardAccess("ProductLine"))
        {
            OnLines = (await Api.PostAsyncByContext<List<GetTagStatsOnLineVm>>("SReportOnProductLine",
                    new GetOnProductLineReportContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        IsLoading = false;
    }

    public void Dispose()
    {
        if (SiloContext is not null)
        {
            SiloContext.QuickAccessChanged -= OnQuickAccessChange;
        }
    }

    private string KpiColumnClass => VisibleKpiCount switch
    {
        1 => "col-md-12",
        2 => "col-md-6",
        _ => "col-md-4"
    };

    private bool HasDashboardAccess(string itemCode)
    {
        return DashboardItems.Any(x => x.ItemCode.Equals(itemCode, StringComparison.OrdinalIgnoreCase));
    }
}
