using AutoMapper;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Features;

namespace Silo.Pages;

public partial class Security
{
    public SecurityPageModes Mode;
    public bool IsLoading = true;
    public DocumentCheckType DefaultDocumentCheckType;
    public string PageTitle = string.Empty;
    public string DocumentCode = "0";
    public string GateOperationDateTime = string.Empty;
    public string ErrorsClass = "btn-light";
    public string MovementActionId = string.Empty;
    public string CurrentDestinationWarehouse = string.Empty;
    public List<GetAllGateProductVm> GateProducts = new();
    public List<GetAllGateProductDetailsVm> GateDetails = new();
    public List<GetDocProductDataByDocKeyVm> Documents = new();
    public List<GateProductErrorDto> Errors;
    public GetExitActionByUhfIdVm Operation = new();
    public SaveExitActionCommand SaveOperation = new();
    public List<GetAllWarehousesVm> Warehouses;
    public List<Get100LastActionsVm> LastActions;
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<ChoosableKeyValue> DynamicFieldsDto = new();
    public List<GetAllStationsVm> Stations;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public ProtectedLocalStorage Storage { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal ModalErrors { get; set; }
    public Modal ModalDetails { get; set; }
    public Modal ModalLastActions { get; set; }
    public ElementReference RefOperation { get; set; }
    #region Events
    /// <summary>
    /// This page have 2 parts of security check:
    /// Part 1 Check tag status.  
    /// Part 2 Check GateProducts with Documents(Docs).
    /// </summary>
    /// <returns></returns>
    protected override async Task SiloInitializer()
    {
       

        NavigationManager.RegisterLocationChangingHandler(OnLocationChanged);

        Warehouses = (await FormalCache.GetWarehouses()).Where(p => p.InventoryType == DestinationInventoryType.Physical).ToList();

        await GetGateAndDestination();

        await Init();

        DefaultDocumentCheckType = (await Api.PostAsync<DocumentCheckType>("SIsMustCheckDocItemsRemain")).Value;

        IsLoading = false;
    }

    private async Task Init(string target = "")
    {
        IsLoading = true;

        if (target.HasNoValue())
        {
            target = NavigationManager.Uri;
        }

        await RefOperation.FocusAsync();

        if (target.Contains("/gate/securityGateDoc"))
        {
            PageTitle = TextResources.APP_StringKeys_View_Security_Index_Doc_required;

            Mode = SecurityPageModes.GateAndDoc;
        }
        else
        {
            PageTitle = TextResources.APP_StringKeys_View_Security_Index;

            Mode = SecurityPageModes.GateOnly;
        }

        Operation.GateOperationCode = (await Api.PostAsync<int>("SGetMaxInvIdByGate",
               new KeyValuePair<string, object>[] { new("gate", Operation.Gate) })).Value;

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
        , new GetAllStationsVmContext())).Value;

        IsLoading = false;

