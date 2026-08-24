using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using AutoMapper;
using Silo.Shared.Tools;
using Silo.Shared.Components.Print;
using Silo.Application;

namespace Silo.Pages.Reports;
public partial class EnterActions
{
    private int FilterCount = 1;
    private int CurrentActionType = 0;
    public bool IsLoading = true;
    public GetEnterActionsQuery Request = new();
    public List<GetAllEnterActionsVm> Enters;
    public List<GetEnterActionAggregateOnProductCodeVm> Aggregates;
    public List<GetEnterActionDetailsFullVm> FullDetails;
    public List<GetEnterActionDetailsFullVm> AggDetails;
    public List<GetEnterActionDetailsVm> Details;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public List<string> TechnicalInfoDataKeys = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<string> TechnicalFilterColumns = new();
    public List<string> MovementActionDataColumns = new();
    public List<TelerikDropDownItem> MovementActionDataItems = new();
    public GetAllEnterActionsVm InfoAction;
    public MovementActionPrintDto PrintableAction = new();
    public GetActionTruckCrossVm ActionTruckCross = new();
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllStationsVm> Stations;
    //Action Type = 0 - Update april 2025
    public List<string> DynamicFieldRegisterDataColumns = new();
    public List<string> DynamicFieldForActionTypeDataColumns = new();
    public GetAllEnterActionsVm PrintData;
    public string UserId;
    public string CompanyName;

    [Parameter] public int TabMode { get; set; } = 0;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public ILogger<EnterActions> Logger { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal DetailsModal { get; set; }
    public Modal DetailsAggModal { get; set; }
    public Modal FiltersModal { get; set; }
    public Modal InfoModal { get; set; }
    public Modal ModalPrint { get; set; }
    public Modal ModalTruckCross { get; set; }

    public SelectPrintFormat SelectPrintFormatRef { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        Sizes = await FormalCache.GetSizes();

        Qcs = await FormalCache.GetQcs();

        Warehouses = await FormalCache.GetWarehouses();

        Groups = await FormalCache.GetGroups();   

        Brands =  await FormalCache.GetBrands();

        ProductTypes = await FormalCache.GetTypes();


        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
                        , new GetAllStationsVmContext())).Value;

        SearchWarehouses = Warehouses;

        TechnicalInfoDataKeys = (await Api.PostAsyncByUri<List<string>>("wms/Product", "SGetAllTechnicalInfoDataKeys")).Value;

        InitFilters();

