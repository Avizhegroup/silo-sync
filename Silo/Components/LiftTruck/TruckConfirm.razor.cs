using Silo.Application;
using Silo.Infrastructure.Web;

namespace Silo.Components.LiftTruck;
public partial class TruckConfirm
{
    public TruckCargoDto Cargo;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllZonesVm> Zones;

    [Parameter] public EventCallback<TruckConfirmMode> OnChangeConfirmMode { get; set; }
  
    [CascadingParameter] public TelerikLoaderContainer LoadingContainer { get; set; }
    [CascadingParameter] public RfidConnectApi Api { get; set; }

    [Inject] public IFormalDataCache FormalCache { get; set; }

    protected override async Task OnInitializedAsync()
    {
        LoadingContainer.Visible = true;

        Warehouses = await FormalCache.GetWarehouses();


        LoadingContainer.Visible = false;
    }

    public async Task Show(TruckCargoDto cargo)
    {
        Cargo = cargo;

        if (Cargo.DestinationZoneCode.HasValue()
           && Cargo.DestinationWarehouseCode.HasValue())
        {
            Zones = (await Api.PostAsync<List<GetAllZonesVm>>("SSearchZone"
                , new("productCode", "-1")
                , new("minCap", -1)
                , new("maxCap", -1)
                , new("zoneCode", Cargo.DestinationZoneCode)
                , new("location", Cargo.DestinationWarehouseCode)
                , new("location", DestinationOperationalType.NotSpecified)
            )).Value;
        }

        StateHasChanged();
    }

    public void Hide()
    {
        Cargo = null;
    }

    public async Task OnSelectWarehouseChange(ChangeEventArgs e)
    {
        LoadingContainer.Visible = true;

        Zones = (await Api.PostAsync<List<GetAllZonesVm>>("SSearchZone"
            , new("productCode", "-1")
            , new("minCap", -1)
            , new("maxCap", -1)
            , new("zoneCode", "-1")
            , new("location", Cargo.DestinationWarehouseCode)
            , new("warehouseType", DestinationOperationalType.NotSpecified))).Value;

        LoadingContainer.Visible = false;
    }

    public async Task OnVerifyClick(MouseEventArgs e)
    {
        Hide();

        await OnChangeConfirmMode.InvokeAsync(TruckConfirmMode.Verify);
    }

    public async Task OnCancelClick(MouseEventArgs e)
    {
        Hide();

        await OnChangeConfirmMode.InvokeAsync(TruckConfirmMode.Cancel);
    }

    public async Task OnBackClick(MouseEventArgs e)
    {
        Hide();

        await OnChangeConfirmMode.InvokeAsync(TruckConfirmMode.Back);
    }
}
