using Silo.Application;

namespace Silo.Pages.Warehouse;
public partial class ManageWarehouse
{
    public bool IsLoading = true;
    public string UserToken;
    public bool IsDisabled = false;
    public GetAllWarehousesVm Warehouse = new();
    public List<GetAllWarehousesVm> Warehouses = new();
    public List<TelerikDropDownItemGeneric<DestinationOperationalType>> OperationalType = new()
    {
        new()
        {
            Name= TextResources.APP_StringKeys_NotChoosed,
            Value = DestinationOperationalType.NotSpecified
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Production_Warehouse,
            Value = DestinationOperationalType.Quarentine
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Product_Warehouse,
            Value = DestinationOperationalType.Product
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Material_Warehouse,
            Value = DestinationOperationalType.Material
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Waste_Warehouse,
            Value = DestinationOperationalType.Waste
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Loading_Warehouse,
            Value = DestinationOperationalType.Loading
        },
        new()
        {
            Name= "انبار خروج",
            Value = DestinationOperationalType.Sales
        }

    };
    public List<TelerikDropDownItemGeneric<DestinationInventoryType>> InventoryType = new()
    {
        new()
        {
            Name= TextResources.APP_StringKeys_NotChoosed,
            Value = DestinationInventoryType.NotSpecified
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Virtual,
            Value = DestinationInventoryType.Virtual
        },
        new()
        {
            Name= TextResources.APP_StringKeys_Physical,
            Value = DestinationInventoryType.Physical
        }
    };

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal ModalRemove { get; set; }
    public Modal ModalWarehouses { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        UserToken = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        await RefreshWarehousesData();

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        Warehouse = new();

        IsDisabled = false;
    }

    public async Task OnSubmitClick(MouseEventArgs e)
    {
        if (!CheckIsFormValid())
        {
            return;
        }

        Warehouse.IsDefault = false;

        int result = (await Api.PostAsync<int>("SSaveWarehouse",
            new KeyValuePair<string, object>("warehouse", Warehouse))).Value;

        if (Warehouse.Id == 0)
        {
            Warehouse.Id = result;
        }

        await RefreshWarehousesData();

        await FormalCache.UpdateWarehouses(Warehouses);

        IsDisabled = (Warehouse.IsDefault && result > 0) ? true : false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }

    public async Task OnRemoveModalClick(MouseEventArgs e)
    {
        if (Warehouse.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        if (IsDisabled)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Delete_Default_Forbidden, "error");

            return;
        }

        await ModalRemove.Open(e);
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        bool result = (await Api.PostAsync<bool>("SDeleteWarehouse",
                          new KeyValuePair<string, object>("warehouseId", Warehouse.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await RefreshWarehousesData();

        await OnClearClick(e);
    }

    public async Task OnSelectWarehouse(GetAllWarehousesVm warehouse)
    {
        Warehouse.DestinationCode = warehouse.DestinationCode;

        Warehouse.DestinationTitle = warehouse.DestinationTitle;

        Warehouse.OperationalType = warehouse.OperationalType;

        Warehouse.InventoryType = warehouse.InventoryType;

        Warehouse.IsDefault = warehouse.IsDefault;

        Warehouse.IsActive = warehouse.IsActive;

        Warehouse.Id = warehouse.Id;

        IsDisabled = warehouse.IsDefault;

        await ModalWarehouses.Close(new());
    }

    public async Task RefreshWarehousesData()
    {
        IsLoading = true;
        Warehouses = (await Api.PostAsyncByContext<List<GetAllWarehousesVm>>("SGetAllWarehouses"
              , new GetAllWarehousesVmContext())).Value;


        IsLoading = false;
    }

    private bool CheckIsFormValid()
    {
        string validationString = "";

        if (IsDisabled)
        {
            validationString = TextResources.APP_StringKeys_Validation_Edit_Default_Forbidden;
        }

        if (Warehouse.DestinationCode.HasNoValue())
        {
            validationString = TextResources.APP_StringKeys_Field_Warehouse_Code + " " + TextResources.APP_StringKeys_Validation_EmptyError;
        }

        if (Warehouse.DestinationTitle.HasNoValue())
        {
            validationString = TextResources.APP_StringKeys_Field_Warehouse_Title + " " + TextResources.APP_StringKeys_Validation_EmptyError;
        }

        if (Warehouse.OperationalType == DestinationOperationalType.NotSpecified)
        {
            validationString = TextResources.APP_StringKeys_Validation_Operational_Type;
        }

        if (Warehouse.InventoryType == DestinationInventoryType.NotSpecified)
        {
            validationString = TextResources.APP_StringKeys_Validation_Inventory_Type;
        }

        if (validationString.HasValue())
        {
            Notification.Show(validationString, "error");

            return false;
        }

        return true;
    }
}
