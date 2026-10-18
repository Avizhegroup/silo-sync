using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Silo.Application;
using Silo.Application.Features;
using Silo.Modules.Register.Models;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Register.Pages;

/// <summary>
/// Defines a print/registration batch (legacy F2): production data, document ("roll code"), product(s) and count,
/// then creates the print rows on the server. Two modes: several products in one operation (list, then confirm) or a
/// single product saved directly, optionally with an explicit serial list (range or Excel).
/// </summary>
public partial class MultiProductBatch
{
    private const string DocumentTypeId = "1";

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

    public bool IsSingleMode { get; set; }
    public bool CheckRemaining { get; set; } = true;

    // Document ("roll code").
    public bool ShowDocuments { get; set; }
    public string DocumentSearch { get; set; } = string.Empty;
    public List<DocumentInfo> Documents { get; set; } = [];
    public DocumentInfo? LoadedDocument { get; set; }
    public List<DocumentItemInfo> DocumentItems { get; set; } = [];
    public List<KeyValuePair<string, string>> HeaderRows { get; set; } = [];
    public Dictionary<string, decimal> Remain { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // Serial list (single mode).
    public string SerialMode { get; set; } = "range";
    public string RangeFrom { get; set; } = "0";
    public string RangeTo { get; set; } = "0";
    public int CutStart { get; set; }
    public int CutLength { get; set; }
    public bool ExcelHasHeader { get; set; }
    public List<string> Serials { get; set; } = [];

    private string _userCode = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; } = default!;
    [Inject] public IFormalDataCache FormalCache { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    public IEnumerable<DocumentInfo> FilteredDocuments => string.IsNullOrWhiteSpace(DocumentSearch)
        ? Documents
        : Documents.Where(p => p.Key.Contains(DocumentSearch.Trim(), StringComparison.OrdinalIgnoreCase));

    public bool HasSavedRows => Rows.Any(p => p.IsSaved);

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
            ApplyDefaults();
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

    /// <summary>Legacy: a list with exactly one item is selected for the operator; the shift follows the clock.</summary>
    private void ApplyDefaults()
    {
        if (Lines.Count == 1)
        {
            Line = Lines[0].Code;
        }

        if (Warehouses.Count == 1)
        {
            Warehouse = Warehouses[0].DestinationCode;
        }

        if (Shifts.Count == 1)
        {
            Shift = Shifts[0].Code;
        }
        else if (Shifts.Count > 1)
        {
            // 06:00-13:59 shift 1, 14:00-21:59 shift 2, otherwise shift 3 (legacy never reached its third branch).
            var hour = DateTime.Now.Hour;
            var number = hour is >= 6 and < 14 ? '1' : hour is >= 14 and < 22 ? '2' : '3';
            var matches = Shifts.Where(p => (p.Title ?? string.Empty).Contains(number)
                && !(p.Title ?? string.Empty).Any(c => char.IsDigit(c) && c != number)).ToList();
            if (matches.Count == 1)
            {
                Shift = matches[0].Code;
            }
        }
    }

    public async Task OnChooseProductClick(MouseEventArgs e)
    {
        await ProductModal.Show();
    }

    public void OnProductSelected(PositionProductResponse product)
    {
        // Legacy: with a document loaded only the document's products can be chosen.
        if (LoadedDocument is not null && DocumentItems.Count > 0
            && !DocumentItems.Any(p => string.Equals(p.ProductCode, product.ProductCode, StringComparison.OrdinalIgnoreCase)))
        {
            Notification.Show("The selected product is not among the items of the document.", "error");
            return;
        }

        CurrentProduct = product;
        PackValue = product.ProductPackValue;
        Count = 1;
    }

    #region Document

    public async Task OnDocumentChanged(ChangeEventArgs e)
    {
        DocumentId = (e.Value?.ToString() ?? string.Empty).Trim();
        await LoadDocumentAsync();
    }

    public async Task ToggleDocumentsAsync()
    {
        ShowDocuments = !ShowDocuments;
        if (!ShowDocuments || Documents.Count > 0)
        {
            return;
        }

        IsLoading = true;
        try
        {
            Documents = (await Api.PostAsync<List<DocumentInfo>>("SGetDocumentsByDocType"
                , new KeyValuePair<string, object>("documentType", DocumentTypeId)
                , new KeyValuePair<string, object>("limit", 300))).Value ?? [];
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

    public async Task SelectDocumentAsync(string key)
    {
        DocumentId = key;
        ShowDocuments = false;
        await LoadDocumentAsync();
    }

    private void ResetDocument()
    {
        LoadedDocument = null;
        DocumentItems = [];
        HeaderRows = [];
        Remain.Clear();
    }

    public async Task LoadDocumentAsync()
    {
        ResetDocument();

        if (DocumentKey.IsNone(DocumentId))
        {
            return;
        }

        if (!DocumentKey.IsValid(DocumentId))
        {
            Notification.Show("The document code contains characters that are not allowed.", "error");
            return;
        }

        IsLoading = true;
        try
        {
            var document = (await Api.PostAsync<DocumentInfo>("SGetDocumentData"
                , new KeyValuePair<string, object>("documentKey", DocumentId)
                , new KeyValuePair<string, object>("documentType", DocumentTypeId))).Value;

            if (document is null || string.IsNullOrWhiteSpace(document.Key))
            {
                Notification.Show($"No data was found for document {DocumentId}.", "warning");
                return;
            }

            LoadedDocument = document;
            DocumentItems = (document.DocumentItems ?? [])
                .Where(p => !string.IsNullOrWhiteSpace(p.ProductCode))
                .GroupBy(p => p.ProductCode!, StringComparer.OrdinalIgnoreCase)
                .Select(p => new DocumentItemInfo
                {
                    ProductCode = p.Key,
                    ProductTitle = p.First().ProductTitle,
                    Count = p.Sum(x => x.Count)
                }).ToList();

            ApplyHeader(document.HeaderData);
            await LoadRemainAsync();
        }
        catch
        {
            ResetDocument();
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>Shows the document header and copies each value into the dynamic field of the same title when that is still empty.</summary>
    private void ApplyHeader(string? headerData)
    {
        HeaderRows = [];
        if (string.IsNullOrWhiteSpace(headerData))
        {
            return;
        }

        try
        {
            using var json = JsonDocument.Parse(headerData);
            if (json.RootElement.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            foreach (var property in json.RootElement.EnumerateObject())
            {
                var value = property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString() ?? string.Empty
                    : property.Value.GetRawText();
                HeaderRows.Add(new(property.Name, value));

                var field = DynamicFields.FirstOrDefault(p => p.Title == property.Name && string.IsNullOrWhiteSpace(p.Value));
                if (field is not null)
                {
                    field.Value = value;
                }
            }
        }
        catch (JsonException)
        {
            // A header that is not a JSON object is simply not displayed.
        }
    }

    private async Task LoadRemainAsync()
    {
        Remain.Clear();
        DocumentItems.ForEach(p => p.Remain = null);

        if (LoadedDocument is null || !DocumentKey.IsValid(LoadedDocument.Key))
        {
            return;
        }

        var rows = (await Api.PostAsync<List<RemainRow>>("SGetRemainRegisterByDoc"
            , new KeyValuePair<string, object>("DocumentId", LoadedDocument.Key))).Value ?? [];

        foreach (var row in rows.Where(p => !string.IsNullOrWhiteSpace(p.ProductCode)))
        {
            Remain[row.ProductCode!] = Remain.GetValueOrDefault(row.ProductCode!) + row.Count;
        }

        RefreshItemRemain();
    }

    /// <summary>Remaining quantity of a document product minus what the unsaved rows of this page already claim.</summary>
    private decimal AvailableFor(string productCode)
    {
        var pending = Rows.Where(p => !p.IsSaved && p.DocumentId == DocumentId
                && string.Equals(p.ProductCode, productCode, StringComparison.OrdinalIgnoreCase))
            .Sum(p => p.Count * p.UnitsPerTag);
        return Remain.GetValueOrDefault(productCode) - pending;
    }

    private void RefreshItemRemain()
    {
        foreach (var item in DocumentItems)
        {
            item.Remain = Remain.ContainsKey(item.ProductCode!) ? AvailableFor(item.ProductCode!) : null;
        }
    }

    public async Task SelectDocumentItemAsync(DocumentItemInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.ProductCode) || IsLoading)
        {
            return;
        }

        IsLoading = true;
        try
        {
            var request = new PositionProductRequest
            {
                MProductCode = item.ProductCode,
                MProductTitle = "-1",
                MQuality = "-1",
                MSize = "-1",
                MTechCode = "-1"
            };
            var products = (await Api.PostAsync<List<PositionProductResponse>>("SPSearchProductWeb"
                , new KeyValuePair<string, object>("search", request))).Value ?? [];
            var product = products.FirstOrDefault(p => string.Equals(p.ProductCode, item.ProductCode, StringComparison.OrdinalIgnoreCase));

            if (product is null)
            {
                Notification.Show($"Product {item.ProductCode} was not found.", "error");
                return;
            }

            OnProductSelected(product);

            // Legacy fills the count with the remaining quantity; here it is converted to a number of tags.
            if (ReferenceEquals(CurrentProduct, product) && item.Remain is > 0
                && TryParseValue(PackValue, out var pack) && TryParseValue(product.ProductValue, out var unit) && unit > 0 && pack > 0)
            {
                Count = (int)Math.Max(1, Math.Ceiling(item.Remain.Value / Math.Round(pack / unit, 2)));
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

    #endregion

    #region Serial list

    public void GenerateRange()
    {
        if (SerialListBuilder.TryRange(RangeFrom, RangeTo, CutStart, CutLength, out var serials, out var error))
        {
            Serials = serials;
            Notification.Show($"{serials.Count} serial(s) prepared.", "success");
        }
        else
        {
            Notification.Show(error, "error");
        }
    }

    public async Task OnExcelSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file.Size > SerialListBuilder.MaxExcelBytes)
        {
            Notification.Show("The file is larger than 5 MB.", "error");
            return;
        }

        try
        {
            await using var memory = new MemoryStream();
            await using (var stream = file.OpenReadStream(SerialListBuilder.MaxExcelBytes))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            if (SerialListBuilder.TryExcel(memory, ExcelHasHeader, out var serials, out var error))
            {
                Serials = serials;
                Notification.Show($"{serials.Count} serial(s) read from {file.Name}.", "success");
            }
            else
            {
                Notification.Show(error, "error");
            }
        }
        catch (IOException)
        {
            Notification.Show("The file could not be read.", "error");
        }
    }

    public void ClearSerials() => Serials = [];

    #endregion

    #region Rows and save

    public async Task OnAddClick(MouseEventArgs e)
    {
        if (IsLoading)
        {
            return;
        }

        if (CurrentProduct is null || string.IsNullOrWhiteSpace(CurrentProduct.ProductCode)
            || string.IsNullOrWhiteSpace(Line) || string.IsNullOrWhiteSpace(Shift)
            || string.IsNullOrWhiteSpace(Warehouse) || string.IsNullOrWhiteSpace(ActionId)
            || string.IsNullOrWhiteSpace(DocumentId) || DocumentId == "0"
            || HasSavedRows
            || !TryParseValue(PackValue, out var value) || value <= 0
            || !TryParseValue(CurrentProduct.ProductValue, out var productValue) || productValue <= 0
            || DynamicFields.Any(p => p.IsRequired == true && string.IsNullOrWhiteSpace(p.Value)))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        var useSerials = IsSingleMode && Serials.Count > 0;
        var count = useSerials ? Serials.Count : Count;
        if (count <= 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }

        var unitsPerTag = Math.Round(value / productValue, 2);
        var requested = count * unitsPerTag;

        if (CheckRemaining && LoadedDocument is not null && Remain.ContainsKey(CurrentProduct.ProductCode)
            && AvailableFor(CurrentProduct.ProductCode) < requested)
        {
            Notification.Show("The quantity is greater than what remains in the production document.", "error");
            return;
        }

        var properties = DynamicFields.GroupBy(p => p.Title)
            .ToDictionary(p => p.Key, p => p.Last().Value ?? string.Empty);
        var row = new BatchRow
        {
            ProductCode = CurrentProduct.ProductCode,
            ProductName = CurrentProduct.ProductName,
            TechnicalCode = CurrentProduct.TechnicalCode,
            Count = count,
            PackValue = value,
            UnitsPerTag = unitsPerTag,
            DocumentId = DocumentId,
            Line = Line,
            Shift = Shift,
            Warehouse = Warehouse,
            PropertiesJson = JsonSerializer.Serialize(properties),
            Serials = useSerials ? [.. Serials] : null
        };

        if (IsSingleMode)
        {
            var confirmed = await JS.InvokeAsync<bool>("confirm",
                $"Save {count} serial(s) of {row.ProductName} ({row.TechnicalCode}) with document {row.DocumentId}?");
            if (!confirmed)
            {
                return;
            }

            Rows.Add(row);
            await SaveRowsAsync();
            return;
        }

        Rows.Add(row);
        CurrentProduct = null;
        PackValue = string.Empty;
        Count = 1;
        RefreshItemRemain();
    }

    public void OnRemoveClick(BatchRow row)
    {
        if (!IsLoading && !row.IsSaved && row.SavedCount == 0)
        {
            Rows.Remove(row);
            RefreshItemRemain();
        }
    }

    public async Task OnSaveClick(MouseEventArgs e) => await SaveRowsAsync();

    private async Task SaveRowsAsync()
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
                if (!await SaveRowAsync(row))
                {
                    Notification.Show(row.SavedCount > 0
                        ? $"Saving stopped after {row.SavedCount} of {row.Serials!.Count} serial(s). Press Confirm to continue."
                        : TextResources.APP_StringKeys_Alert_Fail, "error");
                    return;
                }

                await InvokeAsync(StateHasChanged);
            }

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
            await LoadRemainAsync();
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

    /// <summary>One request for a generated batch, or one request per listed serial (resumable after a failure).</summary>
    private async Task<bool> SaveRowAsync(BatchRow row)
    {
        if (row.Serials is { Count: > 0 })
        {
            while (row.SavedCount < row.Serials.Count)
            {
                if (!await PostSaveAsync(row, row.Serials[row.SavedCount], "1"))
                {
                    return false;
                }

                row.SavedCount++;
                if (row.SavedCount % 25 == 0)
                {
                    await InvokeAsync(StateHasChanged);
                }
            }
        }
        else if (!await PostSaveAsync(row, string.Empty, row.Count.ToString(CultureInfo.InvariantCulture)))
        {
            return false;
        }

        row.IsSaved = true;
        return true;
    }

    private async Task<bool> PostSaveAsync(BatchRow row, string serial, string printCount)
    {
        var result = await Api.PostAsync<string>("SSavePrintBySerial",
            new("ProductSerial", serial),
            new("ProductCode", row.ProductCode),
            new("ProductCount", row.PackValue.ToString(CultureInfo.InvariantCulture)),
            new("PrintActionId", ActionId),
            new("ProductProductionLine", row.Line),
            new("ProductProductionShift", row.Shift),
            new("PrintCount", printCount),
            new("ProductContractType", "0"),
            new("ProductOldSerial", serial),
            new("Location", "0"),
            new("DocumentId", row.DocumentId),
            new("PrintUser", _userCode),
            new("WareHouseCode", row.Warehouse),
            new("ProductProperties", JsonSerializer.Deserialize<JsonElement>(row.PropertiesJson)));

        return result?.Successful == true && !string.IsNullOrWhiteSpace(result.Value);
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
                ResetDocument();
                Serials = [];
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

    #endregion

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
        public decimal UnitsPerTag { get; init; }
        public decimal TotalValue => Math.Round(Count * PackValue, 2);
        public string DocumentId { get; init; } = string.Empty;
        public string Line { get; init; } = string.Empty;
        public string Shift { get; init; } = string.Empty;
        public string Warehouse { get; init; } = string.Empty;
        public string PropertiesJson { get; init; } = "{}";
        public List<string>? Serials { get; init; }
        public int SavedCount { get; set; }
        public bool IsSaved { get; set; }
    }
}
