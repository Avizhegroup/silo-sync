using System.Text.Json;
using Silo.Application.Dto;
using Silo.Application.Dto.Chart;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components;
public partial class DynamicChart
{
    public bool IsPivotChart = false;
    public string CalculationColumn = string.Empty;
    public List<TelerikDropDownItem> ChartDatas = new();
    public List<DynamicPivotData> PivotChartDatas = new();
    public string seriesLabelTamplate = "#=value# %";
    public string seriesLabelTamplateNoPercent = "#=category#\n #=value#";
    public ChartSeriesLabelsPosition seriesLabelPosition = ChartSeriesLabelsPosition.Right;

    [Parameter][EditorRequired] public List<object> Results { get; set; }
    [Parameter][EditorRequired] public List<string> AddedDataColumns { get; set; }
    [Parameter][EditorRequired] public List<string> AddedCalculatingColumns { get; set; }
    [Parameter][EditorRequired] public List<string> PivotColumns { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AddedCalculatingColumns.Count == 1)
        {
            CalculationColumn = AddedCalculatingColumns.First();
        }
    }

    public async Task OnCreateChartClick()
    {
        if (PivotColumns.Any())
        {
            FillPivotChartDatas();
        }
        else
        {
            if (AddedDataColumns.Count == 0)
            {
                Notification.Show("چارت: " + string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Columns_Data), "error");

                return;
            }

            if (CalculationColumn.HasNoValue())
            {
                Notification.Show("چارت: " + string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Columns_Calculating), "error");

                return;
            }

            FillChartDatas();
        }
    }

    public async Task PrintCharts()
    {
        await JSRuntime.InvokeVoidAsync("printFullCharts", ".printable-element");
    }

    #region Private 
    private void FillChartDatas()
    {
        ChartDatas = new();

        foreach (JsonElement data in Results)
        {
            TelerikDropDownItem item = new();

            List<string> staticColumns = new();

            foreach (var staticColumn in AddedDataColumns)
            {
                if (data.TryGetProperty(staticColumn, out JsonElement name))
                {
                    string nameString = name.ToString().Replace("{}", "0");

                    if (nameString.HasNoValue())
                    {
                        nameString = "0";
                    }

                    staticColumns.Add(nameString);
                }
            }

            item.Name = string.Join(',', staticColumns);

            if (data.TryGetProperty(CalculationColumn, out JsonElement value))
            {
                string valueString = value.ToString().Replace("{}", "0");

                if (valueString.HasNoValue())
                {
                    valueString = "0";
                }

                item.Value = valueString;
            }

                ChartDatas.Add(item);
        }

        ChartDatas = ChartDatas.OrderBy(p => p.Name).ToList();
    }

    private void FillPivotChartDatas()
    {
        PivotChartDatas = new();

        foreach (JsonElement data in Results)
        {
            List<string> staticColumns = new();

            foreach (var staticColumn in AddedDataColumns)
            {
                if (data.TryGetProperty(staticColumn, out JsonElement name))
                {
                    string nameString = name.ToString().Replace("{}", "0");

                    if (nameString.HasNoValue())
                    {
                        nameString = "0";
                    }

                    staticColumns.Add(nameString);
                }
            }

            string aggStaticColumn = string.Join(',', staticColumns);

            foreach (var pivotColumn in PivotColumns)
            {
                if (data.TryGetProperty(pivotColumn, out JsonElement value))
                {
                    string valueString = value.ToString().Replace("{}", "0");

                    if (valueString.HasNoValue())
                    {
                        valueString = "0";
                    }

                    if (valueString != "0")
                    {
                        PivotChartDatas.Add(new()
                        {
                            PivotColumn = pivotColumn,
                            StaticColumn = aggStaticColumn,
                            Value = valueString
                        });
                    }
                }
            }
        }

        PivotChartDatas = PivotChartDatas.OrderBy(p => p.PivotColumn).ToList();
    }
    #endregion
}
