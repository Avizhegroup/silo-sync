using System.Text.Json;
using AutoMapper;
using Silo.Identity.Client;

namespace Silo.Modules.Ai.Pages;

public partial class ViewSavedAiReport
{
    public bool IsLoading = true;
    public List<List<object>> ReportData = new();
    public string ReportName = string.Empty;
    public string ReportMode = string.Empty;
    public int ReportQueryId { get; set; }

    private int loadedReportId = -1;

    [Parameter] public int ReportId { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (ReportId <= 0 || ReportId == loadedReportId)
            return;

        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        IsLoading = true;

        ReportData = new();
        ReportName = string.Empty;
        ReportMode = string.Empty;
        ReportQueryId = 0;

        StateHasChanged();

        var requestedReportId = ReportId;

        var report = (await Api.PostAsyncByUriAndContext<GetAiReportDataVm>(
            "wms/ReportFormat",
            "SGetAiReportData",
            new GetReportFormatByIdVmContext(),
            new KeyValuePair<string, object>(
                "query",
                new GetReportFormatByIdQuery
                {
                    FormatId = requestedReportId
                })
        )).Value;

        if (ReportId != requestedReportId)
            return;

        if (report != null)
        {
            ReportName = report.Name;
            ReportData = report.Data ?? new();
            ReportQueryId = report.QueryId;
            PageTitle = ReportName;

            loadedReportId = requestedReportId;
        }
        else
        {
            Notification.Show("گزارش یافت نشد", "error");
            ReportData = new();
        }

        IsLoading = false;

        StateHasChanged();
    }
}
