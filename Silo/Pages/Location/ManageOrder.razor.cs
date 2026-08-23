using Silo.Application.Features;

namespace Silo.Pages.Location;

public partial class ManageOrder
{
    public PlacementOrderDto Filter = new();
    public SavePlacementOrderCommand NewOrder = new();
    public List<GetAllPlacementOrderByProductCodeVm> Orders;
    public List<GetAllPlacementOrderByProductCodeVm> OrdersFiltered;
    public bool IsLoading = true;
    public bool IsModalProductCodeForFilter = true;
    public bool IsModalLocationForFilter = true;
    public string IgnoredOrderCode = string.Empty;
    public string MessageText = string.Empty;
    public string TempLocation = string.Empty;
    public string CurrentUser = string.Empty;
    public List<TelerikDropDownItem> FilterStatus = new()
    {
        new()
        {
            Name="فعال",
            Value = "1"
        },
         new()
        {
            Name="اتمام",
            Value = "2"
        },
        new()
        {
            Name="لغو شده",
            Value = "3"
        }
    };

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationModal { get; set; }
    public Modal ModalIgnore { get; set; }
    public Modal ModalMessage { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Orders = (await Api.PostAsync<List<GetAllPlacementOrderByProductCodeVm>>("SPGetPlacementOrdersList",
               new KeyValuePair<string, object>[] { new("type", 1) })).Value;

        OrdersFiltered = Orders;

        CurrentUser = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUsername();

        IsLoading = false;
    }

    public async Task OnFilterClick(MouseEventArgs e)
    {
        IsLoading = true;

        OrdersFiltered = Orders;

        if (Filter.ProductCode.HasValue())
        {
            OrdersFiltered = OrdersFiltered.Where(p => p.ProductCode.Equals(Filter.ProductCode)).ToList();
        }

        if (Filter.Location.HasValue())
        {
            OrdersFiltered = OrdersFiltered.Where(p => p.Zones.Contains(Filter.Location)).ToList();
        }

        if (Filter.Status.HasValue())
        {
            OrdersFiltered = OrdersFiltered.Where(p => p.Status.ToString().Equals(Filter.Status)).ToList();
        }

        IsLoading = false;
    }

    public async Task OnClearFilterClick(MouseEventArgs e)
    {
        Filter = new();
    }

    public async Task OnChooseProductCode(string productCode)
    {
        if (IsModalProductCodeForFilter)
        {
            Filter.ProductCode = productCode;
        }
        else
        {
            NewOrder.ProductCode = productCode;
        }
    }

    public async Task OnChooseProductTitle(string productTitle)
    {
        if (!IsModalProductCodeForFilter)
        {
            NewOrder.ProductName = productTitle;
        }
    }

    public async Task OnChooseLocation(string location)
    {
        if (IsModalLocationForFilter)
        {
            Filter.Location = location;
        }
        else
        {
            TempLocation = location;
        }
    }

    public async Task OnChooseOrderForIgnore(string orderCode)
    {
        IgnoredOrderCode = orderCode;

        await ModalIgnore.Open(new());
    }

    public async Task OnIgnoreClick(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SDeleteReportCargoByActionId"
                , new KeyValuePair<string, object>[] { new("actionId", IgnoredOrderCode) })).Value;

        if (!result)
        {
            MessageText = TextResources.APP_StringKeys_Alert_Fail;

            await ModalMessage.Open(e);
        }

        Orders.FirstOrDefault(p=>p.OrderCode.Equals(int.Parse(IgnoredOrderCode))).Status = 3;

        IgnoredOrderCode = string.Empty;

        IsLoading = false;
    }

    public async Task OnOpenLocationModal(bool isFilter)
    {
        IsModalLocationForFilter = isFilter;

        await LocationModal.Show();
    }

    public async Task OnOpenProductCodeModal(bool isFilter)
    {
        IsModalProductCodeForFilter = isFilter;

        await ProductCodeModal.Show();
    }

    public async Task OnAddZone(MouseEventArgs e)
    {
        if (TempLocation.HasNoValue())
        {
            return;
        }

        IsLoading = true;

        int locationRemainCount = (await Api.PostAsync<int>("SGetRemainZoneCapacity",
                new("storeCode", "1")
                , new("zoneCode", TempLocation)
            )).Value;

        NewOrder.Count = (int.Parse(NewOrder.Count) + locationRemainCount).ToString();

        if (NewOrder.Zones.Any())
        {
            NewOrder.Zones += " | ";
        }

        NewOrder.Zones += TempLocation;

        TempLocation = string.Empty;

        IsLoading = false;
    }

    public async Task OnRecalculateClick(MouseEventArgs e)
    {
        IsLoading = true;

        int count = (await Api.PostAsync<int>("SGetRemainZoneCapacityMulti",
                  new("storeCode", "1")
                , new("zones", NewOrder.Zones)
            )).Value;

        NewOrder.Count = count.ToString();

        IsLoading = false;
    }

    public async Task OnSaveOrder(MouseEventArgs e)
    {
        if (!NewOrder.Zones.Any())
        {
            MessageText = TextResources.APP_StringKeys_Validation_Empty;

           await ModalMessage.Open(new());

            return;
        }

        IsLoading = true;

        int result = (await Api.PostAsync<int>("SPSavePlacementOrders",
                new("userToken", CurrentUser)
                , new("productCode", NewOrder.ProductCode)
                , new("productLine", "0")
                , new("productShift", "0")
                , new("packCount", NewOrder.Count.ToString())
                , new("storeCode", "1")
                , new("zoneList", NewOrder.Zones.Split('|').ToList())
                , new("pOCode", "-1")
                , new("truck", 0)
                , new("fromZoneCode", "0")
                , new("type", "1")
            )).Value;

        if (result != -1)
        {
            NewOrder = new();

            Orders = (await Api.PostAsync<List<GetAllPlacementOrderByProductCodeVm>>("SPGetPlacementOrdersList",
             new KeyValuePair<string, object>[] { new("type", 1) })).Value;

            OrdersFiltered = Orders;

            MessageText = TextResources.APP_StringKeys_Alert_Success;
        }
        else
        {
            MessageText = TextResources.APP_StringKeys_Alert_Fail;
        }

        await ModalMessage.Open(new());

        IsLoading = false;
    }
}