        IsLoading = false;
    }

    #region Search Request
    public async Task OnSearchClick()
    {
        var filters = FillFilters();

        if (filters is null)
        {
            return;
        }

        if (TabMode == 2 || TabMode == 0)
        {
            Enters = (await Api.PostAsyncByContext<List<GetAllEnterActionsVm>>("SReportEnterAction",
            new GetAllEnterActionsVmContext(),
            new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;
        }

        if (TabMode == 1 || TabMode == 0)
        {
            Aggregates = (await Api.PostAsyncByContext<List<GetEnterActionAggregateOnProductCodeVm>>("SReportEnterActionTajamoeeProductCode",
                new GetEnterActionAggregateOnProductCodeVmContext(),
                new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;
        }

        if (TabMode == 3 || TabMode == 0)
        {
            FullDetails = (await Api.PostAsyncByContext<List<GetEnterActionDetailsFullVm>>("SReportEnterActionFull",
                new GetEnterActionDetailsFullVmContext(),
                new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;
        }

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnEnterActionDetailsClick(GetAllEnterActionsVm product)
    {
        IsLoading = true;

        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        ReportFilter operationCodeFilter = Filters.First(p => p.FieldName.Equals("OperationCode"));

        ReportFilter destinationFilter = Filters.First(p => p.FieldName.Equals("Destination"));

        ReportFilter actionTypeFilter = Filters.First(p => p.FieldName.Equals("ActionType"));

        operationCodeFilter.Values = new()
        {
            product.OpCode
        };

        destinationFilter.Values = new()
        {
            product.Destination
        };

        actionTypeFilter.Values = new()
        {
            product.ActionType?.ToString()
        };

        filters.RemoveAll(p => p.FieldName.Equals("OperationCode"));

        filters.RemoveAll(p => p.FieldName.Equals("Destination"));

        filters.RemoveAll(p => p.FieldName.Equals("ActionType"));

        filters.Add(operationCodeFilter);

        filters.Add(destinationFilter);

        filters.Add(actionTypeFilter);

        Details = (await Api.PostAsync<List<GetEnterActionDetailsVm>>("SReportEnterActionByOpCode"
            , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        await DetailsModal.Open(new());

        IsLoading = false;
    }

    public async Task OnEnterActionAggOnProductCodeDetailsClick(GetEnterActionAggregateOnProductCodeVm product)
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

        AggDetails = (await Api.PostAsync<List<GetEnterActionDetailsFullVm>>("SReportEnterActionFull"
            , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        await DetailsAggModal.Open(new());

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        Request = new();

        Enters = null;

        Aggregates = null;

        FullDetails = null;

        Details = null;

        ApplyFilters = new();

        CurrentActionType = 0;

        SearchWarehouses = Warehouses;
    }

    public async Task OnActionInfoClick(GetAllEnterActionsVm action)
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

            string data = JsonSerializer.Serialize(exo);

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
    #endregion

    #region Export
    public async Task OnClickExportEntersToPdfMaster()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        List<KeyValuePair<string, object>> variables = new()
        {
            new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
          , new("CompanyName", CompanyName)
          , new("PageTitle", PageTitle)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetAllEnterActionsVm), Enters)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "EnterAction",
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

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetEnterActionAggregateOnProductCodeVm), Aggregates)
        };

        List<KeyValuePair<string, object>> variables = new()
        {
            new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
          , new("CompanyName", CompanyName)
          , new("PageTitle", PageTitle)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "EnterActionAgg",
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


    public async Task OnSelectProductForPrint(GetAllEnterActionsVm product)
    {
        PrintData = product;

        if (SelectPrintFormatRef != null)
        {
            await SelectPrintFormatRef.ShowPrintFormatsAsync(new MouseEventArgs());
        }
    }

    public async Task OnPrintActionClick(GetPrintFormatsByPageTitleDto format)
    {
        IsLoading = true;

        MovementActionPrintDto printAction = new();

        printAction = Mapper.Map<MovementActionPrintDto>(PrintData);

        List<ReportFilter> filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        ReportFilter destinationFilter = Filters.First(p => p.FieldName.Equals("Destination"));

        ReportFilter actionTypeFilter = Filters.First(p => p.FieldName.Equals("ActionType"));

        ReportFilter operationCodeFilter = Filters.First(p => p.FieldName.Equals("OperationCode"));

        destinationFilter.Values = new()
        {
            PrintData.Destination
        };

        actionTypeFilter.Values = new()
        {
            PrintData.ActionType?.ToString()
        };

        operationCodeFilter.Values = new()
        {
            PrintData.OpCode
        };

        filters.RemoveAll(p => p.FieldName.Equals("Destination"));

        filters.RemoveAll(p => p.FieldName.Equals("ActionType"));

        filters.RemoveAll(p => p.FieldName.Equals("OperationCode"));

        filters.Add(destinationFilter);

        filters.Add(actionTypeFilter);

        filters.Add(operationCodeFilter);

        var serials =
            (await Api.PostAsyncByContext<List<TagMovementPrintDto>>("SReportEnterActionByOpCode"
                    , new TagMovementPrintDtoContext()
                    , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        var truckCrosses = (await Api.PostAsync<List<GetActionTruckCrossVm>>("SGetTruckCrossByMovementActionId"
        , new KeyValuePair<string, object>[] { new("movementActionId", printAction.OpCode) })).Value;

        if (truckCrosses.Any())
        {
            printAction.TruckCross = truckCrosses.First();
        }

        List<EnterActionPrintMainDto> printData = new();

        foreach (var serial in serials)
        {
            EnterActionPrintMainDto print = printData.FirstOrDefault(p => p.ProductCode.Equals(serial.ProductCode));

            if (print is null)
            {
                print = new()
                {
                    ProductCode = serial.ProductCode,
                    ProductName = serial.ProductName,
                    TechnicalCode = serial.TechnicalCode,
                    Count = 0,
                    SumCount = 0
                };
            }

            print.SumCount += serial.ProductCount;

            print.Count++;

            printData.ReplaceOrAdd(p => p.ProductCode.Equals(serial.ProductCode), print);
        }

        printAction.EnterPrints = printData;

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
                new(nameof(EnterActionPrintMainDto), printableAction.EnterPrints)
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

            await Export.ExportAndDownloadUsingBypass(response.Value.Result);
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
            await ChangeActionType(filter);
        }
    }

    public async Task OnRemoveFilterInComponentClick(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await ChangeActionType(null);
        }
    }

    public async Task OnChangeFilterValueInEditComponent(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await ChangeActionType(filter);
        }
    }

    #region Init Filters
    private void InitFilters()
    {
        #region Static Filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ActionType",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_ActionType,
            IsLikeCheckboxShown = false,
            Items = ActionTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code.Value.ToString()
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Destination",
            Type = FilterType.Static,
            Component = FilterComponent.Modal,
            Label = TextResources.APP_StringKeys_Warehouse,
            IsLikeCheckboxShown = false,
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
            EqualityType = FilterEqualityType.BiggerThan,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.PersianDate,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToDate",
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.SmallerThan,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.PersianDate,
            Label = TextResources.APP_StringKeys_ToDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Qc",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_Chart_Qc,
            IsLikeCheckboxShown = false,
            Items = Qcs.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Size",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_Product_Size,
            IsLikeCheckboxShown = false,
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
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Drop,
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
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Drop,
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
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Drop,
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
            Label = TextResources.APP_StringKeys_ProductType,
            IsLikeCheckboxShown = false,
            Items = ProductTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "RecordsCount",
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_RecordsCount,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Drop,
            Items = GetRecordsCounts().Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DocumentKey",
            IsLikeCheckboxShown = true,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_DocKey,
            Component = FilterComponent.Text
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
                Type = FilterType.TechnicalInfo,
                IsLikeCheckboxShown = true,
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
    private async Task ChangeActionType(ReportFilter actionType)
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

            foreach (var field in DynamicFields.Where(p => p.FieldType == DynamicFieldType.HeaderData))
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
                        FieldName = field.Title,
                        Value = field.DefaultValue,
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
                else if (field.ValueType == DynamicFieldValueType.Plaque)
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Plaque,
                        IsFilterShown = field != null ? (field.FieldShowColumn && (field.ActionType == 0)) : false,
                        IsLikeCheckboxShown = true,
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

            Request.Destination = string.Empty;

            SearchWarehouses = items;
        }
        else
        {
            SearchWarehouses = Warehouses;
        }

        Filters.FirstOrDefault(p => p.FieldName.Equals("Destination")).Items = SearchWarehouses.Select(p => new ReportDataItem()
        {
            Label = p.DestinationTitle,
            Value = p.DestinationCode
        }).ToList();
    }
    #endregion
    #endregion

    #region Action TruckCross
    public async Task OnTruckCrossClick(string opCode)
    {
        IsLoading = true;

        ActionTruckCross = (await Api.PostAsync<List<GetActionTruckCrossVm>>("SGetTruckCrossByMovementActionId"
                , new KeyValuePair<string, object>[] { new("movementActionId", opCode) })).Value.FirstOrDefault();

        await ModalTruckCross.Open(new());

        IsLoading = false;
    }
    #endregion

    #region private
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
                DynamicFieldRegisterDataColumns.Add(filter.Label);
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
