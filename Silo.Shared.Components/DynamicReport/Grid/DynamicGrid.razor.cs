using System.Dynamic;
using System.Globalization;
using System.Text.Json;

namespace Silo.Shared.Components;
public partial class DynamicGrid
{
    private const string rowSumKey = "__rowSum";
    private const string serialKey = "__serial";
    private List<GridColumnInfo> columns = new();
    private object lastResults;
    private int lastCount = -1;
    private List<string> lastColumns;

    public List<ExpandoObject> Rows = new();

    [Parameter][EditorRequired] public List<object> Results { get; set; }
    [Parameter][EditorRequired] public List<string> GridColumns { get; set; }
    [Parameter] public Dictionary<string, decimal> ColumnAgg { get; set; }
    [Parameter] public bool HasPivotColumn { get; set; }
    [Parameter] public decimal TotalSum { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnExcelExportClick { get; set; }
    [Parameter] public string? SerialColumnName { get; set; }

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(lastResults, Results) 
            && lastCount == (Results?.Count ?? 0)
            && lastColumns is not null 
            && GridColumns is not null && lastColumns.SequenceEqual(GridColumns))
        {
            return;
        }

        lastResults = Results;

        lastCount = Results?.Count ?? 0;

        lastColumns = GridColumns?.ToList();

        BuildRows();
    }

    private void BuildRows()
    {
        Rows = new();

        columns = new();

        if (Results is null || GridColumns is null)
        {
            return;
        }

        List<Dictionary<string, string>> raw = new(Results.Count);
        List<decimal> rowSums = new(Results.Count);

        foreach (var result in Results)
        {
            Dictionary<string, string> row = new ();

            decimal rowSum = 0;

            if (result is JsonElement element && element.ValueKind == JsonValueKind.Object)
            {
                foreach (var title in GridColumns)
                {
                    if (!element.TryGetProperty(title, out var property))
                    {
                        continue;
                    }

                    var value = property.ToString().Replace("{}", "0");

                    if (value.HasNoValue())
                    {
                        value = "0";
                    }

                    row[title] = value;

                    if (ColumnAgg is not null && ColumnAgg.ContainsKey(title))
                    {
                        if (decimal.TryParse(value, out var number))
                        {
                            rowSum += number;
                        }
                        else
                        {
                            rowSum++;
                        }
                    }
                }
            }

            raw.Add(row);

            rowSums.Add(rowSum);
        }

        HashSet<string> numericColumns = new ();

        for (int i = 0; i < GridColumns.Count; i++)
        {
            var title = GridColumns[i];

            var values = raw.Where(r => r.ContainsKey(title)).Select(r => r[title]).ToList();

            if (values.Count > 0 && values.All(v => decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out _)))
            {
                numericColumns.Add(title);
            }

            columns.Add(new GridColumnInfo($"c{i}", title, numericColumns.Contains(title) ? typeof(decimal) : typeof(string)));
        }

        for (int r = 0; r < raw.Count; r++)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            for (int i = 0; i < GridColumns.Count; i++)
            {
                var title = GridColumns[i];

                if (!raw[r].TryGetValue(title, out var value))
                {
                    continue;
                }

                expando[$"c{i}"] = numericColumns.Contains(title)
                    ? decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture)
                    : value;
            }

            expando[rowSumKey] = rowSums[r];

            if (SerialColumnName.HasValue() && raw[r].TryGetValue(SerialColumnName, out var serial))
            {
                expando[serialKey] = serial;
            }

            Rows.Add((ExpandoObject)expando);
        }
    }

    private static string GetTagHistoryUrl(ExpandoObject row)
    {
        IDictionary<string, object> values = row;

        return values.TryGetValue(serialKey, out var serial)
            ? $"/report/taghistory/{Uri.EscapeDataString(serial?.ToString() ?? string.Empty)}"
            : null;
    }

    record GridColumnInfo(string Key, string Title, Type FieldType);
}
