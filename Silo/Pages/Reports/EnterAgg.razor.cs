using Silo.Application;

namespace Silo.Pages.Reports;
public partial class EnterAgg
{
    public bool IsLoading = true;
    public List<ReportFilter> Filters = new();
    public IEnumerable<object> ExpandedPanels;
    public List<TelerikPanelBarTempleteItem> Panels;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    protected override async Task SiloInitializer()
    {
        LoadPanels();

        var Warehouses = await FormalCache.GetWarehouses();

        var Groups = await FormalCache.GetGroups();

        Filters.Add(new()
        {
            Id = 1,
            Label = TextResources.APP_StringKeys_Chart_Regcode,
            Component = FilterComponent.Text
        });

        Filters.Add(new()
        {
            Id = 2,
            Label = TextResources.APP_StringKeys_Field_Warehouse_Title,
            Component = FilterComponent.Modal,
            Items = Warehouses.Select(p=> new ReportDataItem()
            {
                Label = p.DestinationTitle,
                Value = p.DestinationCode
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = 3,
            Label = TextResources.APP_StringKeys_Group,
            Component = FilterComponent.Drop,
            Items = Groups.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = 4,
            Label = TextResources.APP_StringKeys_Date,
            Component = FilterComponent.PersianDate
        });

        IsLoading = false;
    }

    private void LoadPanels()
    {
        Panels = new()
        {
            new ()
            {
                Id = 1,
                Text = TextResources.APP_StringKeys_Filters
            },
            new ()
            {
                Id = 2,
                Text = TextResources.APP_StringKeys_Filters_Results
            }
        };

        ExpandedPanels = new List<object> { Panels.First() };
    }
}
