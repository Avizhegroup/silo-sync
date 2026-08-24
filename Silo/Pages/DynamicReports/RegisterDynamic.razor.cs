using AutoMapper;
using Silo.Application.Dto.Filter;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Telerik.DataSource.Extensions;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using Silo.Application;

namespace Silo.Pages.DynamicReports;
public partial class RegisterDynamic
{
    private int FilterCount = 1;
    private int ColumnCount = 1;
    private int CalculationColumnCount = 1;
    private int PivotColumnCount = 1;
    public bool IsLoading = true;
    public bool IsInitPageFinished = false;
    public string UserId;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public List<AddAccountCommand> Users;
    public List<object> Results = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductBrandVm> ProductBrands;
    public List<GetAllProductGroupVm> ProductGroups;
    public List<GetAllProductSubGroupVm> ProductSubGroups;
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
    public List<ReportFilterGeneric<RegisterReportDynamicFilterType>> Filters;
    public List<ReportFilterGeneric<RegisterReportDynamicFilterType>> ApplyFilters = new();
    public List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> DataColumns;
    public List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> AddedDataColumns = new();
    public List<ReportCalculatingColumn<RegisterReportDynamicColumnsType>> CalculatingColumns;
    public List<ReportCalculatingColumn<RegisterReportDynamicColumnsType>> AddedCalculatingColumns = new();
    public List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> PivotColumns;
    public ReportColumnGeneric<RegisterReportDynamicColumnsType> AddedPivotColumn;
    public List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> DataMiningElementColumns;
    public List<ReportColumnGeneric<RegisterReportDynamicColumnsType>> AddedDataMiningElementColumns = new();
    public List<GetDataMiningElementIdsAndTitlesDto> DataMiningElements = new();
    public GetReportFormatByIdVm Format;
    public List<string> GridColumns = new() { "ردیف"};
    public List<string> PivotColumnTitles = new();
    public Dictionary<string, decimal> ColumnAgg = new();
    public decimal TotalSum = 0;
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<GetAllWarehousesVm> Warehouses;

    public Modal ModalFilters { get; set; }
    public Modal ModalColumns { get; set; }

    [Parameter] public int? FormatId { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", 0))).Value;

        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser")).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<AddAccountCommand>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        Shifts = await FormalCache.GetShifts();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Lines =   await FormalCache.GetLines();
        ProductBrands = await FormalCache.GetBrands();

        ProductGroups = await FormalCache.GetGroups();

        ProductSubGroups = await FormalCache.GetSubGroups();

        Warehouses = await FormalCache.GetWarehouses();

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
        if (!AddedDataColumns.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Dynamic_Column, "error");

            return;
        }

        IsLoading = true;

        TotalSum = 0;

        var filters = AggregateFilterValues();

        var resultTemp  = (await Api.PostAsyncByUri<List<object>>("wms/report"
            , "SRepRegisterTagSummary"
            , new("filters", filters)
            , new("selectColumns", AddedDataColumns)
            , new("calculating", AddedCalculatingColumns)
            , new("pivot", AddedPivotColumn)
            , new("dataMiningElements", AddedDataMiningElementColumns)
            )).Value;

