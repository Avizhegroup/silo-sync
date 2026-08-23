using System.Reflection;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto.DynamicField;
using Silo.Components.DynamicField;
using Silo.Shared.Components.Print;

namespace Silo.Pages.Location;
public partial class Place
{
    public bool IsLoading = true;
    public bool IsTagCountEditable = false;
    public bool ShowSerialDelete = true;
    public int ActiveTabIndex = 0;
    public string UserId;
    public string UserName;
    public string CompanyName;
    public string Serial = string.Empty;
    public string ErrorButtonClass = "btn-light";
    public string DeleteSerial = string.Empty;
    public string CurrentSourceWarehouse = string.Empty;
    public string CurrentDestinationWarehouse = string.Empty;
    public List<string> ChosenActionInfos = new();
    public List<string> ChosenStationNames = new();
    public List<GetAllWarehousesVm> SourceWarehouses;
    public List<GetAllWarehousesVm> DestinationWarehouses;
    public PlaceSerialErrorsDto Errors = new();
    public List<TruckCrossDataDto> NotExitedCrosses = new();
    public MovementActionDirectDto DirectPlaceDto = new() { GateCodes = new() };
    public SaveMovementActionDirectCommand SaveDirectPlace = new();
    public List<PlaceProductBySerialDto> FinalProductInfos = new();
    public List<PlaceProductAggDto> PlaceProductsAgg = new();
    public List<GetAllPlaceHeadersVm> DirectPlaceHeaders = new();
    public GetAllPlaceProductQuery MultipleProductSearchRequest = new();
    public List<GetMovementAllPlaceProductVm> MultipleProductSearchResponse = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<DynamicFieldWithValueDto> DynamicFieldsDto = new();
    public List<GetDocProductDataByDocKeyVm> Documents = new();
    public GetAllUhfReaderLogByActionIdQuery SearchGateAction = new();
    public List<Get100LastActionsByIdVm> GateActionsData;
    public List<GetAllStationsVm> Stations;
    public List<string> TempNewSerials = new();
    public List<int> LogGateActionIds = new();
    public GetAllActionTypesDto ActionType = new();
    public bool ValidationDocCodeSet = false;
    public bool ValidationDocItemsCheck1 = false;
    public bool TruckCrossShow = false;
    public bool TruckCrossRequirement = false;
    public bool IsAllSearchProductsSelected = false;

