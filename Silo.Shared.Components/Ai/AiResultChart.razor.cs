using System.Globalization;
using System.Text.Json;
using Silo.Application.Shared.Features;
using Silo.Shared.Tools;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Microsoft.JSInterop;

namespace Silo.Shared.Components.Ai;

public partial class AiResultChart
{

    public const int MaxLabelLength = 22;
    public const string BaseColor = "#4e9bff";
    public static readonly string[] Palette =
    {
        "#4e9bff", "#ff8a4c", "#3ecf8e", "#f25f7a", "#b184ff",
        "#f5c542", "#2ec4d6", "#ff7ab8", "#8bc34a", "#9aa5b1"
    };

    public int LineHeight => 460;
    public int LineWidth => Math.Max(700, ChartData.Count * 60);
    public int PieHeight => 440;
    public int ColumnHeight => 480;
    public int ColumnWidth => Math.Max(900, ChartData.Count * 110);
    public string LineHeightText => LineHeight + "px";
    public string PieHeightText => PieHeight + "px";
    public string ColumnHeightText => ColumnHeight + "px";
    public string LineContainerStyle =>  "width:" + LineWidth + "px; min-width:100%";
    public string ColumnContainerStyle => "width:" + ColumnWidth + "px; min-width:" + ColumnWidth + "px; max-width:none";
    public static string Shorten(string s) => s.Length <= MaxLabelLength ? s : s.Substring(0, MaxLabelLength - 1) + "…";

    public List<ChartPoint> ChartData { get; set; } = new();

    public ElementReference ChartContainer;

    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public List<List<object>>? Data { get; set; }
    [Parameter] public AiReportCommandType Type { get; set; }

    protected override void OnParametersSet()
    {
        ChartData = BuildChartData();
    }
    
    private List<ChartPoint> BuildChartData()
    {
        var result = new List<ChartPoint>();

        if (Data is null || Data.Count == 0 || Data[0] is null)
        {
            return result;
        }

        foreach (var row in Data[0])
        {
            if (row is null)
            {
                continue;
            }

            var category = GetText(Read(row, "Category"));

            if (category.HasNoValue())
            {
                continue;
            }

            if (!TryGetDouble(Read(row, "Value"), out var number))
            {
                continue;
            }

            result.Add(new ChartPoint { Category = category.Trim(), Value = number });
        }

        if (Type == AiReportCommandType.PieChart)
        {
            result = result.Where(p => p.Value > 0).ToList();
        }

        var usedKeys = new HashSet<string>();

        for (var i = 0; i < result.Count; i++)
        {
            var shown = Shorten(result[i].Category);

            while (!usedKeys.Add(shown))
            {
                shown += "\u200B";
            }

            result[i].Key = shown;

            result[i].Short = shown;

            result[i].Label = result[i].Category + " — " + result[i].Value.ToString("N0");

            result[i].Color = Palette[i % Palette.Length];
        }

        return result;
    }

    private object? Read(object row, string key)
    {
        if (row is JsonElement json)
        {
            if (json.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            foreach (var prop in json.EnumerateObject())
            {
                if (string.Equals(prop.Name, key, StringComparison.OrdinalIgnoreCase))
                {
                    return prop.Value;
                }
            }

            return null;
        }

        if (row is IDictionary<string, object> dict)
        {
            foreach (var kv in dict)
            {
                if (string.Equals(kv.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    return kv.Value;
                }
            }
        }

        return null;
    }

    private string GetText(object? value)
    {
        if (value is null || value is DBNull)
        {
            return string.Empty;
        }

        if (value is JsonElement json)
        {
            return json.ValueKind switch
            {
                JsonValueKind.Null or JsonValueKind.Undefined => string.Empty,
                JsonValueKind.String => json.GetString() ?? string.Empty,
                _ => json.ToString()
            };
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private bool TryGetDouble(object? value, out double result)
    {
        result = 0;

        if (value is null || value is DBNull)
        {
            return false;
        }

        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Number)
            {
                return json.TryGetDouble(out result);
            }

            if (json.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = json.GetString();
        }

        if (value is string text)
        {
            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
     
        return !double.IsNaN(result) && !double.IsInfinity(result);
        
    }

    private async Task DownloadChartAsync()
    {
        var fileName = Type switch
        {
            AiReportCommandType.PieChart => "PieChart.png",
            AiReportCommandType.LineChart => "LineChart.png",
            _ => "ColumnChart.png"
        };

        await JSRuntime.InvokeVoidAsync("downloadChartAsPng", ChartContainer, fileName);
    }

    private double PieTotal
    {
        get
        {
            var total = ChartData.Sum(p => Math.Max(p.Value, 0));

            return total > 0 ? total : 1;
        }
    }

}
