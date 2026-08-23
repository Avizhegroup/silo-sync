using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class ColumnEditSectionGeneric<TColumn> where TColumn : struct
{
    public List<TelerikDropDownItemGeneric<ReportColumnSortType>> SortTypes = new()
    {
        new()
        {
            Value = ReportColumnSortType.None,
            Name = TextResources.APP_StringKeys_NotChoosed
        },
         new()
        {
            Value = ReportColumnSortType.Asc,
            Name = TextResources.APP_StringKeys_Sort_Asc
        },
         new()
        {
            Value = ReportColumnSortType.Desc,
            Name = TextResources.APP_StringKeys_Sort_Desc
        }
    };

    [Parameter][EditorRequired] public List<ReportColumnGeneric<TColumn>> AddedDataColumns { get; set; }
    [Parameter][EditorRequired] public List<ReportCalculatingColumn<TColumn>> AddedCalculatingColumns { get; set; }
    [Parameter][EditorRequired] public ReportColumnGeneric<TColumn> AddedPivotColumn { get; set; }
    [Parameter] public List<ReportColumnGeneric<TColumn>> AddedDataMiningElementColumns { get; set; }
    [Parameter] public bool IsEditable { get; set; } = true;

    public TelerikContextMenu<TelerikContextMenuItem> ContextMenuRef { get; set; }
    public List<TelerikContextMenuItem> MenuItems { get; set; }
    
    private ReportColumnGeneric<TColumn> SelectedDataColumn { get; set; }
    private ReportCalculatingColumn<TColumn> SelectedCalculatingColumn { get; set; }
    private ReportColumnGeneric<TColumn> SelectedDataMiningColumn { get; set; }
    private bool IsSelectedPivotColumn { get; set; }

    protected override async Task OnInitializedAsync()
    {
        InitializeContextMenu();
    }

    private void InitializeContextMenu()
    {
        MenuItems = new List<TelerikContextMenuItem>
        {
            new TelerikContextMenuItem
            {
                Text = TextResources.APP_StringKeys_Delete,
                Icon = "delete"
            },
            new TelerikContextMenuItem
            {
                Text = TextResources.APP_StringKeys_Sorting,
                Icon = "sort",
                Items = new List<TelerikContextMenuItem>
                {
                    new TelerikContextMenuItem
                    {
                        Text = TextResources.APP_StringKeys_Sort_Asc,
                        Icon = "sort-asc-small"
                    },
                    new TelerikContextMenuItem
                    {
                        Text = TextResources.APP_StringKeys_Sort_Desc,
                        Icon = "sort-desc-small"
                    }
                }
            }
        };
    }

    public async Task OnRemoveDataClick(ReportColumnGeneric<TColumn> column)
    {
        AddedDataColumns.Remove(column);
    }

    public async Task OnRemoveDataMiningElementClick(ReportColumnGeneric<TColumn> column)
    {
        AddedDataMiningElementColumns.Remove(column);
    }

    public async Task OnRemoveCalculatingClick(ReportCalculatingColumn<TColumn> column)
    {
        AddedCalculatingColumns.Remove(column);
    }

    public async Task OnRemovePivotClick()
    {
        AddedPivotColumn = null;
    }

    public async Task OnDataColumnContextMenu(ReportColumnGeneric<TColumn> column, MouseEventArgs args)
    {
        SelectedDataColumn = column;
        SelectedCalculatingColumn = null;
        SelectedDataMiningColumn = null;
        IsSelectedPivotColumn = false;
        
        UpdateMenuItemsForDataColumn();
        
        if (ContextMenuRef != null)
        {
            await ContextMenuRef.ShowAsync(args.ClientX, args.ClientY);
        }
    }

    public async Task OnCalculatingColumnContextMenu(ReportCalculatingColumn<TColumn> column, MouseEventArgs args)
    {
        SelectedCalculatingColumn = column;
        SelectedDataColumn = null;
        SelectedDataMiningColumn = null;
        IsSelectedPivotColumn = false;
        
        UpdateMenuItemsForCalculatingColumn();
        
        if (ContextMenuRef != null)
        {
            await ContextMenuRef.ShowAsync(args.ClientX, args.ClientY);
        }
    }

    public async Task OnPivotColumnContextMenu(MouseEventArgs args)
    {
        IsSelectedPivotColumn = true;
        SelectedDataColumn = null;
        SelectedCalculatingColumn = null;
        SelectedDataMiningColumn = null;
        
        UpdateMenuItemsForPivotColumn();
        
        if (ContextMenuRef != null)
        {
            await ContextMenuRef.ShowAsync(args.ClientX, args.ClientY);
        }
    }

    public async Task OnDataMiningColumnContextMenu(ReportColumnGeneric<TColumn> column, MouseEventArgs args)
    {
        SelectedDataMiningColumn = column;
        SelectedDataColumn = null;
        SelectedCalculatingColumn = null;
        IsSelectedPivotColumn = false;
        
        UpdateMenuItemsForDataMiningColumn();
        
        if (ContextMenuRef != null)
        {
            await ContextMenuRef.ShowAsync(args.ClientX, args.ClientY);
        }
    }

    private void UpdateMenuItemsForDataColumn()
    {
        MenuItems[0].Disabled = !IsEditable;
        MenuItems[1].Disabled = !IsEditable;
        
        if (MenuItems[1].Items != null)
        {
            foreach (var item in MenuItems[1].Items)
            {
                item.Disabled = !IsEditable;
            }
        }
    }

    private void UpdateMenuItemsForCalculatingColumn()
    {
        MenuItems[0].Disabled = !IsEditable;
        MenuItems[1].Disabled = true;
        
        if (MenuItems[1].Items != null)
        {
            foreach (var item in MenuItems[1].Items)
            {
                item.Disabled = true;
            }
        }
    }

    private void UpdateMenuItemsForPivotColumn()
    {
        MenuItems[0].Disabled = !IsEditable;
        MenuItems[1].Disabled = true;
        
        if (MenuItems[1].Items != null)
        {
            foreach (var item in MenuItems[1].Items)
            {
                item.Disabled = true;
            }
        }
    }

    private void UpdateMenuItemsForDataMiningColumn()
    {
        MenuItems[0].Disabled = !IsEditable;
        MenuItems[1].Disabled = true;
        
        if (MenuItems[1].Items != null)
        {
            foreach (var item in MenuItems[1].Items)
            {
                item.Disabled = true;
            }
        }
    }

    public async Task OnContextMenuClick(TelerikContextMenuItem item)
    {
        if (item.Text == TextResources.APP_StringKeys_Delete)
        {
            await HandleDeleteAction();
        }
        else if (item.Text == TextResources.APP_StringKeys_Sort_Asc && SelectedDataColumn != null)
        {
            SelectedDataColumn.SortType = ReportColumnSortType.Asc;
        }
        else if (item.Text == TextResources.APP_StringKeys_Sort_Desc && SelectedDataColumn != null)
        {
            SelectedDataColumn.SortType = ReportColumnSortType.Desc;
        }
    }

    private async Task HandleDeleteAction()
    {
        if (SelectedDataColumn != null)
        {
            await OnRemoveDataClick(SelectedDataColumn);
        }
        else if (SelectedCalculatingColumn != null)
        {
            await OnRemoveCalculatingClick(SelectedCalculatingColumn);
        }
        else if (IsSelectedPivotColumn)
        {
            await OnRemovePivotClick();
        }
        else if (SelectedDataMiningColumn != null)
        {
            await OnRemoveDataMiningElementClick(SelectedDataMiningColumn);
        }
    }
}