    public LocationModal LocationModal { get; set; }
    public Modal ErrorModal { get; set; }
    public Modal ModalDelete { get; set; }
    public ProductSerialModal ProductModal { get; set; }
    public Modal PlaceOperationModal { get; set; }
    public Modal GateActionsModal { get; set; }
    public Modal HandHeldActionsModal { get; set; }
    public Modal DuplicateSerialModal { get; set; }
    public BarcodeModal BarcodeModal { get; set; }
    public TelerikGrid<GetDocProductDataByDocKeyVm> DocumentsGrid { get; set; }
    public TelerikGrid<PlaceProductBySerialDto> FinalProductInfosGrid { get; set; }
    public DynamicFieldFillValue DynamicFieldFillValueRef { get; set; }
    public SelectPrintFormat SelectPrintFormatRef { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    [Parameter] public string? ActiveStations { get; set; } = "";
    [Parameter] public int? TruckCrossId { get; set; } = -1;
    [Parameter] public int? ActionTypeCode { get; set; } = -1;
    [Parameter] public int? Mode { get; set; } = 1;

    protected override async Task SiloInitializer()
    {
        IsTagCountEditable = (await Api.PostAsyncByUri<bool>("wms/Product"
            , "SIsTagProductCountEditable")).Value;

        NavigationManager.RegisterLocationChangingHandler(OnLocationChanged);

        await InitMetaData();

        IsLoading = false;
    }

    protected override Task OnParametersSetAsync()
    {
        ActiveStations = ActiveStations ?? "";
        TruckCrossId = TruckCrossId ?? -1;
        ActionTypeCode = ActionTypeCode ?? -1;
        Mode = Mode ?? -1;

        return base.OnParametersSetAsync();
    }

    #region Choose Destination And Source
    public async Task OnModalLocationClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.DestinationWarehouseCode.HasValue())
        {
            await LocationModal.Show();

            return;
        }

        Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required1,
                                        TextResources.APP_StringKeys_Destination_Warehouse),
                                        "error");
    }

    public async Task OnChooseDestinationZoneClick(GetAllZonesVm zone)
    {
        Enum.TryParse<DestinationInventoryType>(zone.StoreType, out DestinationInventoryType destinationCapacityType);

        DirectPlaceDto.DestinationZoneCode = zone.ZoneCode;

        await SetDynamicFieldsBySourceAndDestination();
    }

    public async Task OnSourceWarehouseChange(object e)
    {
        var newSource = string.Empty;

        if (e is not null)
        {
            newSource = e as string;
        }

        if (CurrentSourceWarehouse.NotEquals(newSource))
        {
            CurrentSourceWarehouse = newSource;

            await SetDynamicFieldsBySourceAndDestination();
        }
    }

    public async Task OnDestinationWarehouseChange(object e)
    {
        var newSource = string.Empty;

        if (e is not null)
        {
            newSource = e as string;
        }

        if (CurrentDestinationWarehouse.NotEquals(newSource))
        {
            CurrentDestinationWarehouse = newSource;

            DirectPlaceDto.DestinationZoneCode = null;

            await SetDynamicFieldsBySourceAndDestination();
        }
    }
    #endregion

    #region Add Single Serial
    public async Task OnSingleSerialAddClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse)
                , "warning");

            return;
        }

        if (DirectPlaceDto.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse)
                , "warning");

            return;
        }

        if (Serial.HasValue())
        {
            await AddNewSerials(new() { Serial }, -1);

            Serial = string.Empty;
        }
    }

    public async Task OnSerialKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnSingleSerialAddClick(new());
        }
    }
    #endregion

    #region Modal
    public async Task OnProductModalClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse)
                , "warning");

            return;
        }

        if (DirectPlaceDto.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse)
                , "warning");

            return;
        }

        await OnClearProductModalClick(e);

        MultipleProductSearchRequest.SourceWarehouseCode = DirectPlaceDto.SourceWarehouseCode;

        IsAllSearchProductsSelected = false;

        await ProductModal.Open();
    }

    public async Task OnSearchProductModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllPlaceProductQuery request = FixSearchMultipleProductEmptiness();

        var result = (await Api.PostAsync<List<GetMovementAllPlaceProductVm>>("SPSearchProductPlace",
            new KeyValuePair<string, object>[] { new("search", request) })).Value;

        MultipleProductSearchResponse = result;

        IsLoading = false;
    }

    public async Task OnApproveSelectedProductsClick(List<GetAllProductBySerialVm> tags)
    {
        IsLoading = true;

        List<string> serials = tags
            .Select(p => p.ProductSerial)
            .ToList();

        if (serials.Any())
        {
            await AddNewSerials(serials, -1);
        }

        IsLoading = false;
    }

    public async Task OnClearProductModalClick(MouseEventArgs e)
    {
        MultipleProductSearchRequest = new();

        MultipleProductSearchResponse = new();
    }
    #endregion

    #region Excel
    public async Task OnUploadExcelComplete(string path)
    {
        IsLoading = true;

        var result = await Api.PostFileAsync<List<string>>("InputExcelFilePlace", path
            , new("sourceWarehouseCode", DirectPlaceDto.SourceWarehouseCode)
            , new("logGateActionId", "-1")
            , new("userToken", UserId));

        if (result.Successful && result.Value.Any())
        {
            await AddNewSerials(result.Value, -1);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            Errors.ErrorTitle = "";

            Errors.Serials = result.Value.ToList();

            await ErrorModal.Open(new());
        }

        IsLoading = false;
    }
    #endregion

    #region Gate And HandHeld
    public async Task OnGateModalClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse)
                , "warning");

            return;
        }

        if (DirectPlaceDto.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse)
                , "warning");

            return;
        }

        await OnGateActionsClear(e);

        await GateActionsModal.Open(e);
    }

    public async Task OnGateValidSubmit()
    {
        if (ActiveStations is not null && SearchGateAction.UhfGateCode is null)
        {
            await GateActionsModal.Close(new());

            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Station)
                , "warning");

            return;
        }

        IsLoading = true;

        SearchGateAction.UhfGateMovementActionDestination = DirectPlaceDto.DestinationWarehouseCode;

        SearchGateAction.UhfGateMovementActionFrom = DirectPlaceDto.SourceWarehouseCode;

        GateActionsData = (await Api.PostAsyncByContext<List<Get100LastActionsByIdVm>>("SGetLastUhfGateActionsById"
            , new Get100LastActionsByIdVmContext()
            , new KeyValuePair<string, object>("action", SearchGateAction)
            )).Value;

        IsLoading = false;
    }

    public async Task OnGateActionChoose(Get100LastActionsByIdVm action)
    {
        IsLoading = true;

        await GateActionsModal.Close(new());

        var serials = (await Api.PostAsync<List<string>>("SGetAllUhfLogReadedSerials"
           , new KeyValuePair<string, object>("uhfLogId", action.Code.ToString()))).Value;

        if (serials.Any())
        {
            DirectPlaceDto.GateCodes.Add(action.GateCode);

            await AddNewSerials(serials, action.Code);

            ChosenActionInfos.Add($@"{TextResources.APP_StringKeys_Station}:{action.StationName} 
                                    | {TextResources.APP_StringKeys_OperationCode}:{action.Code} 
                                    | {TextResources.APP_StringKeys_DateTime}:{action.DateTime.ToNormalPersianDateTime()} 
                                    | {TextResources.APP_StringKeys_Field_Status}:{action.Status}");

            ChosenStationNames.Add(action.StationName);
        }

        IsLoading = false;
    }

    public async Task OnGateActionsClear(MouseEventArgs e)
    {
        SearchGateAction = new();

        if (Stations.Count == 1)
        {
            SearchGateAction.UhfGateCode = Stations.First().Code;
        }

        GateActionsData = new();
    }
    #endregion

    #region Barcode Scanner
    public async Task OnOpenBarcodeModalClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse)
                , "warning");

            return;
        }

        await BarcodeModal.Show();
    }

    public async Task OnAddBarcodeClick(string barcode)
    {
        if (barcode.HasValue())
        {
            await AddNewSerials(new() { barcode }, -1);
        }
    }
    #endregion

    #region History
    public async Task OnPlaceOperationModalClick(MouseEventArgs e)
    {
        DirectPlaceHeaders = new();

        await PlaceOperationModal.Open(e);
    }

    public async Task OnSearchPlaceOperationModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        DirectPlaceHeaders = (await Api.PostAsync<List<GetAllPlaceHeadersVm>>("SGetAllPlaces")).Value;

        IsLoading = false;
    }

    public async Task OnChoosePlaceHeader(GetAllPlaceHeadersVm Place)
    {
        IsLoading = true;

        var newProducts = (await Api.PostAsyncByContext<List<GetPlaceProductBySerialWithAggResultVm>>(
            "SGetPlaceItems"
           , new GetPlaceProductBySerialWithAggResultVmContext()
           , new KeyValuePair<string, object>("placeId", Place.PlaceId))).Value;

        DirectPlaceDto = Mapper.Map<MovementActionDirectDto>(Place);

        if (newProducts is not null)
        {
            PlaceProductsAgg = new();
        }

        IsLoading = false;

        await PlaceOperationModal.Close(new());
    }
    #endregion

    #region Document Code
    public async Task OnSearchDocumentClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse)
                , "warning");

            return;
        }

        if (DirectPlaceDto.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse)
                , "warning");

            return;
        }

        if (DirectPlaceDto.DocumentCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Doc_Code_Required, "error");

            return;
        }

        ActiveTabIndex = 1;

        IsLoading = true;

        Documents = (await Api.PostAsyncByContext<List<GetDocProductDataByDocKeyVm>>("SGetDocumentItemData"
             , new GetDocProductDataByDocKeyVmContext()
             , new("documentKey", DirectPlaceDto.DocumentCode)
             , new("sourceWarehouse", DirectPlaceDto.SourceWarehouseCode)
             , new("destinationWarehouse", DirectPlaceDto.DestinationWarehouseCode))).Value;

        if (Documents.Any())
        {
            var headerData = Documents.FirstOrDefault(p => p.DocumentHeaderData.HasValue()).DocumentHeaderData;

            await DynamicFieldFillValueRef.FillJsonData(JToken.Parse(headerData));

            if (ValidationDocItemsCheck1)
            {
                CheckDocument();
            }

            SetErrorButtonClass();
        }

        IsLoading = false;

        void CheckDocument()
        {
            DocumentCheckType? docCheckType = DocumentCheckType.None;

            docCheckType = Documents.FirstOrDefault(p => p.DocumentCheckType is not null)?.DocumentCheckType;

            foreach (var doc in Documents)
            {
                if (PlaceProductsAgg.Any(p => p.ProductCode.Equals(doc.ProductCode)))
                {
                    doc.Status = "";
                }
                else if (PlaceProductsAgg.Any())
                {
                    if (docCheckType == DocumentCheckType.Exact)
                    {
                        doc.Status = "مغایرت عدم شناسایی کالا";
                    }
                    else
                    {
                        doc.Status = "";
                    }
                }
            }

            if (docCheckType == DocumentCheckType.DocCodeRemain)
            {
                decimal documentAllUsed = Documents.First().DocumentUsedCount ?? 0;

                if (PlaceProductsAgg.Sum(p => p.ProductCount) > Documents.Sum(p => p.SumValue) - documentAllUsed)
                {
                    Documents.ForEach(p => p.Status = "مغایرت مقدار کل سند در عملیات");
                }
            }
        }
    }

    public async Task OnDocumentKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await OnSearchDocumentClick(new());
        }
    }

    public async Task OnClearDocumentClick(MouseEventArgs e)
    {
        DirectPlaceDto.DocumentCode = string.Empty;

        Documents = new();
    }
    #endregion

    #region Events
    public async Task OnToggleSearchProductSelectAll()
    {
        MultipleProductSearchResponse.ForEach(p =>
        {
            p.IsChoosed = IsAllSearchProductsSelected;
        });
    }

    public async ValueTask OnLocationChanged(LocationChangingContext context)
    {
        await InitMetaData(context.TargetLocation);
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (!IsSaveRequestValid())
        {
            return;
        }

        IsLoading = true;

        DirectPlaceDto.MovementActionData = await DynamicFieldFillValueRef.GetJsonData();

        SaveDirectPlace = new()
        {
            SourceWarehouseCode = DirectPlaceDto.SourceWarehouseCode,
            DestinationWarehouseCode = DirectPlaceDto.DestinationWarehouseCode,
            DestinationZoneCode = DirectPlaceDto.DestinationZoneCode,
            DirectPlaceActionData = DirectPlaceDto.MovementActionData,
            DirectPlaceActionDesc = DirectPlaceDto.MovementActionDesc,
            DocumentId = DirectPlaceDto.DocumentCode,
            LogGateActionIds = LogGateActionIds,
            Serials = FinalProductInfos.Select(p => p.ProductSerial).ToList(),
            GateCode = DirectPlaceDto.GateCodes.FirstOrDefault() ?? "-1",
            TruckCrossId = DirectPlaceDto.TruckCrossId
        };

        int result = (await Api.PostAsync<int>("SSaveDirectPlace"
                        , new KeyValuePair<string, object>("place", SaveDirectPlace))).Value;

        IsLoading = false;

        if (result == -1)
        {
            IsLoading = false;

            Notification.Show(TextResources.APP_StringKeys_Alert_Fail
                , "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            SaveDirectPlace = new();

            DirectPlaceDto.MovementActionId = result;

            DirectPlaceDto.MovementActionDateTime = $"{DateTime.Now.ToString("HH:mm")} {PersianCalendarTools.GregorianToPersian(DateTime.Now)}";

            DirectPlaceDto.UserName = UserName;
        }
    }

    public async Task OnDeleteSerialProductClick(PlaceProductBySerialDto product)
    {
        DeleteSerial = product.ProductSerial;

        await ModalDelete.Open(new());
    }

    public async Task OnEditCountSerialClick(PlaceProductBySerialDto product)
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SEditTag"
            , new KeyValuePair<string, object>("productSerial", product.ProductSerial)
            , new KeyValuePair<string, object>("productCount", product.SumCount)
        , new KeyValuePair<string, object>("userToken", UserId))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            CreateAggProductFromSerials();

            FinalProductInfosGrid.Rebind();

            product.IsEditMode = false;
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public void OnRemoveClick()
    {
        FinalProductInfos = FinalProductInfos.Where(p => p.ProductSerial.NotEquals(DeleteSerial)).ToList();

        DeleteSerial = string.Empty;

        CreateAggProductFromSerials();
    }

    public async Task OnClearAllClick(MouseEventArgs e)
    {
        IsLoading = true;

        DirectPlaceDto = new() { GateCodes = new() };

        Serial = string.Empty;

        FinalProductInfos = new();

        PlaceProductsAgg = new();

        DirectPlaceDto.DocumentCode = string.Empty;

        DynamicFields = new();

        Documents = new();

        ErrorButtonClass = "btn-light";

        DeleteSerial = string.Empty;

        await SetDynamicFieldsBySourceAndDestination();

        ChosenActionInfos = new();

        IsLoading = false;

        StateHasChanged();
    }

    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        if (DirectPlaceDto.MovementActionId == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_Notfound_Operation_Code, "error");

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsync<List<GetPlaceByActionIdVm>>("GetPlaceByActionId"
            , new KeyValuePair<string, object>("actionId", DirectPlaceDto.MovementActionId.ToString()))).Value;

        var stream = ExcelExporter.ExportDatatable(GetDataTableFromList(result));

        stream.Seek(0, SeekOrigin.Begin);

        await Exporter.ExportAndDownload(stream, $"{TextResources.APP_StringKeys_MovementAction}.xlsx");

        IsLoading = false;
    }

    public async Task OnSelectProductForPrint(MouseEventArgs e)
    {
        if (SelectPrintFormatRef != null)
        {
            await SelectPrintFormatRef.ShowPrintFormatsAsync(e);
        }
    }

    public async Task OnPrintClick(GetPrintFormatsByPageTitleDto format)
    {
        if (DirectPlaceDto.MovementActionId.Equals(0))
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_OperationCode)
                , "error");

            return;
        }

        IsLoading = true;

        MovementActionPrintDto printAction = new();

        List<TagMovementPrintDto> serials = (await Api.PostAsync<List<TagMovementPrintDto>>("SGetSerialDataForPlace"
            , new KeyValuePair<string, object>("serials"
            , FinalProductInfos.Select(p => p.ProductSerial).ToList()))).Value;

        printAction.TruckCross = new();

        List<ExitActionPrintMainDto> printData = new();

        foreach (var serial in serials)
        {
            ExitActionPrintMainDto print = printData.FirstOrDefault(p => p.ProductCode.Equals(serial.ProductCode));

            if (print is null)
            {
                print = new()
                {
                    ProductCode = serial.ProductCode,
                    ProductName = serial.ProductName,
                    TechnicalCode = serial.TechnicalCode,
                    Count = 0,
                    SumCount = 0,
                    Serials = string.Empty
                };
            }

            print.SumCount += serial.ProductCount;

            print.Count++;

            printData.ReplaceOrAdd(p => p.ProductCode.Equals(serial.ProductCode), print);
        }

        printAction.ExitPrints = printData;

        printAction.DateTime = $"{PersianCalendarTools.GregorianToPersian(DateTime.Now)} {DateTime.Now.ToString("HH:mm")}";

        printAction.User = (await AuthState.GetAuthenticationStateAsync()).User.GetUsername();

        printAction.OpCode = DirectPlaceDto.MovementActionId.ToString();

        printAction.ActionDocumentId = DirectPlaceDto.DocumentCode;

        printAction.MovementActionData = DirectPlaceDto.MovementActionData;

        printAction.StationName = ChosenStationNames.FirstOrDefault() ?? string.Empty;

        printAction.StationNames = string.Join(',', ChosenStationNames);

        await PrintAction(printAction);

        IsLoading = false;

        async Task PrintAction(MovementActionPrintDto printAction)
        {
            if (CompanyName.HasNoValue())
            {
                CompanyName = Configuration["Settings:Company"];
            }

            var variables = new List<KeyValuePair<string, object>>()
            {
                  new("DateString", $"{PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
                , new("ActionTypeTitle", printAction.ActionTypeTitle)
                , new("OperationCode", printAction.OpCode)
                , new("DateTimeString", printAction.DateTime)
                , new("User", printAction.User)
                , new("Count", printAction.Count)
                , new("GateCode", printAction.GateCode)
                , new("GateOpCode", printAction.GateOp)
                , new("StationName", printAction.StationName)
                , new("StationNames", printAction.StationNames)
                , new("Document", printAction.ActionDocumentId)
                , new("Description", printAction.MovementActionDesc)
                , new("CompanyName", CompanyName)
            };

            if (printAction.TruckCross is not null)
            {
                variables.Add(new("TruckDriverName", printAction.TruckCross.DriverName));
                variables.Add(new("TruckDriverPhone", printAction.TruckCross.DriverPhone));
                variables.Add(new("TruckDriverNationalCode", printAction.TruckCross.NationalCode));
                variables.Add(new("TruckPlaque", printAction.TruckCross.Plaque));
                variables.Add(new("TruckDriverLicenseCode", printAction.TruckCross.LicenseCode));
                variables.Add(new("TruckType", printAction.TruckCross.TypeTitle));
                variables.Add(new("TruckEnterWeight", printAction.TruckCross.EnterWeightTonage));
                variables.Add(new("TruckExitWeight", printAction.TruckCross.ExitWeightTonage));
            }

            if (printAction.MovementActionData.HasValue())
            {
                var headerDatas = JToken.Parse(printAction.MovementActionData);

                if (headerDatas is not null)
                {
                    foreach (JProperty item in headerDatas)
                    {
                        variables.Add(new(item.Name.ToString().Trim().Replace(' ', '_'), item.Value.ToString()));
                    }
                }
            }

            string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

            List<KeyValuePair<string, string>> images = new()
            {
                new("Image_Logo", path)
            };

            List<KeyValuePair<string, object>> dataSources = new()
            {
                new(nameof(ExitActionPrintMainDto), printAction.ExitPrints)
            };

            var command = new CreatePreparedReportCommand
            {
                Title = PageTitle,
                ReportFileName = format.Path,
                Variables = variables,
                DataSources = dataSources,
                Images = images
            };

            var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
         , "PreparedReport/Create"
         , command);

            if (response.Value.Result < 1)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

                return;
            }

            await Exporter.ExportAndDownloadUsingBypass(response.Value.Result);
        }
    }
    #endregion

    #region Row Render Events
    /// <summary>
    /// OperationProducts can have a ProductCode that is not in in Docs. "مغایرت در کد کالا"
    /// || Same ProductCode in OperationProducts and Docs can have diffrent value. "مقدار مغایرت ..." 
    /// </summary>
    /// <param name="args"></param>
    public void OnRowOpRenderHandler(GridRowRenderEventArgs args)
    {
        if (ValidationDocItemsCheck1)
        {
            PlaceProductAggDto item = (PlaceProductAggDto)args.Item;

            DocumentCheckType? docCheckType = DocumentCheckType.None;

            if (Documents is not null)
            {
                docCheckType = Documents.FirstOrDefault(p => p.DocumentCheckType is not null)?.DocumentCheckType;
            }

            if (docCheckType.Equals(DocumentCheckType.DocCodeRemain))
            {
                item.Status = "";

                return;
            }

            if (Documents is not null && Documents.Any())
            {
                foreach (GetDocProductDataByDocKeyVm doc in Documents)
                {
                    if (doc.ProductCode.Equals(item.ProductCode))
                    {
                        if (doc.DocumentCheckType == DocumentCheckType.Exact && (item.ProductCount != doc.SumValue))
                        {
                            args.Class += " bg-warning";

                            item.Status = "مقدار مغایرت " + (item.ProductCount - doc.SumValue);
                        }
                        else
                        {
                            item.Status = "";
                        }

                        return;
                    }
                }
            }
            else
            {
                item.Status = "";

                return;
            }

            args.Class += " bg-warning";

            item.Status = "مغایرت در کدکالا";
        }
    }

    /// <summary>
    /// Docs can have a ProductCode that is not in GateProducts. "عدم شناسایی کد کالا"
    /// </summary>
    /// <param name="args"></param>
    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        if (ValidationDocItemsCheck1)
        {
            GetDocProductDataByDocKeyVm item = (GetDocProductDataByDocKeyVm)args.Item;

            DocumentCheckType? docCheckType = DocumentCheckType.None;

            if (Documents is not null)
            {
                docCheckType = Documents.FirstOrDefault(p => p.DocumentCheckType is not null)?.DocumentCheckType;
            }

            if (PlaceProductsAgg is not null && PlaceProductsAgg.Any())
            {
                if (docCheckType == DocumentCheckType.ProductCodeAndDocCodeRemain)
                {
                    foreach (PlaceProductAggDto op in PlaceProductsAgg)
                    {
                        if (op.ProductCode.Equals(item.ProductCode))
                        {
                            if (op.ProductCount > item.DocumentUnusedCount)
                            {
                                args.Class += " bg-warning";

                                item.Status = "مغایرت مقداری سند در عملیات";

                                return;
                            }
                        }
                    }
                }
            }

            if (docCheckType == DocumentCheckType.Exact && item.Status.HasValue())
            {
                args.Class += " bg-warning";

                item.Status = "مغایرت عدم شناسایی کالا";
            }
            else if (docCheckType == DocumentCheckType.DocCodeRemain && item.Status.HasValue())
            {
                args.Class += " bg-warning";

                item.Status = "مغایرت مقدار کل سند در عملیات";
            }
            else
            {
                args.Class = "";

                item.Status = "";
            }
        }
    }

    /// <summary>
    /// Docs can have a ProductCode that is not in GateProducts. "عدم شناسایی کد کالا"
    /// </summary>
    /// <param name="args"></param>
    public void OnRowSerialsRenderHandler(GridRowRenderEventArgs args)
    {
        PlaceProductBySerialDto item = (PlaceProductBySerialDto)args.Item;

        if (item.Status.HasValue())
        {
            args.Class += " bg-danger";
        }
        else
        {
            args.Class = "";
        }
    }
    #endregion

    #region Private Methods 
    private async Task AddNewSerials(List<string> serials, int gateOpCode)
    {
        serials = serials.Distinct().Where(p => p.HasValue()).ToList();

        if (LogGateActionIds.Neither(p => p.Equals(gateOpCode)))
        {
            LogGateActionIds.Add(gateOpCode);
        }

        foreach (string serial in serials)
        {
            if (FinalProductInfos.Neither(p => p.ProductSerial.Equals(serial)))
            {
                TempNewSerials.Add(serial);
            }
        }

        IsLoading = true;

        var newProductInfos = (await Api.PostAsync<List<GateResult>>("SGetProductsInfoByEpcsForAction"
            , new("epcList", null)
            , new("SerialList", TempNewSerials)
            , new("userToken", UserId)
            , new("ActionId", "-1")
            , new("ActionType", ActionType is null ? 0: ActionType.Code)
            , new("DeviceId", "-1")
            , new("IsSaveUHF", false)
            , new("GetProductByParentEPC", false)
            , new("saveDateTime", null)
            )).Value;

        if (newProductInfos is null)
        {
            IsLoading = false;
            return;
        }

        AddNewProductInfos(newProductInfos);

        IsLoading = false;

        void AddNewProductInfos(List<GateResult> newProducts)
        {
            Documents = new();

            TempNewSerials = new();

            foreach (var productInfo in newProducts)
            {
                if (productInfo.CheckResultType.Equals("0"))
                {
                    break;
                }

                FinalProductInfos.Add(new()
                {
                    ProductSerial = productInfo.ProductSerial,
                    ProductCode = productInfo.ProductCode,
                    RegCode = productInfo.ProductTechnicalCode,
                    ProductName = productInfo.ProductName,
                    SumCount = decimal.Parse(productInfo.SumValue),
                    Status = productInfo.ExceptionMessage,
                    IsEditMode = false
                });
            }

            CreateAggProductFromSerials();

            FinalProductInfosGrid.Rebind();

            SetErrorButtonClass();
        }
    }

    private void CreateAggProductFromSerials()
    {
        PlaceProductsAgg = new();

        foreach (var groupedSerials in FinalProductInfos.GroupBy(p => p.ProductCode))
        {
            PlaceProductsAgg.Add(new()
            {
                ProductCode = groupedSerials.Key,
                ProductName = groupedSerials.First().ProductName,
                ProductCount = groupedSerials.Sum(p => p.SumCount),
                SumCount = groupedSerials.Count(),
                Status = string.Empty
            });
        }
    }

    private bool IsSaveRequestValid()
    {
        if (DirectPlaceDto.MovementActionId != 0)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Alert_MovementAction_Submit, DirectPlaceDto.MovementActionId.ToString()), "error");

            return false;
        }

        if (DirectPlaceDto.SourceWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Source_Warehouse), "error");

            return false;
        }

        if (DirectPlaceDto.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse), "error");

            return false;
        }

        if (FinalProductInfos.Count == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_SerialValidation
                , "warning");
            return false;
        }

        if (FinalProductInfos.Any(p => p.Status.HasValue()))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Prevent_On_Errors, "error");

            return false;
        }

        if (ValidationDocCodeSet && DirectPlaceDto.DocumentCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Enter_DocumentCode, "error");

            return false;
        }

        if (TruckCrossRequirement && DirectPlaceDto.TruckCrossId == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_TruckCross_Property_Required, "error");

            return false;
        }

        if (ValidationDocItemsCheck1)
        {
            if (!Documents.Any())
            {
                Notification.Show(TextResources.APP_StringKeys_Search_Document_First, "error");

                return false;
            }

            if (PlaceProductsAgg.Any(p => p.Status.HasValue()) || Documents.Any(p => p.Status.HasValue()))
            {
                Notification.Show(TextResources.APP_StringKeys_Contradiction, "error");

                return false;
            }
        }

        bool isDynamicFieldsRequirementValid = true;

        foreach (DynamicFieldWithValueDto dynamicFields in DynamicFieldsDto)
        {
            if (dynamicFields.IsRequired)
            {
                if (dynamicFields.Value.HasNoValue())
                {
                    Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required
                        , $"{(TextResources.APP_StringKeys_Dynamic_Field).Replace("فیلد", "")}: {dynamicFields.Title}")
                        , "error");

                    isDynamicFieldsRequirementValid = false;
                }
            }
        }

        if (!isDynamicFieldsRequirementValid)
        {
            return false;
        }

        return true;
    }

    private GetAllPlaceProductQuery FixSearchMultipleProductEmptiness()
    {
        GetAllPlaceProductQuery request = new();

        if (string.IsNullOrEmpty(MultipleProductSearchRequest.ProductCode))
        {
            request.ProductCode = "-1";
        }
        else
        {
            request.ProductCode = MultipleProductSearchRequest.ProductCode;
        }

        if (string.IsNullOrEmpty(MultipleProductSearchRequest.ProductName))
        {
            request.ProductName = "-1";
        }
        else
        {
            request.ProductName = MultipleProductSearchRequest.ProductName;
        }

        if (string.IsNullOrEmpty(MultipleProductSearchRequest.SourceWarehouseCode))
        {
            request.SourceWarehouseCode = "-1";
        }
        else
        {
            request.SourceWarehouseCode = MultipleProductSearchRequest.SourceWarehouseCode;
        }

        if (string.IsNullOrEmpty(MultipleProductSearchRequest.RegCode))
        {
            request.RegCode = "-1";
        }
        else
        {
            request.RegCode = MultipleProductSearchRequest.RegCode;
        }

        return request;
    }

    private async Task SetDynamicFieldsBySourceAndDestination()
    {
        if (DirectPlaceDto.SourceWarehouseCode.HasValue() && DirectPlaceDto.DestinationWarehouseCode.HasValue())
        {
            IsLoading = true;

            if (ActionTypeCode == -1)
            {
                ActionType = (await Api.PostAsync<GetAllActionTypesDto>("SGetActionTypeBySourceAndDestination",
                                new("fromWarehouse", DirectPlaceDto.SourceWarehouseCode),
                                new("toWarehouse", DirectPlaceDto.DestinationWarehouseCode)
                                )).Value;
            }

            DynamicFields = (await Api.PostAsync<List<GetAllDynamicFieldVm>>("SGetDynamicFieldsBySourceAndDestination",
                new("sourceWarehouseCode", DirectPlaceDto.SourceWarehouseCode),
                new("destinationWarehouseCode", DirectPlaceDto.DestinationWarehouseCode)
                )).Value;

            DynamicFieldsDto = DynamicFields.Where(p => p.FieldType == DynamicFieldType.HeaderData)
                                            .DistinctBy(p => p.Title)
                                            .Select(p => new DynamicFieldWithValueDto()
                                            {
                                                Title = p.Title,
                                                DefaultValue = p.DefaultValue,
                                                Value = p.DefaultValue,
                                                ValueOptions = p.ValueOptionList,
                                                ValueType = p.ValueType,
                                                IsRequired = p.IsRequired ?? false,
                                                IsReadOnly = p.IsReadOnly ?? false
                                            }).ToList();

            
                var jsonString = ActionType is null ? "{}" : ActionType.ActiveControls;

                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                if (jsonObject.TryGetValue("DocCodeSet", out string docCodeSetValue))
                {
                    ValidationDocCodeSet = bool.Parse(docCodeSetValue);
                }

                if (jsonObject.TryGetValue("DocItemsCheck1", out string docItemsCheck1Value))
                {
                    ValidationDocItemsCheck1 = bool.Parse(docItemsCheck1Value);
                }

                if (jsonObject.TryGetValue("TruckCrossShow", out string truckCrossShowValue))
                {
                    TruckCrossShow = bool.Parse(truckCrossShowValue);

                    if (!TruckCrossShow)
                    {
                        DirectPlaceDto.TruckCrossId = 0;
                    }
                }


                if (jsonObject.TryGetValue("TruckCrossRequirement", out string truckCrossRequirementValue))
                {
                    TruckCrossRequirement = bool.Parse(truckCrossRequirementValue);
                }
            
            IsLoading = false;
        }
        else
        {
            DynamicFields = new();

            DynamicFieldsDto = new();

            ValidationDocCodeSet = false;

            ValidationDocItemsCheck1 = false;
        }
    }

    private void SetErrorButtonClass()
    {
        if (FinalProductInfos.Any(p => p.Status.HasValue()))
        {
            ErrorButtonClass = "btn-danger";
        }
        else if (PlaceProductsAgg.Any(p => p.Status.HasValue()) || Documents.Any(p => p.Status.HasValue()))
        {
            ErrorButtonClass = "btn-warning";
        }
        else
        {
            ErrorButtonClass = "btn-light";
        }
    }

    private static DataTable GetDataTableFromList(List<GetPlaceByActionIdVm> data)
    {
        var propertyList = typeof(GetPlaceByActionIdVm).GetProperties().ToList();

        DataTable table = new();

        foreach (PropertyInfo prop in propertyList)
        {
            var attrList = prop.GetCustomAttributes().ToList(); //as DisplayAttribute;

            foreach (var attr in attrList)
            {
                if (attr is DisplayAttribute)
                {
                    var name = ((DisplayAttribute)attr).Name;
                    table.Columns.Add(ResourceManager.GetString(name), typeof(string));

                    break;
                }
            }
        }

        foreach (var item in data)
        {
            DataRow row = table.NewRow();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                row[i] = propertyList[i].GetValue(item) ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private async Task InitMetaData(string target = "")
    {
        if (target.HasNoValue())
        {
            target = NavigationManager.Uri;
        }

        if (ActionTypeCode != -1)
        {
            ActionType = (await Api.PostAsync<GetAllActionTypesDto>("SGetActionTypeByActionTypeCode",
                                new KeyValuePair<string, object>("actionTypeCode", ActionTypeCode)
                                )).Value;
        }

        await OnClearAllClick(new());

        await LoadWarehouses();

        InnerPermissions = new();

        InnerPermissions.Name = "PageControl";

        PermissionCheck();

        await SetSerialDeleteVisibility();

        await LoadStations();

        await LoadTruckCross();

        await SetDynamicFieldsBySourceAndDestination();
    }

    private void PermissionCheck()
    {
        if (Mode == 1)
        {
            PageTitle = TextResources.APP_StringKeys_Location_SecControl;
            InnerPermissions.AdditionalData.Add("ReadTag", "Gate,Handheld");
        }
        else if (Mode == 2)
        {
            PageTitle = TextResources.APP_StringKeys_View_Position_Place;
            InnerPermissions.AdditionalData.Add("Select", "Modal,Serial");
        }
        else
        {
            PageTitle = TextResources.APP_StringKeys_Location_SecExit;
            InnerPermissions.AdditionalData.Add("ReadTag", "Gate,Handheld,Modal,Serial");
        }
    }

    private async Task LoadWarehouses()
    {
        var allWarehouses = await FormalCache.GetWarehouses();

        if (ActionTypeCode != -1)
        {
            foreach (string type in ActionType.From.Split(','))
            {
                SourceWarehouses = allWarehouses.Where(p => p.InventoryType != DestinationInventoryType.Virtual
                                                                        && ((int)p.OperationalType).ToString() == type)
                                                .ToList();
            }

            foreach (string type in ActionType.To.Split(','))
            {
                DestinationWarehouses = allWarehouses.Where(p => ((int)p.OperationalType).ToString() == type)
                                                .ToList();
            }
        }
        else
        {
            SourceWarehouses = allWarehouses.Where(p => p.InventoryType != DestinationInventoryType.Virtual)
                                            .ToList();

            DestinationWarehouses = allWarehouses;
        }

        if (SourceWarehouses.Any())
        {
            DirectPlaceDto.SourceWarehouseCode = SourceWarehouses.First().DestinationCode;
        }

        if (DestinationWarehouses.Any())
        {
            DirectPlaceDto.DestinationWarehouseCode = DestinationWarehouses.First().DestinationCode;
        }
    }

    private async Task LoadTruckCross()
    {
        NotExitedCrosses = (await Api.PostAsyncByUri<List<TruckCrossDataDto>>("wms/TruckCross", "SGetUnexitedCrosses")).Value;

        if (TruckCrossId != -1)
        {
            DirectPlaceDto.TruckCrossId = (int)TruckCrossId;
        }
    }

    private async Task LoadStations()
    {
        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
       , new GetAllStationsVmContext())).Value;

        if (ActiveStations.HasValue() && ActiveStations.Split(',').Any())
        {
            var activeStations = ActiveStations.Split(',').ToList();

            Stations = Stations.Where(p => activeStations.Any(q => q.Equals(p.Code))).ToList();
        }

        if (Stations.Count >= 1)
        {
            SearchGateAction.UhfGateCode = Stations.First().Code;
        }
    }

    private async Task SetSerialDeleteVisibility()
    {
        if (Mode == 1)
        {
            ShowSerialDelete = (await Api.PostAsync<bool>("SGetSerialDeleteButtonVisibilityInGateMode")).Value;
        }
    }
    #endregion
}
