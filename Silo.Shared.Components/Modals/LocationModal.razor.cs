using Silo.Application;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Modals;
public partial class LocationModal
{
    public bool IsLoading = false;
    public string ProductCode = string.Empty;
    public string OccupiedCapacity = string.Empty;
    public string Location = string.Empty;
    public string Warehouse = string.Empty;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllZonesVm> Zones = new();
    public bool LocationLike { get; set; } = false;
    public List<object> Capacities = new()
    {
        new
        {
            Title = "خالی",
            Value = "0"
        },
        new
        {
            Title = "کمتر از 20 درصد",
            Value = "20"
        },
        new
        {
            Title = "20 تا 40 درصد",
            Value = "40"
        },
        new
        {
            Title = "40 تا 60 درصد",
            Value = "60"
        },
        new
        {
            Title = "60 تا 80 درصد",
            Value = "80"
        },
        new
        {
            Title = "بیش از 80 درصد",
            Value = "100"
        },
    };

    public TelerikGrid<GetAllZonesVm> GridZones { get; set; }

    [Parameter] public string WarehouseCode { get; set; }
    [Parameter] public DestinationOperationalType WarehouseType { get; set; } = DestinationOperationalType.NotSpecified;
    [Parameter] public EventCallback<string> OnClickLocation { get; set; }
    [Parameter] public EventCallback<GetAllZonesVm> OnClickLocationWithZone { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    public Modal Modal { get; set; }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        IsLoading = true;

        int minCapacity = OccupiedCapacity switch
        {
            null or "" => -1,
            "0" => 0,
            "20" => 0,
            "40" => 20,
            "60" => 40,
            "80" => 60,
            "100" => 80
        };

        int maxCapacity = OccupiedCapacity switch
        {
            null or "" => -1,
            "0" => 0,
            "20" => 20,
            "40" => 40,
            "60" => 60,
            "80" => 80,
            "100" => 100
        };

        if (minCapacity == 0)
            maxCapacity = -1;

        Zones = (await Api.PostAsync<List<GetAllZonesVm>>("SSearchZone"
            , new("productCode", "-1")
            , new("minCap", minCapacity)
            , new("maxCap", maxCapacity)
            , new("zoneCode", Location.HasNoValue() ? "-1" : Location)
            , new("location", Warehouse.HasNoValue() ? "-1" : Warehouse)
            , new("warehouseType", WarehouseType)
            , new("zoneCodeLike", LocationLike)
        )).Value;

        GridZones.Data = Zones;

        GridZones.Rebind();

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        OccupiedCapacity = "";
        
        Location = "";
     
        Warehouse = "";

        Zones.Clear();

        LocationLike = false;
    }

    public async Task OnChooseLocation(GetAllZonesVm zone)
    {
        await OnClickLocationWithZone.InvokeAsync(zone);

        await OnClickLocation.InvokeAsync(zone.ZoneCode);

        await Modal.Close(new());
    }

    public async Task Show()
    {
        IsLoading = true;

        ProductCode = string.Empty;

        OccupiedCapacity = string.Empty;

        Zones = null;

        Location = string.Empty;

        Warehouse = string.Empty;

        if (WarehouseCode.HasValue())
        {
            Warehouse = WarehouseCode;
        }

        Warehouses = await FormalCache.GetWarehouses();

        LocationLike = false;

        await Modal.Open(new());

        IsLoading = false;
    }
}
