using System.Text.Json;
using Silo.Application;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Ai;

public partial class AiResultGrid
{
    public bool IsLoading = true;
    public Modal? FormatModal { get; set; }

    [Parameter] public List<List<object>> Data { get; set; } = new();
    [Parameter] public string Mode { get; set; } = string.Empty;
    [Parameter][EditorRequired] public int QueryId { get; set; }
    [Parameter] public bool ShowSaveButton { get; set; } = true;

    [Inject] public IExcelExport ExcelExporter { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    private List<IDictionary<string, object>> ParseGridData(List<object> data)
    {
        var result = new List<IDictionary<string, object>>();

        if (data == null || data.Count == 0)
            return result;

        var rows = data
            .OfType<JsonElement>()
            .Where(e => e.ValueKind == JsonValueKind.Object);

        foreach (var row in rows)
        {
            var dict = new Dictionary<string, object>();

            foreach (var prop in row.EnumerateObject())
            {
                dict[prop.Name] =
                    prop.Value.ValueKind == JsonValueKind.Null
                        ? string.Empty
                        : prop.Value.ToString();
            }

            result.Add(dict);
        }

        return result;
    }

    private async Task ExportToExcelAsync(List<object> rawData)
    {
        if (rawData == null || !rawData.Any()) return;

        IsLoading = true;

        StateHasChanged();

        var rows = rawData.OfType<JsonElement>().Where(e => e.ValueKind == JsonValueKind.Object).ToList();
        if (!rows.Any()) return;

        var columns = rows.First().EnumerateObject().Select(p => p.Name).ToList();

        string fileName = $"گزارش هوش مصنوعی_{PersianCalendarTools.GregorianToPersianWithManualSeprator(DateTime.Now, "")}";

        await ExcelExporter.ExportJsonData(fileName, rawData, columns);

        IsLoading = false;

        StateHasChanged();

    }

    private async Task OnSaveReportClick(MouseEventArgs e)
    {
        if (Data == null || !Data.Any() || Data[0] == null)
        {
            Notification.Show("گزارشی برای ذخیره وجود ندارد", "error");
            return;
        }

        if (QueryId is <= 0)
        {
            Notification?.Show("شناسه کوئری معتبر نیست", "error");
            return;
        }

        if (FormatModal is not null)
            await FormatModal.Open(e);
    }

    private async Task OnFormatSaved()
    {
        if (FormatModal is not null)
            await FormatModal.Close(null);
    }
}
