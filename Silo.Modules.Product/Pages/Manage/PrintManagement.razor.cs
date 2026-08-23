using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;
using Silo.Components.DynamicField;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;
using Silo.Shared.Components.Print;

namespace Silo.Modules.Product.Pages;

public partial class PrintManagement
{
    private string UserCode = string.Empty;
    public string CompanyName = string.Empty;
    public string SearchSerial = string.Empty;
    public bool IsLoading = true;
    public bool IsAllSerialsChoosen = true;
    public List<GetAllLinesVm> Lines = new();
    public List<GetAllShiftsVm> Shifts = new();
    public List<GetAllProductQcsVm> Statuses = new();
    public List<GetAllWarehousesVm> Warehouses = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes = new();
    public List<GetAllProductTypeVm> Types = new();
    public SavePrintCommand Request = new();
    public PositionProductResponse? ChosenProduct;
    public string CurrentPrintActionId = string.Empty;
    public List<GetPrintsByPrintActionIdDto> StagedSerials = new();
    public List<DynamicFieldWithValueDto> DynamicFieldDtos = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public GetPrintsByPrintActionIdDto ShowingPrint = new();
    public List<DynamicFieldWithValueDto> ShowingPrintDynamicFields = new();

    public ProductCodeModal ProductModal { get; set; }
    public DynamicFieldFillValue DynamicFieldRef { get; set; }
    public DynamicFieldFillValue ShowingPrintDynamicFieldRef { get; set; }
    public SelectPrintFormat SelectPrintFormatRef { get; set; }
    public Modal SearchModal { get; set; }
    public Modal ModalPrintRow { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        UserCode = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Lines = await FormalCache.GetLines();

        Shifts = await FormalCache.GetShifts();

        Statuses = await FormalCache.GetQcs();

        Warehouses = await FormalCache.GetWarehouses();

        Sizes = await FormalCache.GetSizes();

        Types = await FormalCache.GetTypes();

        Warehouses = Warehouses.Where(p => p.OperationalType == 0).ToList();

        await LoadDynamicFields();

        await GetNewPrintActionId();

        ResetAll();

        ChooseFirstItems();

        IsLoading = false;
    }

    public async Task OnPreviousActionClick(MouseEventArgs e)
    {
        if (int.TryParse(CurrentPrintActionId, out int id) && id > 1)
        {
            CurrentPrintActionId = (id - 1).ToString();

            await LoadSavedSerials();
        }
    }

    public async Task OnNextActionClick(MouseEventArgs e)
    {
        if (int.TryParse(CurrentPrintActionId, out int id))
        {
            CurrentPrintActionId = (id + 1).ToString();

            await LoadSavedSerials();
        }
    }

    public void OnProductVmSelected(PositionProductResponse product)
    {
        ChosenProduct = product;

        FillProductFields(product);
    }

    public async Task OnSearchProductClick(MouseEventArgs e)
    {
        await ProductModal.Show();
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        string dynamicJson = DynamicFieldRef is not null
            ? await DynamicFieldRef.GetJsonData()
            : "{}";

        JObject? properties = null;

        try
        {
            properties = JObject.Parse(dynamicJson);
        }
        catch { }

        await SavePrint(Request.ProductCode, Request.Count.ToString(), Request.SelectedLine, Request.SelectedShift,
            Request.SelectedWarehouse, Request.SelectedStatus,
            Request.DocumentId.HasNoValue() ? "0" : Request.DocumentId,
            properties,Request.ProductPackValue);

        await LoadSavedSerials();

        IsLoading = false;
    }

    public async Task OnNewClick(MouseEventArgs e)
    {
        await GetNewPrintActionId();

        ResetAll();

        ChooseFirstItems();
    }

