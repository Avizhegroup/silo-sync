using Microsoft.AspNetCore.Components.Web;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Inspect.Pages;

public partial class InspectReportOnProduct
{
    public bool IsLoading = true;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public GetInspectAggByQcQuery Request = new();
    public List<GetInspectAggByQcHeaderVm> Responses = new();
    public List<string> Columns = new();
    public List<string> Titles = new()
    {
        TextResources.APP_StringKeys_Chart_Qc,
        TextResources.APP_StringKeys_Product_Size,
        TextResources. APP_StringKeys_Line,
        TextResources. APP_StringKeys_Accept_Inspect_Count,
        TextResources. APP_StringKeys_Accept_Inspect_Value,
        TextResources. APP_StringKeys_Reject_Inspect_Count,
        TextResources. APP_StringKeys_Reject_Inspect_Value
    };
    public int AcceptedCount = 0;
    public decimal AcceptedSum = 0;
    public int RejectedCount = 0;
    public decimal RejectedSum = 0;
    public Dictionary<string, decimal> CountQcs = new();
    public Dictionary<string, decimal> PercentQcs = new();

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetails { get; set; }

    [Inject] public IExcelExport ExcelExport { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }


    protected override async Task SiloInitializer()
    {
        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Lines = await FormalCache.GetLines();

        Shifts = await FormalCache.GetShifts();

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        GetInspectAggByQcQuery search = FixEmptiness();

        var result = (await Api.PostAsyncByContext<List<GetInspectAggByQcHeaderVm>>("SReportInspectProducts"
            , new GetInspectAggByQcHeaderVmContext()
            , new KeyValuePair<string, object>[] { new("search", search) })).Value;

        CountQcs.Clear();

        Columns.Clear();

        PercentQcs.Clear();

        AcceptedCount = result.Sum(p => (int)p.AcceptedCount);

        AcceptedSum = result.Sum(p => p.AcceptedSum);

        RejectedCount = result.Sum(p => (int)p.RejectedCount);

        RejectedSum = result.Sum(p => p.RejectedSum);

        foreach (var header in result)
        {
            foreach (var item in header.Items)
            {
                if (!Columns.Any(p => p == item.Qc))
                {
                    Columns.Add(item.Qc);

                    CountQcs.Add(item.Qc, 0);

                    PercentQcs.Add(item.Qc, 0);
                }
            }

            foreach (var item in header.Items)
            {
                CountQcs[item.Qc] += item.SumCount;

                if (AcceptedSum != 0)
                {
                    PercentQcs[item.Qc] = Math.Round((CountQcs[item.Qc] / AcceptedSum) * 100);
                }
                else
                {
                    PercentQcs[item.Qc] = 0;
                }
            }
        }

        Responses = result;

        IsLoading = false;
    }

    public async Task OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Responses = new();

        Columns = new();

        AcceptedCount = 0;

        AcceptedSum = 0;
    }

    public async Task OnClickExport(MouseEventArgs e)
    {
        //var excelStream = ExcelExport.ExportJArray(Titles, Columns, Responses);

        //await Export.ExportAndDownload(excelStream, $"{TextResources.APP_StringKeys_Inspect_ReportProduct}.xlsx");

        //excelStream.Dispose();
    }

    private GetInspectAggByQcQuery FixEmptiness()
    {
        GetInspectAggByQcQuery search = new();

        if (string.IsNullOrEmpty(Request.ProductCode))
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Request.ProductCode;
        }

        if (string.IsNullOrEmpty(Request.FromDate))
        {
            search.FromDate = "-1";
        }
        else
        {
            search.FromDate = Request.FromDate;
        }

        if (string.IsNullOrEmpty(Request.ToDate))
        {
            search.ToDate = "-1";
        }
        else
        {
            search.ToDate = Request.ToDate;
        }

        if (string.IsNullOrEmpty(Request.FromTime))
        {
            search.FromTime = "-1";
        }
        else
        {
            search.FromTime = Request.FromTime;
        }

        if (string.IsNullOrEmpty(Request.ToTime))
        {
            search.ToTime = "-1";
        }
        else
        {
            search.ToTime = Request.ToTime;
        }

        if (string.IsNullOrEmpty(Request.TechnicalCode))
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Request.TechnicalCode;
        }

        search.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (string.IsNullOrEmpty(Request.Size))
        {
            search.Size = "-1";
        }
        else
        {
            search.Size = Request.Size;
        }

        if (string.IsNullOrEmpty(Request.Qc))
        {
            search.Qc = "-1";
        }
        else
        {
            search.Qc = Request.Qc;
        }

        if (string.IsNullOrEmpty(Request.Shift))
        {
            search.Shift = "-1";
        }
        else
        {
            search.Shift = Request.Shift;
        }

        if (string.IsNullOrEmpty(Request.Line))
        {
            search.Line = "-1";
        }
        else
        {
            search.Line = Request.Line;
        }

        return search;
    }
}
