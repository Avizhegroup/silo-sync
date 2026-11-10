
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

    public List<GetUserQuickAccessVm> QuickAccessLinks = new();
    public GetUserQuickAccessVm QuickAccessContextTarget;
    public List<TelerikContextMenuItem> QuickContextMenuItems = new()
    {
        new TelerikContextMenuItem { Text = "حذف از دسترسی سریع", Icon = MaterialIconsHelper.BookmarkRemove }
    };
    public string PieSeriesLabel = "#=category#\n #=value# %";

    public HashSet<string> AllowedDashboardItems = new(
        StringComparer.OrdinalIgnoreCase);

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

        if (isAdmin)
        {
            AllowedDashboardItems = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "BriefDaily",
                "BriefMonthly",
                "BriefYearly",
                "ProductDate",
                "ProductCode",
                "Shift",
                "ProductLine"
            };
        }
        else
        {
            var claims = await ClaimManager.GetUserClaims();

            AllowedDashboardItems = claims
                .Where(c => c.Type == "dashboard")
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        IsDashboardPermissionsLoaded = true;

        // دریافت اطلاعات KPI فقط در صورت داشتن مجوز حداقل یکی از کارت‌ها
        if (ShowAnyKpi)
        {
            var onBrief = (await Api
                .PostAsyncByContext<List<OnBrief>>(
                    "SReportOnBrief",
                    new GetOnBriefContext()))
                .Value?.FirstOrDefault();

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

        if (ShowProductCodeChart)
        {
            OnProducts = (await Api
                .PostAsyncByContext<List<GetTagStatsOnProductCodeVm>>(
                    "SReportOnProductCode",
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

        if (ShowProductDateChart)
        {
            OnDates = (await Api
                .PostAsyncByContext<List<GetTagStatsOnProductDateVm>>(
                    "SReportOnProductDate",
                    new GetOnProductDateContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        if (ShowShiftChart)
        {
            OnShifts = (await Api
                .PostAsyncByContext<List<GetTagsStatsOnShiftVm>>(
                    "SReportOnShift",
                    new GetOnShiftContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        if (ShowProductLineChart)
        {
            OnLines = (await Api
                .PostAsyncByContext<List<GetTagStatsOnLineVm>>(
                    "SReportOnProductLine",
                    new GetOnProductLineReportContext(),
                    new KeyValuePair<string, object>[]
                    {
                        new("search", defaultRequest)
                    }))
                .Value ?? new();
        }

        IsLoading = false;
    }

    // KPI permissions

    private bool ShowDailyCard =>
        AllowedDashboardItems.Contains("BriefDaily");

    private bool ShowMonthlyCard =>
        AllowedDashboardItems.Contains("BriefMonthly");

    private bool ShowYearlyCard =>
        AllowedDashboardItems.Contains("BriefYearly");

    private bool ShowAnyKpi =>
        ShowDailyCard || ShowMonthlyCard || ShowYearlyCard;

    private int VisibleKpiCount =>
        new[]
        {
            ShowDailyCard,
            ShowMonthlyCard,
            ShowYearlyCard
        }.Count(x => x);

    private string KpiColumnClass => VisibleKpiCount switch
    {
        1 => "col-md-12",
        2 => "col-md-6",
        _ => "col-md-4"
    };

    // Chart permissions

    private bool ShowProductDateChart =>
        AllowedDashboardItems.Contains("ProductDate");

    private bool ShowProductCodeChart =>
        AllowedDashboardItems.Contains("ProductCode");

    private bool ShowShiftChart =>
        AllowedDashboardItems.Contains("Shift");

    private bool ShowProductLineChart =>
        AllowedDashboardItems.Contains("ProductLine");

    private bool ShowRow1Charts =>
        ShowProductDateChart || ShowProductCodeChart;

    private bool ShowRow2Charts =>
        ShowShiftChart || ShowProductLineChart;

    public void Dispose()
    {
        if (SiloContext is not null)
        {
            SiloContext.QuickAccessChanged -= OnQuickAccessChange;
        }
    }
}
