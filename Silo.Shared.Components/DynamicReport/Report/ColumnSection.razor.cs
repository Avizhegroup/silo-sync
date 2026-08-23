using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class ColumnSection<TColumn> where TColumn : struct
{
    public Guid Id = Guid.NewGuid();
    public string Mode = "1";
    public List<TelerikDropDownItem> Modes = new()
    {
        new TelerikDropDownItem()
        {
            Name = TextResources.APP_StringKeys_Columns_Data,
            Value = "1"
        },
        new TelerikDropDownItem()
        {
            Name = TextResources.APP_StringKeys_Columns_Calculating,
            Value = "0"
        },
        new TelerikDropDownItem()
        {
            Name = TextResources.APP_StringKeys_Columns_Pivot,
            Value = "2"
        },
        new TelerikDropDownItem()
        {
            Name = TextResources.APP_StringKeys_Data_Mining_Elements,
            Value = "3"
        }
    };
    public int SelectedCalculatingColumn = 0;
    public int SelectedDataColumn = 0;
    public int SelectedDataMiningElementColumn = 0;
    public int SelectedPivotColumn = 0;
    public List<ReportColumnGeneric<TColumn>> AddedDataColumns = new();
    public List<ReportColumnGeneric<TColumn>> AddedDataMiningElementColumns = new();
    public List<ReportCalculatingColumn<TColumn>> AddedCalculatingColumns = new();
    public ReportColumnGeneric<TColumn> AddedPivotColumn;

    [Parameter][EditorRequired] public List<ReportColumnGeneric<TColumn>> DataColumns { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> DataMiningElementColumns { get; set; }
    [Parameter][EditorRequired] public List<ReportCalculatingColumn<TColumn>> CalculatingColumns { get; set; }
    [Parameter][EditorRequired] public List<ReportColumnGeneric<TColumn>> PivotColumns { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataColumnAdd { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataMiningElementColumnAdd { get; set; }
    [Parameter] public EventCallback<ReportCalculatingColumn<TColumn>> OnCalculatingColumnAdd { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataColumnRemove { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataMiningElementColumnRemove { get; set; }
    [Parameter] public EventCallback<ReportCalculatingColumn<TColumn>> OnCalculatingColumnRemove { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnPivotColumnAdd { get; set; }
    [Parameter] public EventCallback OnPivotColumnRemove { get; set; }

    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    public async Task OnModeChange(object e)
    {
        SelectedCalculatingColumn = 0;
        SelectedDataColumn = 0;
        SelectedDataMiningElementColumn = 0;
    }

    public async Task OnAddColumnClick(MouseEventArgs e)
    {
        if (Mode.Equals("0"))
        {
            var calculatingColumn = CalculatingColumns.FirstOrDefault(p => p.Id == SelectedCalculatingColumn);

            if (calculatingColumn is not null)
            {
                if (AddedCalculatingColumns.Any(p => p.Id == calculatingColumn.Id))
                {
                    Notification.Show(TextResources.APP_StringKeys_Column_Duplicate, "warning");
                    return;
                }

                ReportCalculatingColumn<TColumn> newCal = new()
                {
                    Id = calculatingColumn.Id,
                    Title = calculatingColumn.Title,
                    Type = calculatingColumn.Type
                };

                AddedCalculatingColumns.Add(newCal);

                await OnCalculatingColumnAdd.InvokeAsync(newCal);
            }
        }
        else if (Mode.Equals("1"))
        {
            var dataColumn = DataColumns.FirstOrDefault(p => p.Id == SelectedDataColumn);

            if (dataColumn is not null)
            {
                if (AddedDataColumns.Any(p => p.Id == dataColumn.Id))
                {
                    Notification.Show(TextResources.APP_StringKeys_Column_Duplicate, "warning");
                    return;
                }

                ReportColumnGeneric<TColumn> newDat = new()
                {
                    Id = dataColumn.Id,
                    Title = dataColumn.Title
                };

                AddedDataColumns.Add(newDat);

                await OnDataColumnAdd.InvokeAsync(newDat);
            }
        }
        else if (Mode.Equals("2"))
        {
            var pivotColumn = PivotColumns.FirstOrDefault(p => p.Id == SelectedPivotColumn);

            if (pivotColumn is not null)
            {
                AddedPivotColumn = new ()
                {
                    Id = pivotColumn.Id,
                    Title = pivotColumn.Title
                };

                await OnPivotColumnAdd.InvokeAsync(AddedPivotColumn);
            }
        }
        else
        {
            var dataMiningElementColumn = DataMiningElementColumns.FirstOrDefault(p => p.Id == SelectedDataMiningElementColumn);

            if (dataMiningElementColumn is not null)
            {
                if (AddedDataMiningElementColumns.Any(p => p.Id == dataMiningElementColumn.Id))
                {
                    Notification.Show(TextResources.APP_StringKeys_Column_Duplicate, "warning");
                    return;
                }

                ReportColumnGeneric<TColumn> newDat = new()
                {
                    Id = dataMiningElementColumn.Id,
                    Title = dataMiningElementColumn.Title,
                    Type = dataMiningElementColumn.Type,
                    Value = dataMiningElementColumn.Value,
                    AdditionalData = dataMiningElementColumn.AdditionalData,
                    IsColumnShown = dataMiningElementColumn.IsColumnShown
                };

                AddedDataMiningElementColumns.Add(newDat);

                await OnDataMiningElementColumnAdd.InvokeAsync(newDat);
            }
        }
    }

    public async Task OnRemoveDataClick(ReportColumnGeneric<TColumn> column)
    {
        AddedDataColumns.Remove(column);

        await OnDataColumnRemove.InvokeAsync(column);
    }

    public async Task OnRemoveDataMiningElementClick(ReportColumnGeneric<TColumn> column)
    {
        AddedDataMiningElementColumns.Remove(column);

        await OnDataMiningElementColumnRemove.InvokeAsync(column);
    }

    public async Task OnRemoveCalculatingClick(ReportCalculatingColumn<TColumn> column)
    {
        AddedCalculatingColumns.Remove(column);

        await OnCalculatingColumnRemove.InvokeAsync(column);
    }

    public async Task OnRemovePivotClick()
    {
        AddedPivotColumn = null;

        await OnPivotColumnRemove.InvokeAsync();
    }

    public async Task Refresh(
        List<ReportColumnGeneric<TColumn>> existingDataColumns = null,
        List<ReportCalculatingColumn<TColumn>> existingCalculatingColumns = null,
        List<ReportColumnGeneric<TColumn>> existingDataMiningElementColumns = null,
        ReportColumnGeneric<TColumn> existingPivotColumn = null)
    {
        Id = Guid.NewGuid();

        Mode = "1";

        SelectedCalculatingColumn = 0;

        SelectedDataColumn = 0;

        SelectedDataMiningElementColumn = 0;

        SelectedPivotColumn = 0;

        AddedDataColumns = existingDataColumns is not null ? new(existingDataColumns) : new();

        AddedCalculatingColumns = existingCalculatingColumns is not null ? new(existingCalculatingColumns) : new();

        AddedDataMiningElementColumns = existingDataMiningElementColumns is not null ? new(existingDataMiningElementColumns) : new();

        AddedPivotColumn = existingPivotColumn;
    }
}
