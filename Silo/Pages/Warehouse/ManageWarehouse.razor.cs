using System.Text.Json;
using Silo.Application;
using Silo.Application.Shared.Features;

namespace Silo.Pages.Warehouse;
public partial class ManageWarehouse
{
    public bool IsLoading = true;
    public string UserToken;
    public bool IsDisabled = false;
    public GetAllWarehousesVm Warehouse = new();
    public List<GetAllWarehousesVm> Warehouses = new();
    public List<GetAllDestinationTypeDto> DestinationTypes = new();

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


        DestinationTypes = (await Api.SendAsyncObjectByUri<GetAllDestinationTypeVm>(HttpMethod.Get
                , "DestinationType/ReadAll")).Value.List;

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

        Warehouse.Epc = JsonSerializer.Serialize(new
        {
            AllowCancelTag = Warehouse.AllowCancelTag.ToString().ToLowerInvariant(),
            AllowRePrintTag = Warehouse.AllowRePrintTag.ToString().ToLowerInvariant(),
            AllowEditInformation = Warehouse.AllowEditInformation.ToString().ToLowerInvariant(),
            AllowEditTagValue = Warehouse.AllowEditTagValue.ToString().ToLowerInvariant()
        });

        int result = (await Api.PostAsync<int>("SSaveWarehouseWithDestinationType",
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

        Warehouse.DestinationTypeCode = warehouse.DestinationTypeCode;

        Warehouse.Epc = warehouse.Epc;

        LoadEpcSettings(warehouse.Epc);

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

        if (Warehouse.DestinationTypeCode.HasNoValue())
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

    private void LoadEpcSettings(string? epc)
    {
        Warehouse.AllowCancelTag = false;
        Warehouse.AllowRePrintTag = false;
        Warehouse.AllowEditInformation = false;
        Warehouse.AllowEditTagValue = false;

        if (string.IsNullOrWhiteSpace(epc))
            return;

        using var document = JsonDocument.Parse(epc);
        var root = document.RootElement;

        Warehouse.AllowCancelTag = GetEpcBoolean(root, "AllowCancelTag");
        Warehouse.AllowRePrintTag = GetEpcBoolean(root, "AllowRePrintTag");
        Warehouse.AllowEditInformation = GetEpcBoolean(root, "AllowEditInformation");
        Warehouse.AllowEditTagValue = GetEpcBoolean(root, "AllowEditTagValue");
    }

    private static bool GetEpcBoolean(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value))
            return false;

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String =>
                bool.TryParse(value.GetString(), out var result) && result,
            _ => false
        };
    }
}
