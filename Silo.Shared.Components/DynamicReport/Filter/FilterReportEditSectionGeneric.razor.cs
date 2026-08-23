using Silo.Application.Dto;
using Silo.Shared.Components.Modals;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class FilterReportEditSectionGeneric<TItem> where TItem : struct
{
    [Parameter][EditorRequired] public List<ReportFilterGeneric<TItem>> Filters { get; set; }
    [Parameter] public string InputClass { get; set; }

    [Parameter] public EventCallback<ReportFilterGeneric<TItem>> OnRemoveClick { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationCodeModal { get; set; }
    public Modal ModalDetails { get; set; }
    public TelerikContextMenu<TelerikContextMenuItem> ContextMenuRef { get; set; }
    
    public List<TelerikContextMenuItem> MenuItems { get; set; }
    public ReportFilterGeneric<TItem> SelectedFilter { get; set; }

    protected override void OnInitialized()
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
            }
        };
    }

    public async Task OnFilterRemoveClick(ReportFilterGeneric<TItem> filter)
    {
        await OnRemoveClick.InvokeAsync(filter);
    }

    public async Task OnModalCheckboxChange(ReportFilterGeneric<TItem> filter)
    {
        var items = filter.Items.Where(p => p.IsChoosen).ToList();

        if (items.Any())
        {
            filter.Value = string.Join(',', items.Select(p => p.Value));
        }
        else
        {
            filter.Value = string.Empty;
        }
    }

    public async Task OnModalOpenClick(ReportFilterGeneric<TItem> filter)
    {
        filter.Items.ForEach(p => p.IsChoosen = false);

        await ModalDetails.Open(new());
    }

    public async Task OnFilterContextMenu(ReportFilterGeneric<TItem> filter, Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        SelectedFilter = filter;
        
        MenuItems[0].Disabled = !filter.IsEditable;
        
        if (ContextMenuRef != null)
        {
            await ContextMenuRef.ShowAsync(args.ClientX, args.ClientY);
        }
    }

    public async Task OnContextMenuClick(TelerikContextMenuItem item)
    {
        if (item.Text == TextResources.APP_StringKeys_Delete && SelectedFilter != null)
        {
            await OnFilterRemoveClick(SelectedFilter);
        }
    }
}