       Results =  AddRowNumbersToResults(resultTemp);

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
    

    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        await ExcelExporter.ExportJsonData(PageTitle, Results, GridColumns);
    }

    #region Filters
    public async Task OnAddNewFilter(ReportFilterGeneric<RegisterReportDynamicFilterType> filter)
    {
        ApplyFilters.Add(filter);
    }

    public async Task OnFilterRemoveClick(ReportFilterGeneric<RegisterReportDynamicFilterType> filter)
    {
        ApplyFilters.RemoveAll(p => p.FieldName.Equals(filter.FieldName) && p.Value.Equals(filter.Value));
    }
    #endregion

    #region Columns
    public async Task OnDataColumnAdd(ReportColumnGeneric<RegisterReportDynamicColumnsType> column)
    {
        var col = DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.Add(col);
    }

    public async Task OnCalculatingColumnAdd(ReportCalculatingColumn<RegisterReportDynamicColumnsType> column)
    {
        var calCol = CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.Add(new()
        {
            GroupColumnType = calCol.GroupColumnType,
            Id = calCol.Id,
            Title = calCol.Title,
            Type = calCol.Type,
            FieldName = calCol.FieldName
        });
    }

    public async Task OnPivotColumnAdd(ReportColumnGeneric<RegisterReportDynamicColumnsType> column)
    {
        var col = PivotColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedPivotColumn = col;
    }

    public async Task OnDataColumnRemove(ReportColumnGeneric<RegisterReportDynamicColumnsType> column)
    {
        var col = DataColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataColumns.RemoveAll(p => p.Id == col.Id);
    }

    public async Task OnCalculatingColumnRemove(ReportCalculatingColumn<RegisterReportDynamicColumnsType> column)
    {
        var calCol = CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedCalculatingColumns.RemoveAll(p => p.Id == calCol.Id);
    }

    public async Task OnPivotColumnRemove()
    {
        AddedPivotColumn = null;
    }

    public async Task OnDataMiningElementColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<RegisterReportDynamicColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataMiningElementColumns.Add(col);
    }

    public async Task OnDataMiningElementColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<RegisterReportDynamicColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);

        AddedDataMiningElementColumns.RemoveAll(p => p.Id == col.Id);
    }
    #endregion

    #region Privates
    private void InitColumnsAndFilters()
    {
        InitStaticFilters();

        InitDynamicFilters();

        AddCalculatingColumns();

        AddSelectColumns();

        InitDynamicColumns();

        AddPivotColumns();

        InitDataMiningElementColumns();

        AddDynamicCalculatingColumns();
    }

    private void AddDynamicCalculatingColumns()
    {
        var numericDynamicFields = DynamicFields.Where(p =>
            p.ActionType == 0 &&
            p.ValueType == DynamicFieldValueType.Numeric).ToList();

        foreach (var field in numericDynamicFields)
        {
            CalculatingColumns.Add(new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = $"مجموع {field.Title}", 
                Type = ReportCalculatingColumnType.Sum, 
                GroupColumnType = RegisterReportDynamicColumnsType.DynamicFields,
                FieldName = field.Title
            });

        }
    }

    private void InitStaticFilters()
    {
        Filters = new()
        {
            new()
            {
                Id =FilterCount++,
                Component = FilterComponent.ProductCodeModal,
                FieldName = "ProductCode",
                Label = TextResources.APP_StringKeys_ProductCode,
                Type = FilterType.Static,
                FieldType = RegisterReportDynamicFilterType.ProductCode,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id =FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Qc",
                Label = TextResources.APP_StringKeys_ProductStatus,
                Type = FilterType.Static,
                Items = Qcs.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.Qc,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "User",
                Label = TextResources.APP_StringKeys_User,
                Type = FilterType.Static,
                Items = Users.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.UserName,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.User,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Shift",
                Label = TextResources.APP_StringKeys_Chart_Shift,
                Type = FilterType.Static,
                Items = Shifts.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.Shift,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Size",
                Label = TextResources.APP_StringKeys_Product_Size,
                Type = FilterType.Static,
                Items = Sizes.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.Size,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Text,
                FieldName = "TechnicalCode",
                Label = TextResources.APP_StringKeys_Chart_Regcode,
                Type = FilterType.Static,
                FieldType = RegisterReportDynamicFilterType.TechnicalCode,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Text,
                FieldName = "ProductSerial",
                Label = TextResources.APP_StringKeys_ProductSerial,
                Type = FilterType.Static,
                FieldType = RegisterReportDynamicFilterType.ProductSerial,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.PersianDate,
                FieldName = "FromDate",
                EqualityType = FilterEqualityType.BiggerThan,
                Label = TextResources.APP_StringKeys_FromDate,
                Type = FilterType.Static,
                FieldType = RegisterReportDynamicFilterType.FromDate,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.PersianDate,
                FieldName = "ToDate",
                EqualityType = FilterEqualityType.SmallerThan,
                Label = TextResources.APP_StringKeys_ToDate,
                Type = FilterType.Static,
                FieldType = RegisterReportDynamicFilterType.ToDate,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "ProductBrand",
                Label = TextResources.APP_StringKeys_ProductBrand,
                Type = FilterType.Static,
                Items = ProductBrands.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.ProductBrand,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "ProductGroup",
                Label = TextResources.APP_StringKeys_ProductGroup,
                Type = FilterType.Static,
                Items = ProductGroups.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.ProductGroup,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "Line",
                Label = TextResources.APP_StringKeys_Line,
                Type = FilterType.Static,
                Items = Lines.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.Line,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount ++,
                Component = FilterComponent.Drop,
                FieldName = "RegisterDevice",
                Label = TextResources.APP_StringKeys_RegisterDevice,
                Type = FilterType.Static,
                Items = RegisterDevices.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.Value,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.RegisterDevice,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "RegisterStatus",
                Label = TextResources.APP_StringKeys_Inspect_Status,
                Type = FilterType.Static,
                Items = InspectStatus.Select(p=> new ReportDataItem
                {
                    Label = p.Name,
                    Value = p.Value,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.InspectStatus,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "TagInDestination",
                Label = TextResources.APP_StringKeys_Warehouse,
                Type = FilterType.Static,
                Items = Warehouses.Select(p=> new ReportDataItem
                {
                    Label = p.DestinationTitle,
                    Value = p.DestinationCode,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.Warehouse,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            },
            new()
            {
                Id = FilterCount++,
                Component = FilterComponent.Drop,
                FieldName = "ProductSubGroup",
                Label = TextResources.APP_StringKeys_Product_SubGroup,
                Type = FilterType.Static,
                Items = ProductSubGroups.Select(p=> new ReportDataItem
                {
                    Label = p.Title,
                    Value = p.Code,
                    IsChoosen = false
                }).ToList(),
                FieldType = RegisterReportDynamicFilterType.ProductSubGroup,
                IsFilterShown = true,
                AdditionalData = new()
                {
                    { "FilterType", "Static"},
                    { "FilterId",(FilterCount-1).ToString()}
                }
            }
        };
    }

    private void InitDynamicFilters()
    {
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
                    IsFilterShown = field != null ? (field.FieldShowColumn && (field.ActionType == 0)) : false,
                    IsLikeCheckboxShown = true,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        {"DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
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
                    FieldType = RegisterReportDynamicFilterType.DynamicFields,
                    IsLikeCheckboxShown = false,
                    IsFilterShown = field != null ? (field.FieldShowColumn && (field.ActionType == 0)) : false,
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
            else if (field.ValueType == DynamicFieldValueType.RichTextEditor)
            {
                Filters.Add(new()
                {
                    Id = FilterCount++,
                    Label = field.Title,
                    FieldName = field.Title,
                    Type = FilterType.Dynamic,
                    FieldType = RegisterReportDynamicFilterType.DynamicFields,
                    Component = FilterComponent.RichTextEditor,
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
            else if (field.ValueType == DynamicFieldValueType.Numeric)
            {
                Filters.Add(new()
                {
                    Id = FilterCount++,
                    Label = field.Title,
                    FieldName = field.Title,
                    Type = FilterType.Dynamic,
                    FieldType = RegisterReportDynamicFilterType.DynamicFields,
                    Component = FilterComponent.Numeric,
                    IsFilterShown = field != null ? (field.FieldShowColumn && (field.ActionType == 0)) : false,
                    IsLikeCheckboxShown = false,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
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
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        {"DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                    }
                });
            }
        }

    }

    private void AddCalculatingColumns()
    {
        CalculatingColumns = new()
        {
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Count,
                Type = ReportCalculatingColumnType.Count,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductSerial
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_SumValue,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductCount
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_FirstDate,
                Type = ReportCalculatingColumnType.Min,
                GroupColumnType = RegisterReportDynamicColumnsType.PersianDateFull
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_LastDate,
                Type = ReportCalculatingColumnType.Max,
                GroupColumnType = RegisterReportDynamicColumnsType.PersianDateFull
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Average,
                Type = ReportCalculatingColumnType.Avg,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductCount
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Max_Count,
                Type = ReportCalculatingColumnType.Max,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductCount
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Min_Count,
                Type = ReportCalculatingColumnType.Min,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductCount
            },
            new ReportCalculatingColumn<RegisterReportDynamicColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Percent,
                Type = ReportCalculatingColumnType.Percent,
                GroupColumnType = RegisterReportDynamicColumnsType.ProductCount
            }
        };
    }

    private void AddSelectColumns()
    {
        DataColumns = new()
        {
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Code_Salon,
                Type = RegisterReportDynamicColumnsType.LineCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon,
                Type = RegisterReportDynamicColumnsType.LineTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Shift_Code,
                Type = RegisterReportDynamicColumnsType.ShiftCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Shift_Title,
                Type = RegisterReportDynamicColumnsType.ShiftTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductCode,
                Type = RegisterReportDynamicColumnsType.ProductCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTitle,
                Type = RegisterReportDynamicColumnsType.ProductName,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductSerial,
                Type = RegisterReportDynamicColumnsType.ProductSerial,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Chart_Regcode,
                Type = RegisterReportDynamicColumnsType.Regcode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Size_Code,
                Type = RegisterReportDynamicColumnsType.SizeCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Size_Title,
                Type = RegisterReportDynamicColumnsType.SizeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Chart_Qc,
                Type = RegisterReportDynamicColumnsType.QcCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Qc_Title,
                Type = RegisterReportDynamicColumnsType.QcTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_DocCode,
                Type = RegisterReportDynamicColumnsType.DocCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Register_Device,
                Type = RegisterReportDynamicColumnsType.RegisterDevice,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Code,
                Type = RegisterReportDynamicColumnsType.GroupCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Title,
                Type = RegisterReportDynamicColumnsType.GroupTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Code,
                Type = RegisterReportDynamicColumnsType.BrandCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Title,
                Type = RegisterReportDynamicColumnsType.BrandTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductType_Code,
                Type = RegisterReportDynamicColumnsType.TypeCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductType_Title,
                Type = RegisterReportDynamicColumnsType.TypeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Inspect_Status,
                Type = RegisterReportDynamicColumnsType.InspectStatus,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_RegUser,
                Type = RegisterReportDynamicColumnsType.RegisterUser,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Full,
                Type = RegisterReportDynamicColumnsType.PersianDateFull,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Month,
                Type = RegisterReportDynamicColumnsType.PersianDateMonth,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Week,
                Type = RegisterReportDynamicColumnsType.PersianDateWeek,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++ ,
                Title = TextResources.APP_StringKeys_PersianDate_Year,
                Type = RegisterReportDynamicColumnsType.PersianDateYear,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Full,
                Type = RegisterReportDynamicColumnsType.GregorianDateFull,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Month,
                Type = RegisterReportDynamicColumnsType.GregorianDateMonth,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Week,
                Type = RegisterReportDynamicColumnsType.GregorianDateWeek,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GregorianDate_Year,
                Type = RegisterReportDynamicColumnsType.GregorianDateYear,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = $"{TextResources.APP_StringKeys_Code} {TextResources.APP_StringKeys_Warehouse}",
                Type = RegisterReportDynamicColumnsType.WarehouseCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = $"{TextResources.APP_StringKeys_Title} {TextResources.APP_StringKeys_Warehouse}",
                Type = RegisterReportDynamicColumnsType.WarehouseTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_SubGroup_Code,
                Type = RegisterReportDynamicColumnsType.SubGroupCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_SubGroup_Title,
                Type = RegisterReportDynamicColumnsType.SubGroupTitle,
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
            DataColumns.Add(new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = field.Title,
                Type = RegisterReportDynamicColumnsType.DynamicFields,
                IsColumnShown = field != null ? (field.FieldShowColumn && (field.ActionType == 0)) : false,
                AdditionalData = new Dictionary<string, string>()
                {
                    { "ColumnType", "Dynamic"},
                    { "ColumnId", field.Id.ToString()},
                    {"DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                }
            });
        }
    }

    private void AddPivotColumns()
    {
        PivotColumns = new()
        {
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Chart_Qc + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.QcTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Brand_Title + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.BrandTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_ProductTypeTitle + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.TypeTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Group_Title + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.GroupTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Product_Size + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.SizeTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Line_Title_Salon + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.LineTitle
            },
            new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Shift_Title + "(Pivot)",
                Type = RegisterReportDynamicColumnsType.ShiftTitle
            }
        };
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

                PivotColumnTitles.Add(item);

                ColumnAgg.Add(item, 0);
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

    private void InitDataMiningElementColumns()
    {
        DataMiningElementColumns = new();

        foreach (var element in DataMiningElements)
        {
            DataMiningElementColumns.Add(new ReportColumnGeneric<RegisterReportDynamicColumnsType>
            {
                Id = ColumnCount++,
                Title = element.Title,
                Type = RegisterReportDynamicColumnsType.DataMiningElements,
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

    private List<ReportFilterGeneric<RegisterReportDynamicFilterType>> AggregateFilterValues()
    {
        List<ReportFilterGeneric<RegisterReportDynamicFilterType>> filters = new();

        filters = ApplyFilters.GroupBy(p => p.FieldName)
                              .Select(p => new
                                ReportFilterGeneric<RegisterReportDynamicFilterType>()
                              {
                                  FieldName = p.Key,
                                  Type = p.First().Type,
                                  Component = p.First().Component,
                                  EqualityType = p.First().IsLike ? FilterEqualityType.Like: FilterEqualityType.Equals,
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

                            if (column != null)
                            {
                                column.SortType = detail.SortType;
                                column.AggType = detail.AggType;
                                AddedDataColumns.Add(column);
                                GridColumns.Add(column.Title);
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
    #endregion
}
