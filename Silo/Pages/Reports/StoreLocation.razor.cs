using System.Text.Json;
using Silo.Application;

namespace Silo.Pages.Reports;
public partial class StoreLocation
{
    public bool IsLoading = false;
    public GetAllZoneProductsQuery Request = new();
    public List<GetAllWarehousesVm> Warehouses;
    public List<SerialRowDto> ResultSerials = new();
    public List<GetAllDynamicFieldVm> VisibleFields = new();
    public List<TelerikDropDownItem> AgeRanges = new()
    {
        new() { Name = "تا یک ماه", Value = "1" },
        new() { Name = "یک تا سه ماه", Value = "2" },
        new() { Name = "سه تا شش ماه", Value = "3" },
        new() { Name = "شش ماه تا یک سال", Value = "4" },
        new() { Name = "بالای یک سال", Value = "5" }
    };
    public List<TelerikDropDownItem> LocationResolutions = new()
    {
        new() { Name = "یک", Value = "1" },
        new() { Name = "دو", Value = "2" },
        new() { Name = "سه", Value = "3" }
    };
    public List<TelerikDropDownItem> OccupiedCapacityPercents = new()
    {
        new() { Name = "خالی", Value = "0" },
        new() { Name = "کمتر از 20 درصد", Value = "20" },
        new() { Name = "20 الی 40 درصد", Value = "40" },
        new() { Name = "40 الی 60 درصد ", Value = "60" },
        new() { Name = "60 الی 80 درصد ", Value = "80" },
        new() { Name = "بیش از 80 درصد", Value = "100" }
    };
    #region Result1
    public List<GetAllZoneProductsVm> Result1 = new();
    public GetAllZoneProductsVm ChoosedResult1 = new();
    #endregion
    #region Result2
    public List<GetZoneProductByZoneAndWarehouseCodeVm> Result2;
    public GetZoneProductByZoneAndWarehouseCodeVm ChoosedResult2 = new();
    #endregion
    #region Result3
    public List<GetProductDetailsByZoneAndWarehouseAndProductCodeVm> Result3;
    public GetProductDetailsByZoneAndWarehouseAndProductCodeVm ChoosedResult3 = new();
    #endregion
    #region Result4
    public List<GetAllZoneProductDetailsDateVm> Result4 = new();
    #endregion

    [Inject] public IFormalDataCache FormalCache { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationModal { get; set; }
    public Modal ModalResult2 { get; set; }
    public Modal ModalResult3 { get; set; }
    public Modal ModalResult4 { get; set; }
    public Modal ModalSerials { get; set; }

    protected override async Task SiloInitializer()
    {
        Warehouses = await FormalCache.GetWarehouses();
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        GetAllZoneProductsQuery request = FixEmptiness();

        if (request is null)
        {
            return;
        }

        IsLoading = true;

        request.MinCapacity = Request.Capacity switch
        {
            null or "" => "-1",
            "0" => "0",
            "20" => "0",
            "40" => "20",
            "60" => "40",
            "80" => "60",
            "100" => "80"
        };

        request.MaxCapacity = Request.Capacity switch
        {
            null or "" => "-1",
            "0" => "0",
            "20" => "20",
            "40" => "40",
            "60" => "60",
            "80" => "80",
            "100" => "100"
        };

        Result1 = (await Api.PostAsync<List<GetAllZoneProductsVm>>("SSearchZonesProducts"
           , new KeyValuePair<string, object>[] { new("search", request) }
           )).Value;

        var allDynamicFields = await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetAllDynamicFields");

        var visibleFields = allDynamicFields.Value.Where(f => f.FieldShowColumn == true).ToList();

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        Request = new();

        Result4 = null;

        Result3 = null;
        ChoosedResult3 = new();

        Result2 = null;
        ChoosedResult2 = new();

        Result1 = new();
        ChoosedResult1 = new();
    }

    public async Task OnShowSerialsClick(GetAllZoneProductsVm item, MouseEventArgs e)
    {
        IsLoading = true;

        var allDynamicFields = await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetAllDynamicFields");

        var activeFields = allDynamicFields.Value.Where(f => f.FieldShowColumn).ToList();

        var serialsRequest = new GetZoneProductSerialsQuery
        {
            ZoneCode = item.ZoneCode,
            ProductCode = Request.ProductCode,
            ProductSerial = Request.ProductSerial,
            StoreCode = Request.WarehouseCode,
            RegCode = Request.TechnicalCode,
            RegCodeLike = Request.TechnicalCodeLike
        };

        var response = await Api.PostAsync<List<SerialRowDto>>("SGetZoneProductSerials",
        new KeyValuePair<string, object>("zoneProductSerialsQuery", serialsRequest));

        if (response.Value is not null)
        {
            ResultSerials = response.Value;

            var presentKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var serial in ResultSerials)
            {
                var normalized = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                
                foreach (var kvp in serial.DynamicData)
                {
                    var key = kvp.Key.Trim();
                    
                    if (normalized.TryAdd(key, kvp.Value) && kvp.Value.HasValue())
                    {
                        presentKeys.Add(key);
                    }
                }

                serial.DynamicData = normalized;
            }

            VisibleFields = activeFields.Where(f => presentKeys.Contains(f.Title?.Trim())).ToList();
        }

        await ModalSerials.Open(e);

        IsLoading = false;
    }