    public async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "F5")
        {
            await OnNewClick(new());
        }
    }

    public async Task OnPrintActionClick(GetPrintFormatsByPageTitleDto format)
    {
        await PrintSelectedFormat(format.Path);
    }

    public async Task PrintSelectedFormat(string reportFileName)
    {
        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration["Settings:Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("ProductSize",  Request.ProductSize)
            , new("CompanyName", CompanyName)
            , new("ProductTitle", Request.ProductTitle)
            , new("ProductUnit", Request.ProductUnit)
            , new("ProductValue", Request.ProductValue)
            , new("ProductCountInPack", Request.ProductCountInPack)
            , new("ProductPackValue", Request.ProductPackValue)
            , new("ProductPackWeight", Request.ProductPackWeight)
            , new("ProductStatusTitle", Request.ProductStatusTitle)
            , new("DocumentId", Request.DocumentId)
            , new("ProductCode", Request.ProductCode)
            , new("RegCode", Request.ProductRegCode)
            , new("Count", Request.Count)
            , new("Line", Lines.FirstOrDefault(p=> p.Code.Equals(Request.SelectedLine))?.Title)
            , new("Shift", Shifts.FirstOrDefault(p=> p.Code.Equals(Request.SelectedShift))?.Title)
            , new("Warehouse", Warehouses.FirstOrDefault(p=> p.DestinationCode.Equals(Request.SelectedWarehouse))?.DestinationTitle)
        };

        foreach (var field in DynamicFieldDtos)
        {
            variables.Add(new(field.Title.ToString().Trim().Replace(' ', '_'), field.Value.ToString()));
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new("Serials", StagedSerials.Where(p=> p.IsChoosed).Select(p=> new TelerikDropDownItem()
            {
                Value = p.ProductSerial
            }))
        };

        CreatePreparedReportCommand command = new()
        {
            Title = PageTitle,
            ReportFileName = reportFileName,
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
         , "PreparedReport/Create"
         , command);

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);
    }

    public async Task OnEditSerialClick(GetPrintsByPrintActionIdDto print)
    {
        ShowingPrint = print;

        var dynamicData = print.ProductProperties.HasNoValue() ? new JObject() : JObject.Parse(print.ProductProperties);

        ShowingPrintDynamicFields = DynamicFieldDtos.Select(f => new DynamicFieldWithValueDto
        {
            Title = f.Title,
            Value = dynamicData.ContainsKey(f.Title) ? dynamicData[f.Title].ToString() : string.Empty,
            ValueType = f.ValueType,
            DefaultValue = f.DefaultValue,
            IsRequired = f.IsRequired,
            ValueOptions = f.ValueOptions,
            IsReadOnly = f.IsReadOnly,
            Order = f.Order
        }).ToList();

        await ModalPrintRow.Open(new());
    }

    public async Task OnEditSinglePrintSubmit(MouseEventArgs e)
    {
        IsLoading = true;

        string dynamicJson = ShowingPrintDynamicFieldRef is not null
            ? await ShowingPrintDynamicFieldRef.GetJsonData()
            : ShowingPrint.ProductProperties ?? "{}";

        EditPrintCommand command = new()
        {
            ProductSerial = ShowingPrint.ProductSerial,
            ProductName = ShowingPrint.ProductName,
            ProductRegCode = ShowingPrint.ProductRegCode,
            ProductPackWeight = ShowingPrint.ProductPackWeight,
            DocumentId = ShowingPrint.DocumentId.ToString(),
            ProductProductionShift = ShowingPrint.ProductProductionShift,
            ProductProductionLine = ShowingPrint.ProductProductionLine,
            DestinationCode = ShowingPrint.DestinationCode,
            ProductStatusCode = ShowingPrint.ProductStatusCode,
            ProductProperties = dynamicJson,
            ProductCount = ShowingPrint.ProductCount
        };

        var result = await Api.SendAsyncObjectByUri<EditPrintVm>(HttpMethod.Put, "Print/EditPrint", command);

        if (result.Value.Result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            await ModalPrintRow.Close(e);

            await LoadSavedSerials();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnSearchSerialClick(MouseEventArgs e)
    {
        if (SearchSerial.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_ProductSerial), "error");

            return;
        }

        IsLoading = true;

        var dt = (await Api.PostAsync<List<GetPrintsByPrintActionIdDto>>("SGetPrintListBySerial",
            new KeyValuePair<string, object>("serial", SearchSerial))).Value;

        if (dt.Any())
        {
            StagedSerials = dt.ToList();

            var sampleProduct = dt.First();

            Request.ProductCode = sampleProduct.ProductCode;

            Request.ProductRegCode = sampleProduct.ProductRegCode;

            Request.ProductTitle = sampleProduct.ProductName;

            Request.ProductSize = sampleProduct.ProductSize;

            Request.ProductType = sampleProduct.ProductTypeCode;

            Request.ProductUnit = sampleProduct.ProductUnit;

            Request.ProductValue = sampleProduct.ProductCount.ToString();

            Request.ProductCountInPack = sampleProduct.ProductCountInPack.ToString();

            Request.ProductPackValue = sampleProduct.ProductPackValue.ToString();

            Request.ProductPackWeight = sampleProduct.ProductPackWeight.ToString();

            Request.ProductStatusTitle = sampleProduct.ProductStatusTitle;

            Request.ProductStatus = sampleProduct.ProductStatusCode;

            Request.DocumentId = sampleProduct.DocumentId.ToString();

            Request.SelectedWarehouse = sampleProduct.DestinationCode;

            Request.SelectedLine = sampleProduct.ProductProductionLine;

            Request.SelectedStatus = sampleProduct.ProductStatusCode;

            Request.SelectedShift = sampleProduct.ProductProductionShift;

            var dynamicData = sampleProduct.ProductProperties.HasNoValue() ? new() : JObject.Parse(sampleProduct.ProductProperties);

            CurrentPrintActionId = sampleProduct.PrintActionId.ToString();

            foreach (var field in DynamicFieldDtos)
            {
                if (dynamicData.ContainsKey(field.Title))
                {
                    field.Value = dynamicData[field.Title].ToString();
                }
            }

            await SearchModal.Close(e);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_NotFound, "error");
        }

        IsLoading = false;
    }

    public async Task OnSerialModalOpen(MouseEventArgs e)
    {
        SearchSerial = string.Empty;

        await SearchModal.Open(e);
    }

    #region Private methods
    private void ResetAll()
    {
        DynamicFieldDtos.ForEach(f => f.Value = string.Empty);

        ResetCombosAndSerials();
    }

    private async Task LoadSavedSerials()
    {
        IsLoading = true;

        var dt = (await Api.PostAsync<List<GetPrintsByPrintActionIdDto>>("SGetPrintListByPrintActionId",
            new KeyValuePair<string, object>("PrintActionId", CurrentPrintActionId),
            new KeyValuePair<string, object>("userToken", UserCode))).Value;

        if (dt.Any())
        {
            StagedSerials = dt.ToList();

            var sampleProduct = dt.First();

            Request.ProductCode = sampleProduct.ProductCode;

            Request.ProductRegCode = sampleProduct.ProductRegCode;

            Request.ProductTitle = sampleProduct.ProductName;

            Request.ProductSize = sampleProduct.ProductSize;

            Request.ProductType = sampleProduct.ProductTypeCode;

            Request.ProductUnit = sampleProduct.ProductUnit;

            Request.ProductValue = sampleProduct.ProductCount.ToString();

            Request.ProductCountInPack = sampleProduct.ProductCountInPack.ToString();

            Request.ProductPackValue = sampleProduct.ProductPackValue.ToString();

            Request.ProductPackWeight = sampleProduct.ProductPackWeight.ToString();

            Request.ProductStatusTitle = sampleProduct.ProductStatusTitle;

            Request.ProductStatus = sampleProduct.ProductStatusCode;

            Request.DocumentId = sampleProduct.DocumentId.ToString();

            Request.SelectedWarehouse = sampleProduct.DestinationCode;

            Request.SelectedLine = sampleProduct.ProductProductionLine;

            Request.SelectedStatus = sampleProduct.ProductStatusCode;

            Request.SelectedShift = sampleProduct.ProductProductionShift;

            var dynamicData = sampleProduct.ProductProperties.HasNoValue() ? new() : JObject.Parse(sampleProduct.ProductProperties);

            foreach (var field in DynamicFieldDtos)
            {
                if (dynamicData.ContainsKey(field.Title))
                {
                    field.Value = dynamicData[field.Title].ToString();
                }
            }
        }

        IsLoading = false;
    }

    private async Task SavePrint(string productCode, string count,
        string line, string shift, string warehouse, string status,
        string documentId, JObject? properties, string packValue)
    {
        string serial = StagedSerials.Count > 0
            ? string.Join(",", StagedSerials)
            : string.Empty;

        var result = (await Api.PostAsync<string>("SSavePrintBySerial",
            new KeyValuePair<string, object>("ProductSerial", serial),
            new KeyValuePair<string, object>("ProductCode", productCode),
            new KeyValuePair<string, object>("ProductCount", packValue),
            new KeyValuePair<string, object>("PrintActionId", CurrentPrintActionId),
            new KeyValuePair<string, object>("ProductProductionLine", line),
            new KeyValuePair<string, object>("ProductProductionShift", shift),
            new KeyValuePair<string, object>("PrintCount", count),
            new KeyValuePair<string, object>("ProductContractType", "0"),
            new KeyValuePair<string, object>("ProductOldSerial", serial),
            new KeyValuePair<string, object>("Location", string.Empty),
            new KeyValuePair<string, object>("DocumentId", documentId),
            new KeyValuePair<string, object>("PrintUser", UserCode),
            new KeyValuePair<string, object>("WareHouseCode", warehouse),
            new KeyValuePair<string, object>("ProductProperties", properties.ToString())
        )).Value;

        if (result.HasValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    private void FillProductFields(PositionProductResponse product)
    {
        ChosenProduct = product;

        Request.ProductCode = product.ProductCode;

        Request.ProductRegCode = product.TechnicalCode;

        Request.ProductTitle = product.ProductName;

        Request.ProductSize = product.ProductSize;

        Request.ProductType = product.ProductType;

        Request.ProductUnit = product.ProductUnit;

        Request.ProductValue = product.ProductValue;

        Request.ProductCountInPack = product.ProductCountInPack;

        Request.ProductPackValue = product.ProductPackValue;

        Request.ProductPackWeight = product.ProductPackWeight;

        Request.ProductStatusTitle = product.ProductStatusTitle;

        Request.ProductStatus = product.ProductStatus;

        Request.DocumentId = product.DocumentId;
    }

    private async Task LoadDynamicFields()
    {
        var fields = (await Api.PostAsync<List<GetAllDynamicFieldVm>>("SGetDynamicFieldsByActionTypeId",
            new KeyValuePair<string, object>("actionTypeId", 0))).Value;

        if (fields is null)
        {
            return;
        }

        DynamicFieldDtos = fields.OrderBy(f => f.Id).Select(f => new DynamicFieldWithValueDto
        {
            Title = f.Title,
            Value = string.Empty,
            ValueType = f.ValueType,
            DefaultValue = f.DefaultValue ?? string.Empty,
            IsRequired = f.IsRequired ?? false,
            ValueOptions = f.ValueOptions.HasValue()
                ? f.ValueOptions.Split('|').ToList()
                : new(),
            IsReadOnly = f.IsReadOnly ?? false,
        }).ToList();
    }

    private void ResetCombosAndSerials()
    {
        StagedSerials = new();

        Request = new();

        ChosenProduct = null;
    }

    private async Task GetNewPrintActionId()
    {
        var result = (await Api.PostAsync<string>("SGetNewPrintActionId",
            new KeyValuePair<string, object>("userToken", UserCode))).Value;

        if (result.HasValue())
        {
            CurrentPrintActionId = result;
        }
    }

    private void ChooseFirstItems()
    {
        if (Lines.Count == 1)
        {
            Request.SelectedLine = Lines[0].Code;
        }

        if (Shifts.Count == 1)
        {
            Request.SelectedShift = Shifts[0].Code;
        }

        if (Warehouses.Count == 1)
        {
            Request.SelectedWarehouse = Warehouses[0].DestinationCode;
        }
    }
    #endregion
}