        StateHasChanged();
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;
        Operation.GateOperationCode = 0;
        DocumentCode = "0";
        GateOperationDateTime = string.Empty;
        ErrorsClass = "btn-light";
        GateProducts = new();
        GateDetails = new();
        Documents = new();
        Errors = new();
        Operation = new();
        MovementActionId = string.Empty;
        DynamicFields = new();
        DynamicFieldsDto = new();
        CurrentDestinationWarehouse = string.Empty;
    }

    public async Task OnSaveClick()
    {
        IsLoading = true;

        if (!await CheckBeforeSave())
        {
            IsLoading = false;

            return;
        }

        Operation.DocumentId = DocumentCode.ToString();

        dynamic exo = new System.Dynamic.ExpandoObject();

        foreach (var field in DynamicFieldsDto)
        {
            ((IDictionary<String, Object>)exo).Add(field.Key, field.Value);
        }

        Operation.MovementActionData = Newtonsoft.Json.JsonConvert.SerializeObject(exo);

        SaveOperation = Mapper.Map<SaveExitActionCommand>(Operation);

        SaveOperation.SourceWarehouseCode = GateProducts.First(p => p.SourceWarehouseCode.HasValue()).SourceWarehouseCode;

        SaveOperation.DestinationWarehouseCode = Operation.DestinationWarehouseCode;

        var result = await Api.PostAsync<bool>("SSaveProductExitAction",
                        new KeyValuePair<string, object>("exitAction", SaveOperation));

        if (result.Value)
        {
            await SetGateAndDestination(SaveOperation.Gate, SaveOperation.DestinationWarehouseCode);

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async ValueTask OnLocationChanged(LocationChangingContext context)
    {
        await OnClearClick(new());

        await Init(context.TargetLocation);
    }

    public async Task OnDestinationWarehouseChange(object e)
    {
        var newDestination = string.Empty;

        if (e is not null)
        {
            newDestination = e as string;
        }

        if (CurrentDestinationWarehouse.NotEquals(newDestination))
        {
            CurrentDestinationWarehouse = newDestination;

            await SetDynamicFieldsBySourceAndDestination();
        }
    }
    #endregion

    #region GetOp
    public async Task OnGetOpKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            IsLoading = true;

            await GetRfidProductsData();

            await GetRfidMovementActionData();

            await GetDocuments();

            IsLoading = false;
        }
    }

    public async Task OnGetPreviousOpClick(MouseEventArgs e)
    {
        if (Operation.GateOperationCode.ToString().HasValue())
        {
            IsLoading = true;

            Operation.GateOperationCode = (await Api.PostAsync<int>("SGetNextPreviousInvIdByCurrentId"
                                , new KeyValuePair<string, object>[] { new("isNext", false), new("invId", Operation.GateOperationCode), new("gate", Operation.Gate ?? string.Empty) })).Value;

            if (Operation.GateOperationCode.Equals(0))
            {
                Notification.Show(TextResources.APP_StringKeys_Error_Notfound_Operation_Code, "error");

                GateOperationDateTime = string.Empty;

                MovementActionId = string.Empty;
            }
            else
            {
                await GetRfidProductsData();

                await GetRfidMovementActionData();

                await GetDocuments();
            }

            IsLoading = false;
        }

    }

    public async Task OnGetNextOpClick(MouseEventArgs e)
    {
        if (Operation.GateOperationCode.ToString().HasValue())
        {
            IsLoading = true;

            Operation.GateOperationCode = (await Api.PostAsync<int>("SGetNextPreviousInvIdByCurrentId"
                                , new KeyValuePair<string, object>[] { new("isNext", true), new("invId", Operation.GateOperationCode), new("gate", Operation.Gate ?? string.Empty) })).Value;

            if (Operation.GateOperationCode.Equals(0))
            {
                Notification.Show(TextResources.APP_StringKeys_Error_Notfound_Operation_Code, "error");

                GateOperationDateTime = string.Empty;

                MovementActionId = string.Empty;
            }
            else
            {
                await GetRfidProductsData();

                await GetRfidMovementActionData();

                await GetDocuments();
            }

            IsLoading = false;
        }
    }

    public async Task OnClickRowDetails(string code)
    {
        IsLoading = true;

        GateDetails = (await Api.PostAsync<List<GetAllGateProductDetailsVm>>("SPSecurityGateReportDetails"
                        , new("gateOperationCode", Operation.GateOperationCode)
                        , new("ProductCode", code))).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    public async Task OnChooseLastActionClick(int code)
    {
        await ModalLastActions.Close(new());

        Operation.GateOperationCode = code;

        await GetRfidProductsData();

        await GetRfidMovementActionData();

        await GetDocuments();
    }

    public async Task OnGetLastActionsDataClick(MouseEventArgs e)
    {
        if (Operation.Gate.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Gate_Required, "error");

            return;
        }

        IsLoading = true;

        LastActions = (await Api.PostAsyncByContext<List<Get100LastActionsVm>>("SReportSecurityTags"
            , new Get100LastActionsVmContext()
            , new KeyValuePair<string, object>("gate", Operation.Gate))).Value;

        await ModalLastActions.Open(new());

        IsLoading = false;
    }
    #endregion

    #region GetDocument
    public async Task OnSearchDocumentClick(MouseEventArgs e)
    {
        await GetDocuments();
    }

    public async Task OnDocumentKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await GetDocuments();
        }
    }
    #endregion

    #region OnRowRenderHandlers and find conflicts

    /// <summary>
    /// GateProducts can have a ProductCode that is not in in Docs. "مغایرت در کد کالا"
    /// || Same ProductCode in GateProducts and Docs can have diffrent value. "مقدار مغایرت ..." 
    /// </summary>
    /// <param name="args"></param>
    public void OnRowOpRenderHandler(GridRowRenderEventArgs args)
    {
        GetAllGateProductVm item = (GetAllGateProductVm)args.Item;

        if (Documents is not null && Documents.Any())
        {
            foreach (GetDocProductDataByDocKeyVm doc in Documents)
            {
                if (doc.ProductCode.Equals(item.ProductCode))
                {
                    if (item.ProductCount != doc.SumValue)
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

    /// <summary>
    /// Docs can have a ProductCode that is not in GateProducts. "عدم شناسایی کد کالا"
    /// </summary>
    /// <param name="args"></param>
    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        GetDocProductDataByDocKeyVm item = (GetDocProductDataByDocKeyVm)args.Item;

        if (GateProducts is not null && GateProducts.Any())
        {
            foreach (GetAllGateProductVm op in GateProducts)
            {
                if (op.ProductCode.Equals(item.ProductCode))
                {
                    item.Status = "";
                    return;
                }
            }
        }
        else
        {
            return;
        }

        args.Class += " bg-warning";

        item.Status = "مغایرت عدم شناسایی کالا";
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Get Products that are readed by gate in an operation
    /// </summary>
    /// <returns></returns>
    private async Task GetRfidProductsData()
    {
        IsLoading = true;

        if (string.IsNullOrEmpty(Operation.GateOperationCode.ToString()))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Required_OperationCode, "error");
        }
        else
        {
            int gateOperationCode = Operation.GateOperationCode;

            string gateCode = Operation.Gate;

            string destination = Operation.DestinationWarehouseCode;

            Operation = new();

            Operation.GateOperationCode = gateOperationCode;

            Operation.Gate = gateCode;

            Operation.DestinationWarehouseCode = destination;

            List<GetAllGateProductVm> ops = (await Api.PostAsyncByContext<List<GetAllGateProductVm>>("SPSecurityGateReport"
                   , new GetAllGateProductVmContext()
                   , new("gate", Operation.Gate ?? "-1")
                   , new("gateOperationCode", Operation.GateOperationCode))).Value;

            if (ops is not null)
            {
                GateProducts = ops.Where(p => p.Error.Equals("3")).ToList();

                Errors = Mapper.Map<List<GateProductErrorDto>>(ops.Where(p => p.Error.Equals("4")).ToList());

                if (Errors.Any())
                {
                    ErrorsClass = "btn-danger";
                }
                else
                {
                    ErrorsClass = "btn-light";
                }

                GateOperationDateTime = "تاریخ و ساعت: " +
                    ops.FirstOrDefault(p => !string.IsNullOrEmpty(p.MaxDate))?.MaxDate.ToNormalPersianDateTime();

                await SetDynamicFieldsBySourceAndDestination();
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }

        IsLoading = false;
    }

    /// <summary>
    /// Get Information of MovmentAction's Static's and Data's)
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task GetRfidMovementActionData()
    {
        IsLoading = true;

        Documents = new();

        MovementActionId = string.Empty;

        var op = (await Api.PostAsyncByContext<List<GetExitActionByUhfIdVm>>("SGetMovementActionByUhfId"
                 , new GetExitActionByUhfIdVmContext()
                 , new("uhfId", Operation.GateOperationCode)
                 , new("userToken", (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId()))).Value;

        if (op is not null && op.Any())
        {
            string gateCode = Operation.Gate;

            Operation = op.First();

            Operation.Gate = gateCode;

            DocumentCode = Operation.DocumentId;

            MovementActionId = op.First().MovementActionId.ToString().HasValue() ?
                string.Format(TextResources.APP_StringKeys_Alert_MovementAction_Submit, Operation.MovementActionId.ToString()) :
                string.Empty;
        }
        else
        {
            DocumentCode = "0";
        }

        IsLoading = false;
    }

    /// <summary>
    /// Get Products that are submited in a document
    /// </summary>
    /// <returns></returns>
    private async Task GetDocuments()
    {
        if (Mode.Equals(0))
        {
            return;
        }

        IsLoading = true;

        if (DocumentCode.HasNoValue() || DocumentCode.Equals("0"))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Doc_Code_Required, "error");
        }
        else
        {
            Documents = (await Api.PostAsyncByContext<List<GetDocProductDataByDocKeyVm>>("SGetDocProductDataByDocKey"
                 , new GetDocProductDataByDocKeyVmContext()
                 , new KeyValuePair<string, object>[] { new("code", DocumentCode) })).Value;

            if (Documents.Any())
            {
                var result = Documents.FirstOrDefault(p => p.DocumentHeaderData.HasValue()).DocumentHeaderData;

                JToken headerData = JToken.Parse(result);

                foreach (var field in DynamicFieldsDto)
                {
                    if (headerData[$"{field.Key?.Trim()}"] is not null)
                    {
                        try
                        {
                            field.Value = headerData[$"{field.Key?.Trim()}"].ToString();
                        }
                        catch
                        {
                            field.Value = "";
                        }
                    }

                }
            }
        }

        IsLoading = false;
    }

    private async Task<bool> CheckBeforeSave()
    {
        if (GateProducts is null ||
            !GateProducts.Any())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_EmptyRows, TextResources.APP_StringKeys_Operation), "error");

            return false;
        }

        if (Operation.DestinationWarehouseCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination), "error");

            return false;
        }

        if (Mode == SecurityPageModes.GateAndDoc)
        {
            if (Documents is null || !Documents.Any())
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_EmptyRows, TextResources.APP_StringKeys_Doc), "error");

                return false;
            }

            if (GateProducts.Any(x => x.Status != "") ||
                Documents.Any(x => x.Status != ""))
            {
                Notification.Show(TextResources.APP_StringKeys_Contradiction, "error");

                return false;
            }
        }

        if (ErrorsClass == "btn-danger")
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Prevent_On_Errors, "error");

            return false;
        }
        return true;
    }

    private async Task GetGateAndDestination()
    {
        var gateStorageResult = await Storage.GetAsync<string>("gate");

        var destStorageResult = await Storage.GetAsync<string>("dest");

        if (gateStorageResult.Success)
        {
            Operation.Gate = gateStorageResult.Value;
        }

        if (destStorageResult.Success)
        {
            Operation.DestinationWarehouseCode = destStorageResult.Value;

            CurrentDestinationWarehouse = destStorageResult.Value;
        }
    }

    private async Task SetGateAndDestination(string gate, string dest)
    {
        await Storage.SetAsync("gate", gate);

        await Storage.SetAsync("dest", dest);
    }

    private async Task SetDynamicFieldsBySourceAndDestination()
    {
        if (GateProducts.Any(p => p.SourceWarehouseCode.HasValue()) && Operation.DestinationWarehouseCode.HasValue())
        {
            IsLoading = true;

            DynamicFields = (await Api.PostAsync<List<GetAllDynamicFieldVm>>("SGetDynamicFieldsBySourceAndDestination",
                new("sourceWarehouseCode", GateProducts.First(p => p.SourceWarehouseCode.HasValue()).SourceWarehouseCode),
                new("destinationWarehouseCode", Operation.DestinationWarehouseCode)
                )).Value;

            DynamicFieldsDto = DynamicFields.Where(p => p.FieldType == DynamicFieldType.HeaderData).DistinctBy(p=>p.Title).Select(p => new ChoosableKeyValue() { Key = p.Title }).ToList();

            IsLoading = false;
        }
        else
        {
            DynamicFields = new();

            DynamicFieldsDto = new();
        }
    }
    #endregion
}

public enum SecurityPageModes
{
    GateOnly,
    GateAndDoc
}
