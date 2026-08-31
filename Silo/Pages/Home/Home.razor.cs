namespace Silo.Pages.Home;

public partial class Home
{
    public bool IsLoading = true;
    public bool IsDashboardShown = false;
    public decimal BriefDaily = 0;
    public decimal BriefMonthly = 0;
    public decimal BriefYearly = 0;
    public List<GetTagStatsOnProductCodeVm> OnProducts;
    public List<GetTagStatsOnProductDateVm> OnDates;
    public List<GetTagsStatsOnShiftVm> OnShifts;
    public List<GetTagStatsOnLineVm> OnLines;
    public List<GetUserQuickAccessVm> QuickAccessLinks = new();
    public GetUserQuickAccessVm QuickAccessContextTarget;
    public List<TelerikContextMenuItem> QuickContextMenuItems = new()
    {
        new TelerikContextMenuItem { Text = "حذف از دسترسی سریع", Icon = MaterialIconsHelper.BookmarkRemove }
    };
    public string PieSeriesLabel = "#=category#\n #=value# %";

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    public TelerikContextMenu<TelerikContextMenuItem> QuickAccessContextMenuRef { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsDashboardShown = bool.Parse(Configuration["DashboardShown"].ToString());

        if (IsDashboardShown)
        {
            QuickAccessLinks = await ClaimManager.GetQuickAccessLinks();

            await LoadDashboardData();
        }
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

        var onBrief = (await Api
            .PostAsyncByContext<List<OnBrief>>("SReportOnBrief", new GetOnBriefContext())).Value?.FirstOrDefault();

        if (onBrief is not null)
        {
            BriefDaily = onBrief.Today;
            BriefMonthly = onBrief.Monthly;
            BriefYearly = onBrief.Yearly;
        }

        var defaultRequest = new GetTagStatsQuery
        {
            FromDate = PersianCalendarTools.GregorianToPersian(DateTime.Now.AddMonths(-1)),
            ToDate = PersianCalendarTools.GregorianToPersian(DateTime.Now),
            Pl = "-1",
            Shift = "-1",
            Regcode = "-1"
        };

        OnProducts = (await Api
            .PostAsyncByContext<List<GetTagStatsOnProductCodeVm>>("SReportOnProductCode", new GetOnProductReportContext(),
                new KeyValuePair<string, object>[] { new("search", defaultRequest) })).Value;

        OnProducts = OnProducts.OrderByDescending(p => p.Percent).Take(5).ToList();

        OnDates = (await Api
            .PostAsyncByContext<List<GetTagStatsOnProductDateVm>>("SReportOnProductDate", new GetOnProductDateContext(),
                new KeyValuePair<string, object>[] { new("search", defaultRequest) })).Value;

        OnShifts = (await Api
            .PostAsyncByContext<List<GetTagsStatsOnShiftVm>>("SReportOnShift", new GetOnShiftContext(),
                new KeyValuePair<string, object>[] { new("search", defaultRequest) })).Value;

        OnLines = (await Api
            .PostAsyncByContext<List<GetTagStatsOnLineVm>>("SReportOnProductLine", new GetOnProductLineReportContext(),
                new KeyValuePair<string, object>[] { new("search", defaultRequest) })).Value;

        IsLoading = false;
    }
}
