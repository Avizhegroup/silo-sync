using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Shared.Tools;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Registered production by line, technical code and size with one column per quality (legacy F14 tab 2,
/// "تجمعی سایز و کد طرح"). The server returns flat rows (<c>SRepRegisterTagMatrix</c>, fully parameterised); the quality
/// columns, the row totals and the total row are built here.
/// </summary>
public partial class RegisterMatrix
{
    private static readonly Regex DateRegex = new(@"^14\d{2}/(0[1-9]|1[0-2])/(0[1-9]|[12]\d|3[01])$", RegexOptions.Compiled);
    private static readonly Regex TimeRegex = new(@"^([01]\d|2[0-3]):[0-5]\d$", RegexOptions.Compiled);

    public bool IsLoading = true;
    public List<GetAllLinesVm> Lines = new();
    public List<GetAllShiftsVm> Shifts = new();
    public List<GetAllProductQcsVm> Qcs = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes = new();

    public string ProductCode = string.Empty;
    public string Line = string.Empty;
    public string Shift = string.Empty;
    public string Qc = string.Empty;
    public string Size = string.Empty;
    public string TechnicalCode = string.Empty;
    public bool TechnicalCodeLike;
    public string DocumentId = string.Empty;
    public string FromDate = string.Empty;
    public string ToDate = string.Empty;
    public string FromTime = string.Empty;
    public string ToTime = string.Empty;

    public List<MatrixRow>? Result;
    public List<string> QcColumns = new();
    public List<PivotRow> PivotRows = new();

    public bool FromDateError => FromDate.Length > 0 && !DateRegex.IsMatch(FromDate);
    public bool ToDateError => ToDate.Length > 0 && !DateRegex.IsMatch(ToDate);
    public bool DateError => (FromDate.Length > 0 && !DateRegex.IsMatch(FromDate)) || (ToDate.Length > 0 && !DateRegex.IsMatch(ToDate));
    public bool TimeError => (FromTime.Length > 0 && !TimeRegex.IsMatch(FromTime)) || (ToTime.Length > 0 && !TimeRegex.IsMatch(ToTime));
    public decimal GrandTotal => PivotRows.Sum(p => p.Total);

    [Inject] public RfidConnectApi Api { get; set; } = default!;
    [Inject] public IFormalDataCache FormalCache { get; set; } = default!;
    [Inject] public IExcelExport ExcelExporter { get; set; } = default!;
    [Inject] public IExport Exporter { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        try
        {
            Lines = await FormalCache.GetLines();
            Shifts = await FormalCache.GetShifts();
            Qcs = await FormalCache.GetQcs();
            Sizes = await FormalCache.GetSizes();
            SetBusinessDay();
        }
        catch
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>Legacy default: today to tomorrow (the business day starts with the first shift).</summary>
    private void SetBusinessDay()
    {
        var calendar = new PersianCalendar();
        string Format(DateTime date) => $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
        FromDate = Format(DateTime.Today);
        ToDate = Format(DateTime.Today.AddDays(1));
    }

    public void OnClearClick()
    {
        ProductCode = Line = Shift = Qc = Size = TechnicalCode = DocumentId = FromTime = ToTime = string.Empty;
        TechnicalCodeLike = false;
        SetBusinessDay();
        Result = null;
        PivotRows = new();
        QcColumns = new();
    }

    public async Task OnSearchClick()
    {
        if (DateError || TimeError)
        {
            Notification.Show("Dates must look like 1404/05/12 and times like 06:00.", "error");
            return;
        }

        IsLoading = true;

        try
        {
            var filter = new GetRegisterRequestFilter
            {
                ProductCode = Unset(ProductCode),
                Line = Unset(Line),
                Shift = Unset(Shift),
                Qc = Unset(Qc),
                Size = Unset(Size),
                TechnicalCode = Unset(TechnicalCode),
                TechnicalCodeLike = TechnicalCodeLike,
                ContractStatus = Unset(DocumentId),
                ProductSerial = "-1",
                ProductOldSerial = "-1",
                ProductGroup = "-1",
                ProductBrand = "-1",
                RegisterDevice = "-1",
                InspectStatus = "-1",
                User = "-1",
                FromDate = FromDate.Length == 0 ? "-1" : FromDate.Replace("/", string.Empty),
                ToDate = ToDate.Length == 0 ? "-1" : ToDate.Replace("/", string.Empty),
                FromTime = FromTime.Length == 0 ? "-1" : FromTime.Replace(":", string.Empty) + "00",
                ToTime = ToTime.Length == 0 ? "-1" : ToTime.Replace(":", string.Empty) + "00"
            };

            Result = (await Api.PostAsync<List<MatrixRow>>("SRepRegisterTagMatrix"
                , new KeyValuePair<string, object>("filter", filter))).Value ?? new();

            BuildPivot();
        }
        catch
        {
            Result = null;
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnExcelClick()
    {
        if (PivotRows.Count == 0)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var table = new DataTable();
            table.Columns.Add(TextResources.APP_StringKeys_Line);
            table.Columns.Add(TextResources.APP_StringKeys_Chart_Regcode);
            table.Columns.Add(TextResources.APP_StringKeys_Product_Size);
            table.Columns.Add(TextResources.APP_StringKeys_ProductType);
            QcColumns.ForEach(p => table.Columns.Add(p));
            table.Columns.Add(TextResources.APP_StringKeys_SumValue);

            foreach (var row in PivotRows)
            {
                var values = new List<object> { row.Line, row.RegCode, row.Size, row.ProductType };
                values.AddRange(QcColumns.Select(p => (object)row.Value(p).ToString("0.##", CultureInfo.InvariantCulture)));
                values.Add(row.Total.ToString("0.##", CultureInfo.InvariantCulture));
                table.Rows.Add(values.ToArray());
            }

            var totals = new List<object> { TextResources.APP_StringKeys_SumValue, string.Empty, string.Empty, string.Empty };
            totals.AddRange(QcColumns.Select(p => (object)ColumnTotal(p).ToString("0.##", CultureInfo.InvariantCulture)));
            totals.Add(GrandTotal.ToString("0.##", CultureInfo.InvariantCulture));
            table.Rows.Add(totals.ToArray());

            var stream = ExcelExporter.ExportDatatable(table);
            stream.Seek(0, SeekOrigin.Begin);
            await Exporter.ExportAndDownload(stream, "RegisterMatrix.xlsx");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public decimal ColumnTotal(string qc) => PivotRows.Sum(p => p.Value(qc));

    private void BuildPivot()
    {
        var rows = Result ?? new();

        // One column per quality found in the data, in alphabetical order.
        QcColumns = rows.Select(p => p.Qc).Distinct(StringComparer.Ordinal).OrderBy(p => p, StringComparer.Ordinal).ToList();

        PivotRows = rows
            .GroupBy(p => (p.Line, p.RegCode, p.Size, p.ProductType))
            .Select(group => new PivotRow
            {
                Line = group.Key.Line,
                RegCode = group.Key.RegCode,
                Size = group.Key.Size,
                ProductType = group.Key.ProductType,
                Values = group.GroupBy(p => p.Qc).ToDictionary(p => p.Key, p => p.Sum(x => x.Count))
            })
            .OrderBy(p => p.RegCode, StringComparer.Ordinal)
            .ThenBy(p => p.Size, StringComparer.Ordinal)
            .ToList();
    }

    private static string Unset(string value) => string.IsNullOrWhiteSpace(value) ? "-1" : value.Trim();

    public sealed class MatrixRow
    {
        public string Line { get; set; } = string.Empty;
        public string RegCode { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Qc { get; set; } = string.Empty;
        public decimal Count { get; set; }
    }

    public sealed class PivotRow
    {
        public string Line { get; set; } = string.Empty;
        public string RegCode { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public Dictionary<string, decimal> Values { get; set; } = new();
        public decimal Total => Values.Values.Sum();
        public decimal Value(string qc) => Values.GetValueOrDefault(qc);
    }
}
