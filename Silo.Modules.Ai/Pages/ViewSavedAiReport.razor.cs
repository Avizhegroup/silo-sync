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
    public int ReportQueryReferenceId { get; set; }

    private int _loadedReportId = -1;
    private CancellationTokenSource? _cts;
    private bool _isLoadingInProgress = false;

    [Parameter] public int ReportId { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }


    protected override async Task OnParametersSetAsync()
    {
        if (ReportId == _loadedReportId || ReportId <= 0)
            return;

        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        if (_isLoadingInProgress)
            return;

        _isLoadingInProgress = true;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        IsLoading = true;
        ReportData = new();          
        StateHasChanged();           

        try
        {
            var report = (await Api.PostAsyncByUriAndContext<GetAiReportDataVm>(
                "wms/ReportFormat",
                "SGetAiReportData",
                new GetReportFormatByIdVmContext(),
                new KeyValuePair<string, object>(
                    "query",
                    new GetReportFormatByIdQuery
                    {
                        FormatId = ReportId
                    })
            )).Value;

            if (token.IsCancellationRequested)
                return;

            if (report != null)
            {
                ReportName = report.Name;
                ReportData = report.Data ?? new();
                ReportQueryReferenceId = report.QueryReferenceId;
                PageTitle = ReportName;

                _loadedReportId = ReportId;    
            }
            else
            {
                Notification.Show("گزارش یافت نشد", "error");
                ReportData = new();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
                Notification.Show($"خطا در اجرای گزارش: {ex.Message}", "error");

            ReportData = new();
        }
        finally
        {
            _isLoadingInProgress = false;
            IsLoading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}

public class GetReportFormatByIdQuery
{
    public int FormatId { get; set; }
}

