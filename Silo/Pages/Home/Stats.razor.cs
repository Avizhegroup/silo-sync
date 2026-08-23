using Silo.Application.Features;

namespace Silo.Pages.Home;

public partial class Stats
{
    public bool IsLoading = true;
    public GetTagStatsQuery Request = new()
    {
        FromDate = PersianCalendarTools.GregorianToPersian(DateTime.Now.AddMonths(-1)),
        ToDate = PersianCalendarTools.GregorianToPersian(DateTime.Now)
    };
    public decimal BriefDaily = 0;
    public decimal BriefMonthly = 0;
    public decimal BriefYearly = 0;
    public List<GetTagStatsOnProductCodeVm> OnProducts;
    public List<GetTagStatsOnRegcodeVm> OnRegcodes;
    public List<GetStatsOnQcVm> OnQcs;
    public List<GetTagStatsOnLineVm> OnLines;
    public List<GetTagsStatsOnShiftVm> OnShifts;
    public List<GetTagStatsOnProductDateVm> OnDates;
    public List<GetTagStatsOnDetailsVm> OnDetails;
    public string seriesLabelTamplate = "#=category#\n #=value# %";
    public string seriesLabelTamplateNoPercent = "#=category#\n #=value#";
    public ChartSeriesLabelsPosition seriesLabelPosition = ChartSeriesLabelsPosition.Right;

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        await OnGetDataClick(new());
    }

    public async Task OnGetDataClick(MouseEventArgs e)
    {
        IsLoading = true;

        GetTagStatsQuery request = FixEmptiness();

        var onBrief = (await Api
                .PostAsyncByContext<List<OnBrief>>("SReportOnBrief", new GetOnBriefContext())).Value.First();

        BriefDaily = onBrief.Today;
        BriefMonthly = onBrief.Monthly;
        BriefYearly = onBrief.Yearly;

        OnProducts = (await Api
            .PostAsyncByContext<List<GetTagStatsOnProductCodeVm>>("SReportOnProductCode", new GetOnProductReportContext(),
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnRegcodes = (await Api
            .PostAsyncByContext<List<GetTagStatsOnRegcodeVm>>("SReportOnRegcode", new GetOnRegcodeReportContext(),
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnQcs = (await Api
             .PostAsyncByContext<List<GetStatsOnQcVm>>("SReportOnQc", new GetOnQcReportContext(),
                 new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnLines = (await Api
            .PostAsyncByContext<List<GetTagStatsOnLineVm>>("SReportOnProductLine", new GetOnProductLineReportContext(),
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnShifts = (await Api
            .PostAsyncByContext<List<GetTagsStatsOnShiftVm>>("SReportOnShift", new GetOnShiftContext(),
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnDates = (await Api
            .PostAsyncByContext<List<GetTagStatsOnProductDateVm>>("SReportOnProductDate", new GetOnProductDateContext(),
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

        OnDetails = (await Api
            .PostAsyncByContext<List<GetTagStatsOnDetailsVm>>("SReportOnDetails", new GetOnDetailContext(),
            new KeyValuePair<string, object>[] { new("search", request) })).Value;

        IsLoading = false;
    }

    private GetTagStatsQuery FixEmptiness()
    {
        GetTagStatsQuery request = new();

        request.FromDate = Request.FromDate.HasNoValue() ? "-1" : Request.FromDate;

        request.ToDate = Request.ToDate.HasNoValue() ? "-1" : Request.ToDate;

        request.Pl = Request.Pl.HasNoValue() ? "-1" : Request.Pl;

        request.Shift = Request.Shift.HasNoValue() ? "-1" : Request.Shift;

        request.Regcode = Request.Regcode.HasNoValue() ? "-1" : Request.Regcode;

        return request;
    }
}
