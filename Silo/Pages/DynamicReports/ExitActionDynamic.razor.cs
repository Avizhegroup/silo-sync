using AutoMapper;
using Silo.Application.Dto.Filter;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Telerik.DataSource.Extensions;
using Silo.Shared.Components.Report;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using Silo.Application;

namespace Silo.Pages.DynamicReports;
public partial class ExitActionDynamic
{
    private int FilterCount = 1;
    private int ColumnCount = 1;
    private int CalculationColumnCount = 1;
    private int PivotColumnCount = 1;
    private int CurrentActionType = 0;
    public bool IsLoading = true;
    public bool IsInitPageFinished = false;
    public string UserId;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllUsersVm> Users;
    public List<object> Results = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductBrandVm> ProductBrands;
    public List<GetAllProductGroupVm> ProductGroups;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllTruckTypesVm> TruckTypes;
    public List<GetAllTruckCrossPresentCauseVm> Causes;
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes;
    public List<GetAllTruckCrossShipmentVm> Shipments;
    public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations;
    public List<TelerikDropDownItem> RegisterDevices = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Kiosk,
            Value = "0"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Handheld,
            Value = ""
        }
    };
    public List<TelerikDropDownItem> InspectStatus = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Failed,
            Value = "0"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Accept,
            Value = "1"
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Inspect_Status_Not,
            Value = "2"
        }
    };
    public List<string> TechnicalInfoDataKeys = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> Filters = new();
    public List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> ApplyFilters = new();
    public List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> DataColumns;
    public List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> AddedDataColumns = new();
    public List<ReportCalculatingColumn<ExitActionDynamicReportColumnsType>> CalculatingColumns;
    public List<ReportCalculatingColumn<ExitActionDynamicReportColumnsType>> AddedCalculatingColumns = new();
    public List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> PivotColumns;
    public ReportColumnGeneric<ExitActionDynamicReportColumnsType> AddedPivotColumn;
    public List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> DataMiningElementColumns;
    public List<ReportColumnGeneric<ExitActionDynamicReportColumnsType>> AddedDataMiningElementColumns = new();
    public List<GetDataMiningElementIdsAndTitlesDto> DataMiningElements = new();
    public List<GetAllStationsVm> Stations;
    public List<TelerikDropDownItem> ChartDatas = new();
    public TelerikDropDownItem ChartKeyValue = new();
    public List<string> GridColumns = new() { "ردیف" };
    public List<string> PivotColumnTitles = new();
    public Dictionary<string, decimal> ColumnAgg = new();
    public decimal TotalSum = 0;
    public GetReportFormatByIdVm Format;
    public new bool mustTitleSetAutomatically = true;

    public Modal ModalColumns { get; set; }

    public ReportAllSection<ExitActionDynamicReportColumnsType, ExitActionDynamicReportFilterType> ReportAllSectionRef { get; set; }

    [Parameter] public int? FormatId { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<GetAllUsersVm>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        Shifts = await FormalCache.GetShifts();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Lines = await FormalCache.GetLines();

        ProductTypes = await FormalCache.GetTypes();

        ProductBrands = await FormalCache.GetBrands();

        ProductGroups = await FormalCache.GetGroups();

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        TruckTypes = (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value;

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

        OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetAllTruckCrossOperationType")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        OperationDestinations = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value;

        Warehouses = await FormalCache.GetWarehouses();

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
                , new GetAllStationsVmContext())).Value;

        SearchWarehouses = Warehouses;

        TechnicalInfoDataKeys = (await Api.PostAsyncByUri<List<string>>("wms/Product", "SGetAllTechnicalInfoDataKeys")).Value;

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetAllDynamicFields")).Value;

        DataMiningElements = (await Api.PostAsync<GetDataMiningElementIdsAndTitlesVm>("SGetDataMiningElementIdsAndTitles"
            , new KeyValuePair<string, object>[]
            {
                new("request", new GetDataMiningElementIdsAndTitlesQuery { UsageType = 1})
            })).Value.Elements;

        InitColumnsAndFilters();

        if (FormatId is not null)
        {
            await LoadFormat();
        }

        IsInitPageFinished = true;

        IsLoading = false;
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (ApplyFilters.Neither(p => p.FieldName == "ActionType"))
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required,
                                            TextResources.APP_StringKeys_ActionType), "error");

            return;
        }

        if (!AddedDataColumns.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Dynamic_Column, "error");

            return;
        }

        IsLoading = true;

        TotalSum = 0;

        var filters = AggregateFilterValues();

        var tempResult = (await Api.PostAsyncByUri<List<object>>("wms/report"
            , "SReportExitAction"
            , new("filters", filters)
            , new("selectColumns", AddedDataColumns)
            , new("calculating", AddedCalculatingColumns)
            , new("pivot", AddedPivotColumn)
            , new("dataMiningElements", AddedDataMiningElementColumns)
            )).Value;

        Results = AddRowNumbersToResults(tempResult);

        if (Results.Any())
        {
            AddPivotDataColumns(Results);
        }

        if (AddedPivotColumn is not null)
        {
            TotalSum = ColumnAgg.Sum(p => p.Value);
        }

        IsLoading = false;

        IsFiltersShown = false;
    }

    /// <summary>
    /// Add row number to data of Result
    /// </summary>
    private List<object> AddRowNumbersToResults(List<object> results)
    {
        var newResults = new List<object>();

        JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };

        for (int i = 0; i < results.Count; i++)
        {
            var item = (JsonElement)results[i];
            var jsonObject = JsonNode.Parse(item.GetRawText())!.AsObject();

            jsonObject["ردیف"] = i + 1;

            byte[] updatedJson = JsonSerializer.SerializeToUtf8Bytes(jsonObject, options);

            JsonElement updatedElement = JsonSerializer.Deserialize<JsonElement>(updatedJson);

            newResults.Add(updatedElement);
        }

        return newResults;
    }

    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        await ExcelExporter.ExportJsonData(PageTitle, Results, GridColumns);
    }

    #region Filters
    public async Task OnFilterRemoveClick(ReportFilterGeneric<ExitActionDynamicReportFilterType> filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            CurrentActionType = 0;

            await OnActionTypeChange(null);
        }

        ApplyFilters.RemoveAll(p => p.FieldName.Equals(filter.FieldName) && p.Value.Equals(filter.Value));
    }

    public async Task OnAddNewFilter(ReportFilterGeneric<ExitActionDynamicReportFilterType> filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            if (CurrentActionType != 0 && ApplyFilters.Any(p => p.FieldName.Equals("ActionType")))
            {
                await ReportAllSectionRef.ModalFilters.Close(new());

                Notification.Show(TextResources.APP_StringKeys_ActionType_OnlyOne_Validation, "error");

                Notification.Show(TextResources.APP_StringKeys_ActionType_HasChosen_Validation, "error");

                return;
            }
            else
            {
                CurrentActionType = int.Parse(filter.Value);

                await OnActionTypeChange(filter);

            }
        }

        ApplyFilters.Add(filter);
    }
    #endregion

    #region Columns
    public async Task OnCalculatingColumnAdd(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<ExitActionDynamicReportColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.Add(new()
        {
            GroupColumnType = calCol.GroupColumnType,
            Id = calCol.Id,
            Title = calCol.Title,
            Type = calCol.Type
        });
    }

    public async Task OnDataColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<ExitActionDynamicReportColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.Add(col);
    }
    public async Task OnDataMiningElementColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<ExitActionDynamicReportColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataMiningElementColumns.Add(col);
    }

    public async Task OnPivotColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<ExitActionDynamicReportColumnsType>)PivotColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedPivotColumn = col;
    }

    public async Task OnDataColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<ExitActionDynamicReportColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.RemoveAll(p => p.Id == col.Id);
    }
    public async Task OnDataMiningElementColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<ExitActionDynamicReportColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataMiningElementColumns.RemoveAll(p => p.Id == col.Id);
    }

    public async Task OnCalculatingColumnRemove(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<ExitActionDynamicReportColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.RemoveAll(p => p.Id == calCol.Id);
    }

    public async Task OnPivotColumnRemove()
    {
        AddedPivotColumn = null;
    }
    #endregion

    #region Privates
    private void InitStaticFilters()
    {
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ActionType",
            FieldType = ExitActionDynamicReportFilterType.ActionType,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.Equals,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ActionType,
            Items = ActionTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code.Value.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "StoreCode",
            FieldType = ExitActionDynamicReportFilterType.StoreCode,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.Equals,
            Component = FilterComponent.Modal,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_Warehouse,
            Items = SearchWarehouses.Select(p => new ReportDataItem()
            {
                Label = p.DestinationTitle,
                Value = p.DestinationCode
            }).ToList(),
            IsFilterShown = true,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromDate",
            FieldType = ExitActionDynamicReportFilterType.FromDate,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.BiggerThan,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_FromDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToDate",
            FieldType = ExitActionDynamicReportFilterType.ToDate,
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ToDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Size",
            FieldType = ExitActionDynamicReportFilterType.Size,
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Product_Size,
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductCode",
            FieldType = ExitActionDynamicReportFilterType.ProductCode,
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ProductCode,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TechnicalCode",
            FieldType = ExitActionDynamicReportFilterType.TechnicalCode,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Chart_Regcode,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "OperationCode",
            FieldType = ExitActionDynamicReportFilterType.OperationCode,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_OperationCode,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "GateOpCode",
            FieldType = ExitActionDynamicReportFilterType.GateOpCode,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_GateOpCode,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "GateCode",
            FieldType = ExitActionDynamicReportFilterType.GateCode,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_Station,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = Stations.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductGroup",
            FieldType = ExitActionDynamicReportFilterType.ProductGroup,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_ProductGroup,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = ProductGroups.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }

        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductBrand",
            FieldType = ExitActionDynamicReportFilterType.ProductBrand,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_ProductBrand,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = ProductBrands.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductType",
            FieldType = ExitActionDynamicReportFilterType.ProductType,

            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ProductType,
            Items = ProductTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DocumentKey",
            FieldType = ExitActionDynamicReportFilterType.DocumentKey,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_DocKey,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "RecordsCount",
            FieldType = ExitActionDynamicReportFilterType.RecordsCount,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_RecordsCount,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = GetRecordsCounts().Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckType",
            FieldType = ExitActionDynamicReportFilterType.TruckType,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_TypeTruck,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = TruckTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "CarPlaque",
            FieldType = ExitActionDynamicReportFilterType.CarPlaque,
            IsFilterShown = true,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "NationalCode",
            FieldType = ExitActionDynamicReportFilterType.NationalCode,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_NationalCode + " " + TextResources.APP_StringKeys_Driver + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DriverName",
            FieldType = ExitActionDynamicReportFilterType.DriverName,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_DriverName + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossCause",
            FieldType = ExitActionDynamicReportFilterType.TruckCrossCause,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Present_Cause + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = Causes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossOperationType",
            FieldType = ExitActionDynamicReportFilterType.TruckCrossOperationType,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Type + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = OperationTypes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossShipment",
            FieldType = ExitActionDynamicReportFilterType.TruckCrossShipment,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Shipment + " " + TextResources.APP_StringKeys_TruckCross,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = Shipments.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "TruckCrossOperationDestination",
            FieldType = ExitActionDynamicReportFilterType.TruckCrossOperationDestination,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Destination + " " + TextResources.APP_StringKeys_TruckCross,
            Items = OperationDestinations.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Line",
            FieldType = ExitActionDynamicReportFilterType.Line,
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Line,
            Items = Lines.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Shift",
            FieldType = ExitActionDynamicReportFilterType.Shift,
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Chart_Shift,
            Items = Shifts.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "RegisterUser",
            FieldType = ExitActionDynamicReportFilterType.RegisterUser,
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Register_User,
            Items = Users.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Username
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromRegisterDate",
            FieldType = ExitActionDynamicReportFilterType.FromRegisterDate,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.BiggerThan,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_FromRegisterDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToRegisterDate",
            FieldType = ExitActionDynamicReportFilterType.ToRegisterDate,
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ToRegisterDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromRegisterTime",
            FieldType = ExitActionDynamicReportFilterType.FromRegisterTime,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.BiggerThan,
            Component = FilterComponent.Time,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_FromRegisterTime,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToRegisterTime",
            FieldType = ExitActionDynamicReportFilterType.ToRegisterTime,
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            Component = FilterComponent.Time,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ToRegisterTime,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ExitActionUser",
            FieldType = ExitActionDynamicReportFilterType.ExitActionUser,
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_TruckCross_Exit_User,
            Items = Users.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId",(FilterCount-1).ToString()}
            }
        });
    }

    private void InitTechnicalFilters()
    {
        foreach (var field in TechnicalInfoDataKeys)
        {
            Filters.Add(new()
            {
                Id = FilterCount++,
                Label = field,
                FieldName = field,
                IsLikeCheckboxShown = true,
                IsFilterShown = true,
                Type = FilterType.TechnicalInfo,
                Component = FilterComponent.Text,
                AdditionalData = new()
                {
                    { "FilterType", "Technical"},
                    { "FilterId", field}
                }
            });
        }
    }

    private void InitDynamicFilters()
    {
        foreach (var field in DynamicFields.Where(p => p.FieldType != DynamicFieldType.ItemData))
        {
            if (field.ValueType == DynamicFieldValueType.TextBox)
            {
                try
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Text,
                        IsFilterShown = field != null ? (field.ActionType == 0 && field.FieldShowColumn) : false,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                    }
                    });
                }
                catch (Exception ex)
                {

                }
            }

            else if (field.ValueType == DynamicFieldValueType.DropDown)
            {
                try
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        Component = FilterComponent.Drop,
                        Type = FilterType.Dynamic,
                        IsLikeCheckboxShown = false,
                        IsFilterShown = field != null ? (field.ActionType == 0 && field.FieldShowColumn) : false,
                        FieldName = field.Title,
                        AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                    },
                        Items = field.ValueOptionList.Select(p => new ReportDataItem()
                        {
                            Label = p,
                            Value = p
                        }).ToList()
                    });
                }
                catch (Exception ex)
                {

                }
            }
            else if (field.ValueType == DynamicFieldValueType.RichTextEditor)
            {
                try
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.RichTextEditor,
                        IsFilterShown = field != null ? (field.ActionType == 0 && field.FieldShowColumn) : false,
                        IsLikeCheckboxShown = true,
                        AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                    }
                    });
                }
                catch (Exception ex)
                {

                }
            }
            else if (field.ValueType == DynamicFieldValueType.Numeric)
            {
                try
                {
                    Filters.Add(new()
                    {
                        Id = FilterCount++,
                        Label = field.Title,
                        FieldName = field.Title,
                        Type = FilterType.Dynamic,
                        Component = FilterComponent.Numeric,
                        IsFilterShown = field != null ? (field.ActionType == 0 && field.FieldShowColumn) : false,
                        IsLikeCheckboxShown = false,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            { "FilterType", "Dynamic"},
                            { "FilterId", field.Id.ToString()},
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                    });
                }
                catch (Exception ex)
                {

                }
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
                            { "FilterType", "Dynamic"},
                            { "FilterId", field.Id.ToString()},
                            { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                        }
                });
            }
        }
    }

    private void InitCalculatingColumns()
    {
        CalculatingColumns = new()
        {
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Max_Count,
                Type = ReportCalculatingColumnType.Max,
                GroupColumnType = ExitActionDynamicReportColumnsType.ProductCount
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Min_Count,
                Type = ReportCalculatingColumnType.Min,
                GroupColumnType = ExitActionDynamicReportColumnsType.ProductCount
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Average,
                Type = ReportCalculatingColumnType.Avg,
                GroupColumnType = ExitActionDynamicReportColumnsType.ProductCount
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Count,
                Type = ReportCalculatingColumnType.Count,
                GroupColumnType = ExitActionDynamicReportColumnsType.ProductSerial
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_SumValue,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = ExitActionDynamicReportColumnsType.SumCount
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Sum_ProductCountInPack,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = ExitActionDynamicReportColumnsType.ProductCountInPack
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Percent,
                Type = ReportCalculatingColumnType.Percent,
                GroupColumnType = ExitActionDynamicReportColumnsType.SumCount
            },
            new ReportCalculatingColumn<ExitActionDynamicReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Count + " " + TextResources.APP_StringKeys_Operation,
                Type = ReportCalculatingColumnType.Count,
                GroupColumnType = ExitActionDynamicReportColumnsType.OperationCode
            }
        };
    }

    private void InitSelectColumns()
    {
        DataColumns = new()
        {
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Operation_DocumentCode,
                Type = ExitActionDynamicReportColumnsType.DocumentCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_OperationCode,
                Type = ExitActionDynamicReportColumnsType.OperationCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Operation_Time,
                Type = ExitActionDynamicReportColumnsType.OperationTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Full +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.PersianDateFull,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Year +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.PersianDateYear,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Month +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.PersianDateMonth,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Day +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.PersianDateDay,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Full +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.GregorianDateFull,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Year +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.GregorianDateYear,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Month +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.GregorianDateMonth,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Day +
                " " + TextResources.APP_StringKeys_Operation,
                Type = ExitActionDynamicReportColumnsType.GregorianDateDay,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Station,
                Type = ExitActionDynamicReportColumnsType.StationName,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Product_Serial,
                Type = ExitActionDynamicReportColumnsType.ProductSerial,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductCode,
                Type = ExitActionDynamicReportColumnsType.ProductCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Chart_Regcode,
                Type = ExitActionDynamicReportColumnsType.Regcode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTitle,
                Type = ExitActionDynamicReportColumnsType.ProductName,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Qc_Code,
                Type = ExitActionDynamicReportColumnsType.QcCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Qc_Title,
                Type = ExitActionDynamicReportColumnsType.QcTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTypeCode,
                Type = ExitActionDynamicReportColumnsType.TypeCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTypeTitle,
                Type = ExitActionDynamicReportColumnsType.TypeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Size_Code,
                Type = ExitActionDynamicReportColumnsType.SizeCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Size_Title,
                Type = ExitActionDynamicReportColumnsType.SizeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Code,
                Type = ExitActionDynamicReportColumnsType.BrandCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Title,
                Type = ExitActionDynamicReportColumnsType.BrandTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Code,
                Type = ExitActionDynamicReportColumnsType.GroupCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Title,
                Type = ExitActionDynamicReportColumnsType.GroupTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_SubGroup_Code,
                Type = ExitActionDynamicReportColumnsType.SubGroupCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_SubGroup_Title,
                Type = ExitActionDynamicReportColumnsType.SubGroupTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Class_Code,
                Type = ExitActionDynamicReportColumnsType.ClassCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Class_Title,
                Type = ExitActionDynamicReportColumnsType.ClassTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Code_Salon,
                Type = ExitActionDynamicReportColumnsType.LineCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon,
                Type = ExitActionDynamicReportColumnsType.LineTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Shift_Code,
                Type = ExitActionDynamicReportColumnsType.ShiftCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Register_User,
                Type = ExitActionDynamicReportColumnsType.RegisterUser,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_RegisterDate,
                Type = ExitActionDynamicReportColumnsType.RegisterDate,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_RegisterTime,
                Type = ExitActionDynamicReportColumnsType.RegisterTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Exit_User,
                Type = ExitActionDynamicReportColumnsType.ExtiActionUser,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            }
        };
    }

    private void InitDynamicColumns()
    {
        foreach (var field in DynamicFields.Where(p => p.FieldType != DynamicFieldType.ItemData))
        {
            DataColumns.Add(new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = field.Title,
                Type = ExitActionDynamicReportColumnsType.DynamicFields,
                IsColumnShown = field != null ? (field.ActionType == 0 && field.FieldShowColumn) : false,
                AdditionalData = new Dictionary<string, string>()
                {
                    { "ColumnType", "Dynamic"},
                    { "ColumnId", field.Id.ToString()},
                    { "DynamicColumnActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                }
            });
        }
    }

    private void InitPivotColumns()
    {
        PivotColumns = new()
        {
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Size_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.SizeTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.BrandTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.GroupTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTypeTitle + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.TypeTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Qc_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.QcTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_SubGroup_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.SubGroupTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Class_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.ClassTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.LineTitle
            },
            new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Shift_Title + "(Pivot)",
                Type = ExitActionDynamicReportColumnsType.ShiftTitle
            }
        };
    }

    private void InitDataMiningElementColumns()
    {
        DataMiningElementColumns = new();

        foreach (var element in DataMiningElements)
        {
            DataMiningElementColumns.Add(new ReportColumnGeneric<ExitActionDynamicReportColumnsType>
            {
                Id = ColumnCount++,
                Title = element.Title,
                Type = ExitActionDynamicReportColumnsType.DataMiningElements,
                IsColumnShown = true,
                Value = element.Id.ToString(),
                AdditionalData = new Dictionary<string, string>()
                {
                    { "ColumnType", "DataMiningElement"},
                    { "ColumnId", element.Id.ToString()},
                }
            });
        }
    }

    private void AddPivotDataColumns(List<object> data)
    {
        GridColumns.Clear();

        PivotColumnTitles.Clear();

        ColumnAgg.Clear();

        var elementFirst = (JsonElement)data.First();

        var jobject = JObject.Parse(elementFirst.GetRawText());


        var names = jobject.Root.Cast<JProperty>()
                                .Select(x => x.Name);

        GridColumns.Add("ردیف");

        foreach (var item in AddedDataColumns)
        {
            if (!GridColumns.Any(p => p.Equals(item.Title)))
            {
                GridColumns.Add(item.Title);
            }
        }

        foreach (var item in AddedCalculatingColumns)
        {
            if (!GridColumns.Any(p => p.Equals(item.Title)))
            {
                GridColumns.Add(item.Title);

                ColumnAgg.Add(item.Title, 0);
            }
        }

        foreach (var item in names)
        {
            if (!GridColumns.Any(p => p.Equals(item)))
            {
                GridColumns.Add(item);

                ColumnAgg.Add(item, 0);

                PivotColumnTitles.Add(item);
            }
        }

        foreach (var item in data)
        {
            var element = (JsonElement)item;

            foreach (var column in ColumnAgg.Keys)
            {
                if (ColumnAgg.ContainsKey(column))
                {
                    if (element.TryGetProperty(column, out var value))
                    {
                        string valueString = value.ToString().Replace("{}", "0");

                        if (string.IsNullOrEmpty(valueString))
                        {
                            valueString = "0";
                        }

                        if (decimal.TryParse(valueString, out decimal result))
                        {
                            ColumnAgg[column] += result;
                        }
                    }
                }
            }
        }
    }

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

    /// <summary>
    /// This method Set Warehouse list and dynamic filters based on chosen actionType.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task OnActionTypeChange(ReportFilter actionType)
    {
        int actionTypeId = actionType is null ? 0 : int.Parse(actionType.Value);

        IsLoading = true;

        SetWarehousesByActionType(actionTypeId);

        await SetDynamicFiltersAndColumnsByActionType(new() { 1, actionTypeId });

        IsLoading = false;

        StateHasChanged();
    }

    private async Task SetDynamicFiltersAndColumnsByActionType(List<int> actionTypeIds)
    {
        if (actionTypeIds.Any())
        {
            HiddenAllDynamicColumns();

            HiddenAllDynamicFilters();

            foreach (var actionTypeId in actionTypeIds)
            {
                ShowDynamicFiltersByActionType(actionTypeId);

                ShowDynamicColumnsByActionType(actionTypeId);
            }

            Filters = Filters.OrderBy(p => p.Type).ToList();
        }
        else
        {
            ShowDynamicFiltersByActionType(0);

            ShowDynamicColumnsByActionType(0);
        }
    }

    private void HiddenAllDynamicColumns()
    {
        foreach (var column in DataColumns)
        {
            if (column.AdditionalData.TryGetValue("ColumnType", out var columnType) && columnType == "Dynamic")
            {
                if (column.AdditionalData.TryGetValue("DynamicColumnActionType", out var actionType) && actionType == "0")
                {
                    var field = DynamicFields.FirstOrDefault(f => f.Id.ToString() == column.AdditionalData["ColumnId"]);
                    if (field is not null && field.FieldShowColumn)
                    {
                        continue;
                    }
                }

                column.IsColumnShown = false;
            }
        }
    }

    private void HiddenAllDynamicFilters()
    {
        foreach (var filter in Filters)
        {
            if (filter.AdditionalData.TryGetValue("FilterType", out var filterType) && filterType == "Dynamic")
            {
                if (filter.AdditionalData.TryGetValue("DynamicFilterActionType", out var actionType) && actionType == "0")
                {
                    var field = DynamicFields.FirstOrDefault(f => f.Id.ToString() == filter.AdditionalData["FilterId"]);
                    if (field is not null && field.FieldShowColumn)
                    {
                        continue;
                    }
                }

                filter.IsFilterShown = false;
            }
        }
    }

    private void ShowDynamicColumnsByActionType(int actionTypeId)
    {
        foreach (var column in DataColumns)
        {
            if (column.AdditionalData.TryGetValue("ColumnType", out var columnType) && columnType == "Dynamic")
            {
                if (column.AdditionalData.TryGetValue("DynamicColumnActionType", out var dynamicColumnActionType)
                    && dynamicColumnActionType == actionTypeId.ToString())
                {
                    var field = DynamicFields.FirstOrDefault(f => f.Id.ToString() == column.AdditionalData["ColumnId"]);

                    if (field is null)
                    {
                        continue;
                    }

                    if ((field.ActionType == 0 && field.FieldShowColumn) 
                        || (field.ActionType != 0 && field.FieldShowColumnForAction))
                    {
                        column.IsColumnShown = true;
                    }

                    if (field is not null && field.FieldShowColumn)
                    {
                        column.IsColumnShown = false;
                    }
                }
            }
        }
    }

    private void ShowDynamicFiltersByActionType(int actionTypeId)
    {
        foreach (var filter in Filters)
        {
            if (filter.AdditionalData.TryGetValue("FilterType", out var filterType) && filterType == "Dynamic")
            {
                if (filter.AdditionalData.TryGetValue("DynamicFilterActionType", out var dynamicFilterActionType)
                    && dynamicFilterActionType == actionTypeId.ToString())
                {
                    var field = DynamicFields.FirstOrDefault(f => f.Id.ToString() == filter.AdditionalData["FilterId"]);

                    if (field is null)
                    {
                        continue;
                    }

                    if ((field.ActionType == 0 && field.FieldShowColumn)
                        || (field.ActionType != 0 && field.FieldShowColumnForAction))
                    {
                        filter.IsFilterShown = true;
                    }

                    if (field is not null && field.FieldShowColumn)
                    {
                        filter.IsFilterShown = false;
                    }
                }
            }
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

    private List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> AggregateFilterValues()
    {
        List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> filters = new();

        filters = ApplyFilters.GroupBy(p => p.FieldName)
                              .Select(p => new
                                ReportFilterGeneric<ExitActionDynamicReportFilterType>()
                              {
                                  FieldName = p.Key,
                                  Type = p.First().Type,
                                  Component = p.First().Component,
                                  EqualityType = p.First().EqualityType,
                                  AddType = p.First().AddType,
                                  FieldType = p.First().FieldType,
                                  AdditionalData = p.First().AdditionalData,
                                  Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                              }).ToList();

        return filters;
    }

    private async Task LoadFormat()
    {
        Format = (await Api.PostAsyncByUriAndContext<GetReportFormatByIdVm>("wms/ReportFormat"
                          , "SGetReportFormatById"
                          , new GetReportFormatByIdVmContext()
                          , new KeyValuePair<string, object>("query", new GetReportFormatByIdQuery()
                          {
                              FormatId = (int)FormatId
                          }))).Value;

        foreach (var detail in Format.DetailsList)
        {
            switch (detail.DetailType)
            {
                case ReportFormatDetailTypes.Data:
                    {
                        if (detail.AdditionalData.TryGetValue("ColumnId", out var detailColumnId) &&
                            detail.AdditionalData.TryGetValue("ColumnType", out var detailColumnType))
                        {
                            var column = DataColumns.FirstOrDefault(p =>
                                p.AdditionalData.TryGetValue("ColumnId", out var columnId) &&
                                columnId == detailColumnId &&
                                p.AdditionalData.TryGetValue("ColumnType", out var columnType) &&
                                columnType == detailColumnType);

                            if (column is not null)
                            {
                                if (!GridColumns.Any(p => p.Equals(column.Title)))
                                {
                                    column.SortType = detail.SortType;

                                    column.AggType = detail.AggType;

                                    GridColumns.Add(column.Title);

                                    AddedDataColumns.Add(column);
                                }
                            }
                        }
                        break;
                    }

                case ReportFormatDetailTypes.Calculating:
                    {
                        var column = CalculatingColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                        AddedCalculatingColumns.Add(column);

                        GridColumns.Add(column.Title);
                        break;
                    }

                case ReportFormatDetailTypes.Pivot:
                    {
                        var column = DataColumns.FirstOrDefault(p => p.Id == int.Parse(detail.Id));

                        AddedPivotColumn = column;
                        break;
                    }

                case ReportFormatDetailTypes.Filter:
                    {
                        if (detail.AdditionalData.TryGetValue("FilterId", out var detailFilterId) &&
                           detail.AdditionalData.TryGetValue("FilterType", out var detailFilterType))
                        {
                            var filter = Filters.FirstOrDefault(p =>
                                p.AdditionalData.TryGetValue("FilterId", out var filterId) &&
                                filterId == detailFilterId &&
                                p.AdditionalData.TryGetValue("FilterType", out var filterType) &&
                                filterType == detailFilterType);

                            if (filter is not null && !ApplyFilters.Any(p =>
                                p.AdditionalData.TryGetValue("FilterId", out var filterId) &&
                                filterId == detailFilterId &&
                                p.AdditionalData.TryGetValue("FilterType", out var filterType) &&
                                filterType == detailFilterType))
                            {
                                filter.IsEditable = false;

                                filter.Value = detail.Value;

                                ApplyFilters.Add(filter);
                            }
                        }
                        break;
                    }
            }
        }

        await OnSearchClick(new());
    }

    /// <summary>
    /// Load all columns and filters before format.
    /// Loading of format is depend on columns and filters. So must loads before format.
    /// </summary>
    private void InitColumnsAndFilters()
    {
        InitStaticFilters();

        InitTechnicalFilters();

        InitDynamicFilters();

        InitCalculatingColumns();

        InitSelectColumns();

        InitDynamicColumns();

        InitPivotColumns();

        InitDataMiningElementColumns();
    }
    #endregion
}
