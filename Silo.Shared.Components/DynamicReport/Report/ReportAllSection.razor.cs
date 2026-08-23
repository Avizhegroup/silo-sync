using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class ReportAllSection<TColumn, TFilter> where TColumn : struct where TFilter : struct
{
    public Modal ModalFilters { get; set; }
    public Modal ModalColumns { get; set; }
    public Modal ModalFormats { get; set; }

    public ColumnSection<TColumn> ColumnSectionRef { get; set; }
    public ColumnEditSectionGeneric<TColumn> ColumnEditSectionRef { get; set; }

    [Parameter] public List<ReportFilterGeneric<TFilter>> Filters { get; set; }
    [Parameter] public List<ReportFilterGeneric<TFilter>> AddedFilters { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> DataColumns { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> AddedDataColumns { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> DataMiningElementColumns { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> AddedDataMiningElementColumns { get; set; }
    [Parameter] public List<ReportCalculatingColumn<TColumn>> CalculatingColumns { get; set; }
    [Parameter] public List<ReportCalculatingColumn<TColumn>> AddedCalculatingColumns { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> PivotColumns { get; set; }
    [Parameter] public ReportColumnGeneric<TColumn> AddedPivotColumn { get; set; }
    [Parameter] public bool IsColumnsEditable { get; set; } = true;

    [Parameter] public EventCallback<MouseEventArgs> OnSearchClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<TFilter>> OnAddNewFilterClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<TFilter>> OnFilterRemoveClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataColumnAddClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataMiningElementColumnAddClick { get; set; }
    [Parameter] public EventCallback<ReportCalculatingColumn<TColumn>> OnCalculatingColumnAddClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnPivotColumnAddClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataColumnRemoveClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnDataMiningElementColumnRemoveClick { get; set; }
    [Parameter] public EventCallback<ReportCalculatingColumn<TColumn>> OnCalculatingColumnRemoveClick { get; set; }
    [Parameter] public EventCallback<ReportColumnGeneric<TColumn>> OnPivotColumnRemoveClick { get; set; }

    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public TelerikNotification Notification { get; set; }

    #region Column Section Events
    public async Task OnDataColumnAdd(ReportColumnGeneric<TColumn> column)
    {
        if (AddedDataColumns is null || !AddedDataColumns.Any(p => p.Id == column.Id))
        {
            await OnDataColumnAddClick.InvokeAsync(column);
        }
    }

    public async Task OnDataMiningElementColumnAdd(ReportColumnGeneric<TColumn> column)
    {
        if (AddedDataMiningElementColumns is null || !AddedDataMiningElementColumns.Any(p => p.Id == column.Id))
        {
            await OnDataMiningElementColumnAddClick.InvokeAsync(column);
        }
    }

    public async Task OnCalculatingColumnAdd(ReportCalculatingColumn<TColumn> column)
    {
        if (AddedCalculatingColumns is null || !AddedCalculatingColumns.Any(p => p.Id == column.Id))
        {
            await OnCalculatingColumnAddClick.InvokeAsync(column);
        }
    }

    public async Task OnPivotColumnAdd(ReportColumnGeneric<TColumn> column)
    {
        await OnPivotColumnAddClick.InvokeAsync(column);
    }

    public async Task OnDataColumnRemove(ReportColumnGeneric<TColumn> column)
    {
        await OnDataColumnRemoveClick.InvokeAsync(column);
    }

    public async Task OnDataMiningElementColumnRemove(ReportColumnGeneric<TColumn> column)
    {
        await OnDataMiningElementColumnRemoveClick.InvokeAsync(column);
    }

    public async Task OnCalculatingColumnRemove(ReportCalculatingColumn<TColumn> column)
    {
        await OnCalculatingColumnRemoveClick.InvokeAsync(column);
    }

    public async Task OnPivotColumnRemove()
    {
        await OnPivotColumnRemoveClick.InvokeAsync(null);
    }

    public async Task OnClearColumnsClick(MouseEventArgs e)
    {
        AddedCalculatingColumns.Clear();

        AddedDataColumns.Clear();

        AddedDataMiningElementColumns.Clear();

        AddedPivotColumn = null;
    }

    public async Task OnColumnSectionModalClick(MouseEventArgs e)
    {
        await ColumnSectionRef.Refresh(
            AddedDataColumns,
            AddedCalculatingColumns,
            AddedDataMiningElementColumns,
            AddedPivotColumn);

        await ModalColumns.Open(e);
    }
    #endregion

    #region Filter Section Events
    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        AddedFilters = new();

        await ModalFilters.Open(e);

        StateHasChanged();
    }

    public async Task OnAddNewFilter(ReportFilterGeneric<TFilter> filter)
    {
        await OnAddNewFilterClick.InvokeAsync(filter);
    }

    public async Task OnFilterRemove(ReportFilterGeneric<TFilter> filter)
    {
        await OnFilterRemoveClick.InvokeAsync(filter);
    }
    #endregion

    #region General Section
    public async Task OnFormatChoose(GetReportFormatsByPathVm format)
    {
        AddedCalculatingColumns.Clear(); 

        AddedPivotColumn = null;

        AddedFilters.Clear();

        AddedDataColumns.Clear();

        AddedDataMiningElementColumns.Clear();

        foreach (var detail in format.DetailsList)
        {
            switch (detail.DetailType)
            {
                case ReportFormatDetailTypes.Data:
                    {
                        if (detail.AdditionalData.TryGetValue("ColumnId", out var detailColumnId) &&
                            detail.AdditionalData.TryGetValue("ColumnType", out var detailColumnType))
                        {
                            var column = DataColumns.FirstOrDefault(p =>
                                p.AdditionalData.TryGetValue("ColumnId", out var columnId) &&
                                columnId == detailColumnId &&
                                p.AdditionalData.TryGetValue("ColumnType", out var columnType) &&
                                columnType == detailColumnType);

                            if (column != null)
                            {
                                AddedDataColumns.Add(column);
                            }
                        }
                        else
                        {
                            var column = DataColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                            AddedDataColumns.Add(column);
                        }
                        break;
                    }

                case ReportFormatDetailTypes.Calculating:
                    {
                        var column = CalculatingColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                        AddedCalculatingColumns.Add(column);
                        break;
                    }

                case ReportFormatDetailTypes.Pivot:
                    {
                        var column = DataColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                        AddedPivotColumn = column;
                        break;
                    }

                case ReportFormatDetailTypes.Filter:
                    {
                        if (detail.AdditionalData.TryGetValue("FilterId", out var detailFilterId) &&
                        detail.AdditionalData.TryGetValue("FilterType", out var detailFilterType))
                        {
                            var filter = Filters.FirstOrDefault(p =>
                                p.AdditionalData.TryGetValue("FilterId", out var filterId) &&
                                filterId == detailFilterId &&
                                p.AdditionalData.TryGetValue("FilterType", out var filterType) &&
                                filterType == detailFilterType);

                            if (filter != null)
                            {
                                filter.Value = detail.Value;

                                AddedFilters.Add(filter);
                            }
                        }
                        else
                        {
                            var filter = Filters.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                            filter.Value = detail.Value;

                            AddedFilters.Add(filter);
                        }
                        break;
                    }

                case ReportFormatDetailTypes.DataMiningElements:
                    {
                        if (detail.AdditionalData.TryGetValue("ColumnId", out var detailColumnId) &&
                            detail.AdditionalData.TryGetValue("ColumnType", out var detailColumnType))
                        {
                            var column = DataMiningElementColumns.FirstOrDefault(p =>
                                p.AdditionalData.TryGetValue("ColumnId", out var columnId) &&
                                columnId == detailColumnId &&
                                p.AdditionalData.TryGetValue("ColumnType", out var columnType) &&
                                columnType == detailColumnType);

                            if (column != null)
                            {
                                AddedDataMiningElementColumns.Add(column);
                            }
                        }
                        else
                        {
                            var column = DataMiningElementColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                            AddedDataMiningElementColumns.Add(column);
                        }
                        break;
                    }
            }
        }

        await ModalFormats.Close(new());
    }
    #endregion
}
