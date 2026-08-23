using System.Text.Encodings.Web;
using System.Text.Json;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Shared.Components.Print;
using Silo.Shared.Tools;

namespace Silo.Pages.Reports;
public partial class ExitActions
{
    private int FilterCount = 1;
    private int CurrentActionType = 0;
    public bool IsLoading = true;
    public string UserId;
    public List<GetAllExitActionVm> Exits;
    public List<GetAllExitActionOnProductCodeVm> Aggregates;
    public List<GetAllExitActionDetailsVm> FullDetails;
    public List<GetAllExitActionDetailsVm> AggDetails;
    public List<GetAllExitActionDetailsByExitCodeVm> Details;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses = new();
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllActionTypesDto> ActionTypes;
    public List<string> TechnicalInfoDataKeys = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<string> TechnicalFilterColumns = new();
    public string CompanyName;
    public List<TelerikDropDownItem> MovementActionDataItems = new();
    public GetAllExitActionVm InfoAction;
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public GetActionTruckCrossVm ActionTruckCross = new();
    public List<TruckCrossDataDto> NotExitedCrosses;
    public List<GetAllTruckTypesVm> TruckTypes;
    public List<GetAllTruckCrossPresentCauseVm> Causes;
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes;
    public List<GetAllTruckCrossShipmentVm> Shipments;
    public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations;
    public List<GetAllStationsVm> Stations;
    public GetAllExitActionVm PrintData;

    //Action Type = 1 - Update april 2025
    public List<string> DynamicFieldRegisterDataColumns = new();
    public List<string> DynamicFieldForActionTypeDataColumns = new();

    [Parameter] public int TabMode { get; set; } = 0;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public ILogger<ExitActions> Logger { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    public Modal DetailsModal { get; set; }
    public Modal DetailsAggModal { get; set; }
    public Modal FiltersModal { get; set; }
    public Modal InfoModal { get; set; }
    public Modal ModalTruckCross { get; set; }
    public Modal ModalTruckCrossNewChoose { get; set; }

    public SelectPrintFormat SelectPrintFormatRef { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Sizes = await FormalCache.GetSizes();

        Warehouses = await FormalCache.GetWarehouses();

        Groups = await FormalCache.GetGroups();

        Brands = await FormalCache.GetBrands();

        ProductTypes = await FormalCache.GetTypes();

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
                , new GetAllStationsVmContext())).Value;

        SearchWarehouses = Warehouses;

        TechnicalInfoDataKeys = (await Api.PostAsyncByUri<List<string>>("wms/Product", "SGetAllTechnicalInfoDataKeys")).Value;

        TruckTypes = (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value;

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

        OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetAllTruckCrossOperationType")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        OperationDestinations = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value;

        Lines = await FormalCache.GetLines();

        Shifts = await FormalCache.GetShifts();

        InitFilters();

        IsLoading = false;
    }