    public async Task OnClickProductCode(string productCode)
    {
        Request.ProductCode = productCode;
    }

    public async Task OnClickLocation(string location)
    {
        Request.TagZone = location;
    }

    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        if (ResultSerials is null || ResultSerials.Neither()) 
        {
            return;
        }

        Dictionary<string, string> fixedColumnsMapping = new()
        {
            { nameof(SerialRowDto.ProductSerial), "سریال" },
            { nameof(SerialRowDto.ProductName), "نام کالا" },
            { nameof(SerialRowDto.ProductCode), "کد کالا" },
            { nameof(SerialRowDto.RegCode), "کد فنی" },
            { nameof(SerialRowDto.ProductCount), "مقدار" }
        };

        var columns = fixedColumnsMapping.Values.ToList();
        columns.AddRange(VisibleFields.Select(f => f.Title));

        var exportData = new List<object>();

        foreach (var serial in ResultSerials)
        {
            var rowNode = new System.Text.Json.Nodes.JsonObject();

            rowNode[fixedColumnsMapping[nameof(serial.ProductSerial)]] = serial.ProductSerial;
            rowNode[fixedColumnsMapping[nameof(serial.ProductName)]] = serial.ProductName;
            rowNode[fixedColumnsMapping[nameof(serial.ProductCode)]] = serial.ProductCode;
            rowNode[fixedColumnsMapping[nameof(serial.RegCode)]] = serial.RegCode;
            rowNode[fixedColumnsMapping[nameof(serial.ProductCount)]] = serial.ProductCount;

            foreach (var field in VisibleFields)
            {
                rowNode[field.Title] = serial.DynamicData.ContainsKey(field.Title)
                    ? serial.DynamicData[field.Title]
                    : "";
            }

            var jsonElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(rowNode.ToJsonString());
            exportData.Add(jsonElement);
        }

        await ExcelExporter.ExportJsonData(TextResources.APP_StringKeys_Location_Serials_Report, exportData, columns);
    }

    private GetAllZoneProductsQuery FixEmptiness()
    {
        GetAllZoneProductsQuery request = new();

        if (Request.TagZone.HasNoValue()
            && Request.ZoneLayer.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_ChooseOneFieldRequired
                , TextResources.APP_StringKeys_Location
                , TextResources.APP_StringKeys_Resolution)
                , "error");

            return null;
        }

        if (string.IsNullOrEmpty(Request.ProductSerial))
            request.ProductSerial = "-1";
        else
            request.ProductSerial = Request.ProductSerial;

        if (string.IsNullOrEmpty(Request.TagZone))
            request.TagZone = "-1";
        else
            request.TagZone = Request.TagZone;

        request.TagZoneLike = Request.TagZoneLike;

        request.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (string.IsNullOrEmpty(Request.ZoneLayer))
            request.ZoneLayer = "-1";
        else
            request.ZoneLayer = Request.ZoneLayer;

        if (string.IsNullOrEmpty(Request.TechnicalCode))
            request.TechnicalCode = "-1";
        else
            request.TechnicalCode = Request.TechnicalCode;

        if (string.IsNullOrEmpty(Request.ProductCode))
            request.ProductCode = "-1";
        else
            request.ProductCode = Request.ProductCode;

        if (string.IsNullOrEmpty(Request.AgeRange))
            request.AgeRange = "-1";
        else
            request.AgeRange = Request.AgeRange;

        if (string.IsNullOrEmpty(Request.WarehouseCode))
            request.WarehouseCode = "-1";
        else
            request.WarehouseCode = Request.WarehouseCode;

        return request;
    }
}
