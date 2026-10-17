using System.Globalization;
using System.Text.Json;
using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Register.Pages;

public partial class MultiProductBatch
{
    public bool IsLoading = true;
    public string ActionId = string.Empty;
    public string Line = string.Empty;
    public string Shift = string.Empty;
    public string Warehouse = string.Empty;
    public string DocumentId = "000";
    public string PackValue = string.Empty;
    public int Count = 1;
    public PositionProductResponse? CurrentProduct;
    public ProductCodeModal ProductModal = default!;
    public List<GetAllLinesVm> Lines = new();
    public List<GetAllShiftsVm> Shifts = new();
    public List<GetAllWarehousesVm> Warehouses = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<BatchRow> Rows = new();

    private string _userCode = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;
    [Inject] public IFormalDataCache FormalCache { get; set; } = default!;

    protected override async Task SiloInitializer()
    {
        try
        {
            _userCode = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();
            Lines = await FormalCache.GetLines();
            Shifts = await FormalCache.GetShifts();
            Warehouses = (await FormalCache.GetWarehouses())
                .Where(p => p.IsActive && p.OperationalType == DestinationOperationalType.Product
                    && p.InventoryType == DestinationInventoryType.Physical).ToList();
            DynamicFields = (await Api.PostAsync<List<GetAllDynamicFieldVm>>("SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", 0))).Value?.OrderBy(p => p.Id).ToList() ?? new();
            DynamicFields.ForEach(p => p.Value = p.DefaultValue ?? string.Empty);
            await LoadActionId();
        }
        catch
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnChooseProductClick(MouseEventArgs e)
    {
        await ProductModal.Show();
    }

    public void OnProductSelected(PositionProductResponse product)
    {
        CurrentProduct = product;
        PackValue = product.ProductPackValue;
        Count = 1;
    }

    public void OnAddClick(MouseEventArgs e)
    {
        if (CurrentProduct is null || string.IsNullOrWhiteSpace(CurrentProduct.ProductCode)
            || string.IsNullOrWhiteSpace(Line) || string.IsNullOrWhiteSpace(Shift)
            || string.IsNullOrWhiteSpace(Warehouse) || string.IsNullOrWhiteSpace(ActionId)
            || string.IsNullOrWhiteSpace(DocumentId) || DocumentId == "0" || Count <= 0
            || Rows.Any(p => p.IsSaved)
            || !TryParseValue(PackValue, out var value) || value <= 0
            || !TryParseValue(CurrentProduct.ProductValue, out var productValue) || productValue <= 0
            || DynamicFields.Any(p => p.IsRequired == true && string.IsNullOrWhiteSpace(p.Value)))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        var properties = DynamicFields.GroupBy(p => p.Title)
            .ToDictionary(p => p.Key, p => p.Last().Value ?? string.Empty);
        Rows.Add(new BatchRow
        {
            ProductCode = CurrentProduct.ProductCode,
            ProductName = CurrentProduct.ProductName,
            TechnicalCode = CurrentProduct.TechnicalCode,
            Count = Count,
            PackValue = value,
            DocumentId = DocumentId,
            Line = Line,
            Shift = Shift,
            Warehouse = Warehouse,
            PropertiesJson = JsonSerializer.Serialize(properties)
        });

        CurrentProduct = null;
        PackValue = string.Empty;
        Count = 1;
    }

    public void OnRemoveClick(BatchRow row)
    {
        if (!IsLoading && !row.IsSaved)
        {
            Rows.Remove(row);
        }
    }

    public async Task OnSaveClick(MouseEventArgs e)
    {
        if (IsLoading || string.IsNullOrWhiteSpace(ActionId) || !Rows.Any(p => !p.IsSaved))
        {
            return;
        }

        IsLoading = true;
        try
        {
            foreach (var row in Rows.Where(p => !p.IsSaved))
            {
                var result = await Api.PostAsync<string>("SSavePrintBySerial",
                    new("ProductSerial", string.Empty),
                    new("ProductCode", row.ProductCode),
                    new("ProductCount", row.PackValue.ToString(CultureInfo.InvariantCulture)),
                    new("PrintActionId", ActionId),
                    new("ProductProductionLine", row.Line),
                    new("ProductProductionShift", row.Shift),
                    new("PrintCount", row.Count.ToString(CultureInfo.InvariantCulture)),
                    new("ProductContractType", "0"),
                    new("ProductOldSerial", string.Empty),
                    new("Location", string.Empty),
                    new("DocumentId", row.DocumentId),
                    new("PrintUser", _userCode),
                    new("WareHouseCode", row.Warehouse),
                    new("ProductProperties", JsonSerializer.Deserialize<JsonElement>(row.PropertiesJson)));

                if (result?.Successful != true || string.IsNullOrWhiteSpace(result.Value))
                {
                    Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
                    return;
                }

                row.IsSaved = true;
                await InvokeAsync(StateHasChanged);
            }

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        catch
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task OnNewClick(MouseEventArgs e)
    {
        if (IsLoading || Rows.Any(p => !p.IsSaved))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        IsLoading = true;
        try
        {
            await LoadActionId();
            if (!string.IsNullOrWhiteSpace(ActionId))
            {
                Rows.Clear();
                CurrentProduct = null;
                DocumentId = "000";
                PackValue = string.Empty;
                Count = 1;
                DynamicFields.ForEach(p => p.Value = p.DefaultValue ?? string.Empty);
            }
        }
        catch
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadActionId()
    {
        var result = await Api.PostAsync<string>("SGetNewPrintActionId", new KeyValuePair<string, object>("userToken", _userCode));
        if (result?.Successful != true || string.IsNullOrWhiteSpace(result.Value))
        {
            ActionId = string.Empty;
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        ActionId = result.Value;
    }

    private static bool TryParseValue(string input, out decimal value)
    {
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value)
            || decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    public sealed class BatchRow
    {
        public string ProductCode { get; init; } = string.Empty;
        public string ProductName { get; init; } = string.Empty;
        public string TechnicalCode { get; init; } = string.Empty;
        public int Count { get; init; }
        public decimal PackValue { get; init; }
        public decimal TotalValue => Math.Round(Count * PackValue, 2);
        public string DocumentId { get; init; } = string.Empty;
        public string Line { get; init; } = string.Empty;
        public string Shift { get; init; } = string.Empty;
        public string Warehouse { get; init; } = string.Empty;
        public string PropertiesJson { get; init; } = "{}";
        public bool IsSaved { get; set; }
    }
}