    public async Task OnSearchClick()
    {
        var filters = FillFilters();

        if (filters is null)
        {
            return;
        }

        if (TabMode == 2 || TabMode == 0)
        {
            Exits = (await Api.PostAsyncByContext<List<GetAllExitActionVm>>("SReportExitActions",
            new GetAllExitActionVmContext()
            , new KeyValuePair<string, object>("reportFilters", filters) )).Value;
         }

        if (TabMode == 1 || TabMode == 0)
        {
            Aggregates = (await Api.PostAsyncByContext<List<GetAllExitActionOnProductCodeVm>>("SReportExitActionTajamoeeProductCode",
            new GetAllExitActionOnProductCodeVmContext(),
            new KeyValuePair<string, object>("reportFilters", filters))).Value;
        }

        if (TabMode == 3 || TabMode == 0)
        {
            FullDetails = (await Api.PostAsyncByContext<List<GetAllExitActionDetailsVm>>("SReportExitActionFull",
                  new GetAllExitActionDetailsVmContext(),
                      new KeyValuePair<string, object>("reportFilters", filters))).Value;

        }

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnExitActionDetailsClick(GetAllExitActionVm product)
    {
        IsLoading = true;

        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        ReportFilter operationCodeFilter = Filters.First(p => p.FieldName.Equals("OperationCode"));

        ReportFilter storeCodeFilter = Filters.First(p => p.FieldName.Equals("StoreCode"));

        ReportFilter actionTypeFilter = Filters.First(p => p.FieldName.Equals("ActionType"));

        operationCodeFilter.Values = new()
        {
            product.OpCode
        };

        storeCodeFilter.Values = new()
        {
            product.StoreCode
        };

        actionTypeFilter.Values = new()
        {
            product.ActionType?.ToString()
        };

        filters.RemoveAll(p => p.FieldName.Equals("OperationCode"));

        filters.RemoveAll(p => p.FieldName.Equals("StoreCode"));

        filters.RemoveAll(p => p.FieldName.Equals("ActionType"));

        filters.Add(operationCodeFilter);

        filters.Add(storeCodeFilter);

        filters.Add(actionTypeFilter);

        Details = (await Api.PostAsyncByContext<List<GetAllExitActionDetailsByExitCodeVm>>("SReportExitActionByOpCode"
            , new GetAllExitActionDetailsByExitCodeVmContext()
            , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        await DetailsModal.Open(new());

        IsLoading = false;
    }

    public async Task OnExitActionAggOnProductCodeDetailsClick(GetAllExitActionOnProductCodeVm product)
    {
        IsLoading = true;

        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        ReportFilter productCodeFilter = Filters.First(p => p.FieldName.Equals("ProductCode"));

        ReportFilter actionTypeFilter = Filters.First(p => p.FieldName.Equals("ActionType"));

        productCodeFilter.Values = new()
        {
            product.ProductCode
        };

        actionTypeFilter.Values = new()
        {
            product.ActionType?.ToString()
        };

        filters.RemoveAll(p => p.FieldName.Equals("ProductCode"));

        filters.RemoveAll(p => p.FieldName.Equals("ActionType"));

        filters.Add(productCodeFilter);

        filters.Add(actionTypeFilter);

        AggDetails = (await Api.PostAsync<List<GetAllExitActionDetailsVm>>("SReportExitActionFull"
            , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        await DetailsAggModal.Open(new());

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        Exits = null;

        Aggregates = null;

        FullDetails = null;

        Details = null;

        ApplyFilters = new();

        CurrentActionType = 0;

        SearchWarehouses = Warehouses;
    }

    public async Task OnActionInfoClick(GetAllExitActionVm action)
    {
        MovementActionDataItems.Clear();

        if (action.MovementActionData.HasValue())
        {
            var jsonData = JToken.Parse(action.MovementActionData);

            foreach (JProperty prop in jsonData)
            {
                MovementActionDataItems.Add(new()
                {
                    Name = prop.Name,
                    Value = prop.Value.ToString()
                });
            }
        }

        InfoAction = action;

        await InfoModal.Open(new());
    }

    public async Task OnSaveInfoClick(MouseEventArgs e)
    {
        try
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            foreach (var item in MovementActionDataItems)
            {
                ((IDictionary<String, Object>)exo).Add(item.Name, item.Value);
            }

            string data = JsonSerializer.Serialize(exo, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            bool result = (await Api.PostAsync<bool>("SSaveMovementData"
                           , new KeyValuePair<string, object>("command", new UpdateActionDataCommand
                           {
                               ActionId = InfoAction.OpCode,
                               Data = data
                           }))).Value;

            if (result)
            {
                InfoAction.MovementActionData = data;

                Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);

            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    #region Export
    public async Task OnClickExportExitsToPdfMaster()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetAllExitActionVm), Exits)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
            new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}"),
            new("CompanyName", CompanyName),
            new("PageTitle", PageTitle)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ExitAction",
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

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnClickExportAggregatesToPdfMaster()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetAllExitActionOnProductCodeVm), Aggregates)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
             	new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}"),
             	new("CompanyName", CompanyName),
             	new("PageTitle", PageTitle)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ExitActionAgg",
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

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnExcelBeforeExport(GridBeforeExcelExportEventArgs args, string fileName)
    {
        IsLoading = true;

        var dataTable = ExcelExportTools.GetDataTableWithDynamicColumnAndValues(args);

        var stream = DataTableTools.GetExcelFromDataTable(dataTable);

        stream.Seek(0, SeekOrigin.Begin);

        await Exporter.ExportAndDownload(stream, $"{fileName}.xlsx");

        IsLoading = false;
    }

    public async Task OnSelectProductForPrint(GetAllExitActionVm product)
    {
        PrintData = product;

        if (SelectPrintFormatRef is not null)
        {
            await SelectPrintFormatRef.ShowPrintFormatsAsync(new());
        }
    }

    public async Task OnPrintActionClick(GetPrintFormatsByPageTitleDto format)
    {
        IsLoading = true;

        MovementActionPrintDto printAction = new();

        printAction = Mapper.Map<MovementActionPrintDto>(PrintData);

        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        PrepareFilters();

        var serials =
            (await Api.PostAsyncByContext<List<TagMovementPrintDto>>("SReportExitActionByOpCode"
                    , new TagMovementPrintDtoContext()
                    , new KeyValuePair<string, object>("reportFilters", filters))).Value;

        var truckCrosses = (await Api.PostAsync<List<GetActionTruckCrossVm>>("SGetTruckCrossByMovementActionId"
                , new KeyValuePair<string, object>("movementActionId", printAction.OpCode))).Value;

        if (truckCrosses.Any())
        {
            printAction.TruckCross = truckCrosses.First();
        }

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
                    Serials = serial.ProductSerial
                };
            }
            else
            {
                print.Serials += $" - {serial.ProductSerial}";
            }

            print.SumCount += serial.ProductCount;

            print.Count++;

            printData.ReplaceOrAdd(p => p.ProductCode.Equals(serial.ProductCode), print);
        }

        printAction.ExitPrints = printData;

        await PrintAction(printAction);

        IsLoading = false;

        async Task PrintAction(MovementActionPrintDto printableAction)
        {
            if (CompanyName.HasNoValue())
            {
                CompanyName = Configuration.GetSection("Settings")["Company"];
            }

            var variables = new List<KeyValuePair<string, object>>()
            {
                  new("DateString", $"{PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
                , new("ActionTypeTitle", printableAction.ActionTypeTitle)
                , new("OperationCode", printableAction.OpCode)
                , new("DateTimeString", printableAction.DateTime)
                , new("User", printableAction.User)
                , new("Count", printableAction.Count)
                , new("GateCode", printableAction.GateCode)
                , new("GateOpCode", printableAction.GateOp)
                , new("StationName", printableAction.StationName)
                , new("StationNames", printableAction.StationName)
                , new("Document", printableAction.ActionDocumentId)
                , new("Description", printableAction.MovementActionDesc)
                , new("CompanyName", CompanyName)
                , new("PageTitle", PageTitle)
            };

            if (printableAction.TruckCross is not null)
            {
                variables.Add(new("TruckDriverName", printableAction.TruckCross.DriverName));
                variables.Add(new("TruckDriverPhone", printableAction.TruckCross.DriverPhone));
                variables.Add(new("TruckDriverNationalCode", printableAction.TruckCross.NationalCode));
                variables.Add(new("TruckPlaque", printableAction.TruckCross.Plaque));
                variables.Add(new("TruckDriverLicenseCode", printableAction.TruckCross.LicenseCode));
                variables.Add(new("TruckType", printableAction.TruckCross.TypeTitle));
                variables.Add(new("TruckEnterWeight", printableAction.TruckCross.EnterWeightTonage));
                variables.Add(new("TruckExitWeight", printableAction.TruckCross.ExitWeightTonage));
            }

            if (printableAction.MovementActionData.HasValue())
            {
                var headerDatas = JToken.Parse(printableAction.MovementActionData);

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
                new(nameof(ExitActionPrintMainDto), printableAction.ExitPrints)
            };

            CreatePreparedReportCommand command = new ()
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

            await Export.ExportAndDownloadUsingBypass(response.Value.Result);
        }

        void PrepareFilters()
        {
            ReportFilter storeCodeFilter = Filters.First(p => p.FieldName.Equals("StoreCode"));

            ReportFilter actionTypeFilter = Filters.First(p => p.FieldName.Equals("ActionType"));

            ReportFilter operationCodeFilter = Filters.First(p => p.FieldName.Equals("OperationCode"));

            storeCodeFilter.Values = new()
        {
            PrintData.StoreCode
        };

            actionTypeFilter.Values = new()
        {
            PrintData.ActionType?.ToString()
        };

            operationCodeFilter.Values = new()
        {
            PrintData.OpCode
        };

            filters.RemoveAll(p => p.FieldName.Equals("StoreCode"));

            filters.RemoveAll(p => p.FieldName.Equals("ActionType"));

            filters.RemoveAll(p => p.FieldName.Equals("OperationCode"));

            filters.Add(storeCodeFilter);

            filters.Add(actionTypeFilter);

            filters.Add(operationCodeFilter);
        }
    }
    #endregion

    #region TruckCross
    public async Task OnTruckCrossClick(string movementActionId)
    {
        IsLoading = true;

        var crosses = (await Api.PostAsync<List<GetActionTruckCrossVm>>("SGetTruckCrossByMovementActionId"
                , new KeyValuePair<string, object>("movementActionId", movementActionId)))
                .Value;

        if (crosses is not null
            && crosses.Any())
        {
            ActionTruckCross = crosses.First();
        }
        else
        {
            ActionTruckCross = new();
        }

        ActionTruckCross.MovementActionId = movementActionId;

        await ModalTruckCross.Open(new());

        IsLoading = false;
    }

    public async Task OnTruckCrossChangeClick(MouseEventArgs e)
    {
        IsLoading = true;

        NotExitedCrosses = (await Api.PostAsyncByUri<List<TruckCrossDataDto>>("wms/TruckCross", "SGetUnexitedCrosses")).Value;

        await ModalTruckCross.Close(e);

        await ModalTruckCrossNewChoose.Open(e);

        IsLoading = false;
    }

    public async Task OnTruckCrossChooseForChange(TruckCrossDataDto cross)
    {
        IsLoading = true;

        await ModalTruckCrossNewChoose.Close(new());

        bool result = (await Api.PostAsync<bool>("SUpdateMovementActionTruckCross"
            , new("actionId", ActionTruckCross.MovementActionId)
            , new("newTruckCrossId", cross.Id))).Value;

        IsLoading = false;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }
    #endregion

    #region Filters
    public async void OnAddNewFilterClick(List<ReportFilter> filters)
    {
        if (filters.Where(p => p.FieldName.Equals("ActionType")).Count() > 1)
        {
            await FiltersModal.Close(new());

            Notification.Show(TextResources.APP_StringKeys_ActionType_OnlyOne_Validation, "error");

            return;
        }
        if (filters.Any(p => p.FieldName.Equals("ActionType")))
        {
            ApplyFilters.RemoveAll(p => p.FieldName.Equals("ActionType"));
        }
        if (filters.Any(p => p.FieldName.Equals("ActionType")))
        {
            CurrentActionType = int.Parse(filters.FirstOrDefault(p => p.FieldName.Equals("ActionType")).Value);
        }

        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        Filters = new();

        InitFilters();

        await SetDynamicFiltersByActionType(new() { 0, CurrentActionType });

        SetWarehousesByActionType(CurrentActionType);

        await FiltersModal.Open(e);

        StateHasChanged();
    }

    public async Task OnAddNewFilterInComponentClick(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await OnActionTypeChange(filter);
        }
    }

    public async Task OnRemoveFilterInComponentClick(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await OnActionTypeChange(null);
        }
    }

    public async Task OnChangeFilterValueInEditComponent(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await OnActionTypeChange(filter);
        }
    }

    #region InitFilters
    private void InitFilters()
    {
        #region Static Filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ActionType",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ActionType,
            Items = ActionTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code.Value.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "StoreCode",
            Type = FilterType.Static,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Warehouse,
            Items = SearchWarehouses.Select(p => new ReportDataItem()
            {
                Label = p.DestinationTitle,
                Value = p.DestinationCode
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromDate",
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.BiggerThan,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToDate",
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ToDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Size",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Product_Size,
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductCode",
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_ProductCode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TechnicalCode",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Chart_Regcode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "OperationCode",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_OperationCode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "GateOpCode",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_GateOpCode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "GateCode",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_Station,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = Stations.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductGroup",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_ProductGroup,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = Groups.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductBrand",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_ProductBrand,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = Brands.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductType",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ProductType,
            Items = ProductTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DocumentKey",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_DocKey,
            Component = FilterComponent.Text
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "RecordsCount",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_RecordsCount,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = GetRecordsCounts().Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckType",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_TypeTruck,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = TruckTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "CarPlaque",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "NationalCode",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_NationalCode + " " + TextResources.APP_StringKeys_Driver + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DriverName",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_DriverName + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossCause",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Present_Cause + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = Causes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossOperationType",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Type + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = OperationTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossShipment",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Shipment + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Items = Shipments.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossOperationDestination",
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Destination + " " + TextResources.APP_StringKeys_TruckCross,
            Items = OperationDestinations.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Line",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Line,
            Items = Lines.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Shift",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Chart_Shift,
            Items = Shifts.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });
        #endregion

        #region Technical Filters
        foreach (var field in TechnicalInfoDataKeys)
        {
            Filters.Add(new()
            {
                Id = FilterCount++,
                Label = field,
                FieldName = field,
                IsLikeCheckboxShown = true,
                Type = FilterType.TechnicalInfo,
                Component = FilterComponent.Text
            });
        }
        #endregion
    }

    /// <summary>
    /// This method Set Warehouse list and dynamic filters based on chosen actionType.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task OnActionTypeChange(ReportFilter actionType)
    {
        int actionTypeId = actionType is null ? 0 : int.Parse(actionType.Value);

        IsLoading = true;
        
        CurrentActionType = actionTypeId;

        SetWarehousesByActionType(actionTypeId);

        await SetDynamicFiltersByActionType(new() { 0, actionTypeId });

        IsLoading = false;

        StateHasChanged();
    }

    private async Task SetDynamicFiltersByActionType(List<int> actionTypeIds)
    {
        if (actionTypeIds.Any())
        {
            DynamicFields = new();

            foreach (var actionTypeId in actionTypeIds)
            {
                var dynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", actionTypeId))).Value;

                DynamicFields.AddRange(dynamicFields);
            }

            Filters.RemoveAll(p => p.Type == FilterType.Dynamic);

            foreach (var field in DynamicFields.Where(p => p.FieldType != DynamicFieldType.ItemData))
            {
                if (field.ValueType == DynamicFieldValueType.TextBox)
                {

                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Text,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });

                }
                else if (field.ValueType == DynamicFieldValueType.DropDown)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        Component = FilterComponent.Drop,
                        Type = FilterType.Dynamic,
                        IsLikeCheckboxShown = false,
                        Value = field.DefaultValue,
                        FieldName = field.Title,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        },
                        Items = field.ValueOptionList.Select(p => new ReportDataItem()
                        {
                            Label = p,
                            Value = p
                        }).ToList()
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.RichTextEditor)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.RichTextEditor,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
                else if (field.ValueType == DynamicFieldValueType.Numeric)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Numeric,
                        IsLikeCheckboxShown = false,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
            }

            Filters = Filters.OrderBy(p => p.Type).ToList();
        }
        else
        {
            Filters.RemoveAll(p => p.Type == FilterType.Dynamic);
        }
    }

    private void SetWarehousesByActionType(int? actionTypeId)
    {
        if (actionTypeId != 0)
        {
            var actionType = ActionTypes.FirstOrDefault(p => p.Code == actionTypeId);

            List<GetAllWarehousesVm> items = new();

            foreach (string warehouseType in actionType.To.Split(','))
            {
                if (warehouseType.HasNoValue())
                {
                    continue;
                }

                var warehouses = Warehouses.Where(p => p.OperationalType == Enum.Parse<DestinationOperationalType>(warehouseType))
                                           .ToList();

                foreach (var warehouse in warehouses)
                {
                    items.Add(warehouse);
                }
            }

            SearchWarehouses = items;
        }
        else
        {
            SearchWarehouses = Warehouses;
        }

        Filters.FirstOrDefault(p => p.FieldName.Equals("StoreCode")).Items = SearchWarehouses.Select(p => new ReportDataItem()
        {
            Label = p.DestinationTitle,
            Value = p.DestinationCode
        }).ToList();
    }
    #endregion
    #endregion

    #region Private
    private List<TelerikDropDownItem> GetRecordsCounts()
    {
        List<TelerikDropDownItem> items = new();
        items.Add(new TelerikDropDownItem()
        {
            Name = "100",
            Value = "100"
        });
        items.Add(new TelerikDropDownItem()
        {
            Name = "200",
            Value = "200"
        });
        items.Add(new TelerikDropDownItem()
        {
            Name = "500",
            Value = "500"
        });
        items.Add(new TelerikDropDownItem()
        {
            Name = "1000",
            Value = "1000"
        });
        items.Add(new TelerikDropDownItem()
        {
            Name = "2000",
            Value = "2000"
        });
        return items;
    }

    private List<ReportFilter> FillFilters()
    {
        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        var actionType = filters.FirstOrDefault(p => p.FieldName.Equals("ActionType"));

        if (actionType is null)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required,
                                            TextResources.APP_StringKeys_ActionType)
                              , "error");
            return null;
        }

        IsLoading = true;

        TechnicalFilterColumns = new();

        foreach (var filter in ApplyFilters.Where(p => p.Type == FilterType.TechnicalInfo))
        {
            TechnicalFilterColumns.Add(filter.Label);
        }

        foreach (var filter in ApplyFilters.Where(p => p.Type == FilterType.Dynamic))
        {
            if (filter.AdditionalData.First(p => p.Key.Equals("DynamicFilterActionType")).Value == "0")
            {
                DynamicFieldRegisterDataColumns.ReplaceOrAdd(p=> p.Equals(filter.Label) ,filter.Label);
            }
        }

        foreach (var dynamicField in DynamicFields)
        {
            if (dynamicField.ActionType != 0 || !dynamicField.FieldShowColumn)
            {
                continue;
            }

            DynamicFieldRegisterDataColumns.ReplaceOrAdd(p => p.Equals(dynamicField.Title), dynamicField.Title);
        }

        foreach (var dynamicField in DynamicFields)
        {
            if (dynamicField.ActionType != CurrentActionType || !dynamicField.FieldShowColumnForAction)
            {
                continue;
            }

            DynamicFieldForActionTypeDataColumns.ReplaceOrAdd(p => p.Equals(dynamicField.Title), dynamicField.Title);
        }

        return filters;
    }
    #endregion
}
