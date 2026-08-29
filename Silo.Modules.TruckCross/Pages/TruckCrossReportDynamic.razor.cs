using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Dto.Filter;
using Silo.Identity.Client;
using Silo.Shared.Components.Report;

namespace Silo.Modules.TruckCross.Pages;

public partial class TruckCrossReportDynamic
{
    #region Private Fields
    private int FilterCount = 1;
    private int ColumnCount = 1;
    private int CalculationColumnCount = 1;
    private int PivotColumnCount = 1;
    #endregion

    #region Public Fields
    public bool IsLoading = true;
    public bool IsInitPageFinished = false;
    public string UserId;
    public List<object> Results = new();
    public List<string> GridColumns = new() { "ردیف" };
    public List<string> PivotColumnTitles = new();
    public Dictionary<string, decimal> ColumnAgg = new();
    public decimal TotalSum = 0;
    public GetReportFormatByIdVm Format;
    public new bool mustTitleSetAutomatically = true;
    #endregion

    #region Reference Data
    public List<GetAllUsersVm> Users;
    public List<GetAllTruckTypesVm> TruckTypes;
    public List<GetAllTruckCrossPresentCauseVm> Causes;
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes;
    public List<GetAllTruckCrossShipmentVm> Shipments;
    public List<GetAllTruckCrossCustomerVm> Customers;
    public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations;
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<GetDataMiningElementIdsAndTitlesDto> DataMiningElements = new();
    public List<GetAllDynamicFieldSectionsVm> DynamicFieldsSections;
    public List<TelerikDropDownItemGeneric<TruckCrossStatuses>> TruckCrossStatusList;
    #endregion

    #region Filter Collections
    public List<ReportFilterGeneric<TruckCrossReportFilterType>> Filters = new();
    public List<ReportFilterGeneric<TruckCrossReportFilterType>> ApplyFilters = new();
    #endregion

    #region Column Collections
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> DataColumns;
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> AddedDataColumns = new();
    public List<ReportCalculatingColumn<TruckCrossReportColumnsType>> CalculatingColumns;
    public List<ReportCalculatingColumn<TruckCrossReportColumnsType>> AddedCalculatingColumns = new();
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> PivotColumns;
    public ReportColumnGeneric<TruckCrossReportColumnsType> AddedPivotColumn;
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> DataMiningElementColumns = new();
    public List<ReportColumnGeneric<TruckCrossReportColumnsType>> AddedDataMiningElementColumns = new();
    #endregion

    #region Component References
    public ReportAllSection<TruckCrossReportColumnsType, TruckCrossReportFilterType> ReportAllSectionRef { get; set; }
    #endregion

    #region Parameters
    [Parameter] public int? FormatId { get; set; }
    #endregion

    #region Injected Services
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    #endregion

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;
        Users = Mapper.Map<List<ApplicationUser>, List<GetAllUsersVm>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        TruckTypes = (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value;

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

        OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetAllTruckCrossOperationType")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        Customers = (await Api.PostAsyncByUri<List<GetAllTruckCrossCustomerVm>>("wms/TruckCross", "SGetAllTruckCrossCustomer")).Value;

        OperationDestinations = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value;

        DynamicFieldsSections = (await Api.PostAsyncByUri<List<GetAllDynamicFieldSectionsVm>>(
            "wms/Document",
            "GetAllDynamicFieldSections")).Value;

        DynamicFieldsSections = DynamicFieldsSections.Where(p => p.DynamicFieldType >= (int)DynamicFieldType.TruckCrossPresent)
                                                     .ToList();

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetAllDynamicFields")).Value;

        DynamicFields = DynamicFields.Where(p => p.FieldType >= DynamicFieldType.TruckCrossPresent)
                                     .OrderBy(p => p.Id)
                                     .ToList();

        DataMiningElements = (await Api.PostAsync<GetDataMiningElementIdsAndTitlesVm>("SGetDataMiningElementIdsAndTitles"
            , new KeyValuePair<string, object>[]
            {
                new("request", new GetDataMiningElementIdsAndTitlesQuery { UsageType = 1})
            })).Value.Elements;

        TruckCrossStatusList = new()
        {
            new()
            {
                Value = TruckCrossStatuses.Present,
                Name = TextResources.APP_StringKeys_TruckCross_Presented
            },
            new()
            {
                Value = TruckCrossStatuses.Enter,
                Name = TextResources.APP_StringKeys_TruckCross_Entered
            },
            new()
            {
                Value = TruckCrossStatuses.Exit,
                Name = TextResources.APP_StringKeys_TruckCross_Exited
            },
            new()
            {
                Value = TruckCrossStatuses.Revoke,
                Name = TextResources.APP_StringKeys_TruckCross_Revoked
            }
        };

        InitColumnsAndFilters();

        if (FormatId is not null)
        {
            await LoadFormat();
        }

        IsInitPageFinished = true;

        IsLoading = false;
    }

    #region Search
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

        var tempResult = (await Api.SendAsyncObjectByUri<GetTruckCrossDynamicSearchVm>(HttpMethod.Get
            , "crosses/GetTruckCrossDynamicSearch"
            , new GetTruckCrossDynamicSearchQuery()
            {
                Filters = filters,
                SelectColumns = AddedDataColumns,
                Calculating = AddedCalculatingColumns,
                Pivot = AddedPivotColumn,
                DataMiningElements = AddedDataMiningElementColumns
            })).Value;

        Results = AddRowNumbersToResults(tempResult.List);

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

    #region Excel Export
    public async Task OnExcelExportClick(MouseEventArgs e)
    {
        await ExcelExporter.ExportJsonData(PageTitle, Results, GridColumns);
    }
    #endregion

    #region Filter Event Handlers
    public async Task OnFilterRemoveClick(ReportFilterGeneric<TruckCrossReportFilterType> filter)
    {
        ApplyFilters.RemoveAll(p => p.FieldName.Equals(filter.FieldName) && p.Value.Equals(filter.Value));
    }

    public async Task OnAddNewFilter(ReportFilterGeneric<TruckCrossReportFilterType> filter)
    {
        ApplyFilters.Add(filter);
    }
    #endregion

    #region Column Event Handlers
    public async Task OnDataColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<TruckCrossReportColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedDataColumns.Add(col);
    }

    public async Task OnDataColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<TruckCrossReportColumnsType>)DataColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedDataColumns.RemoveAll(p => p.Id == col.Id);
    }

    public async Task OnCalculatingColumnAdd(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<TruckCrossReportColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedCalculatingColumns.Add(new()
        {
            GroupColumnType = calCol.GroupColumnType,
            Id = calCol.Id,
            Title = calCol.Title,
            Type = calCol.Type
        });
    }

    public async Task OnCalculatingColumnRemove(ReportColumn column)
    {
        var calCol = (ReportCalculatingColumn<TruckCrossReportColumnsType>)CalculatingColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedCalculatingColumns.RemoveAll(p => p.Id == calCol.Id);
    }

    public async Task OnPivotColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<TruckCrossReportColumnsType>)PivotColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedPivotColumn = col;
    }

    public async Task OnPivotColumnRemove()
    {
        AddedPivotColumn = null;
    }

    public async Task OnDataMiningElementColumnAdd(ReportColumn column)
    {
        var col = (ReportColumnGeneric<TruckCrossReportColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedDataMiningElementColumns.Add(col);
    }

    public async Task OnDataMiningElementColumnRemove(ReportColumn column)
    {
        var col = (ReportColumnGeneric<TruckCrossReportColumnsType>)DataMiningElementColumns.FirstOrDefault(p => p.Id == column.Id);
        AddedDataMiningElementColumns.RemoveAll(p => p.Id == col.Id);
    }
    #endregion

    #region Private Helper Methods
    private void InitColumnsAndFilters()
    {
        InitStaticFilters();
        InitDynamicFilters();
        InitCalculatingColumns();
        InitSelectColumns();
        InitDynamicColumns();
        InitPivotColumns();
        InitDataMiningElementColumns();
    }

    private void AddPivotDataColumns(List<object> data)
    {
        GridColumns.Clear();
        PivotColumnTitles.Clear();
        ColumnAgg.Clear();

        var elementFirst = (JsonElement)data.First();
        var jobject = JObject.Parse(elementFirst.GetRawText());
        var names = jobject.Root.Cast<JProperty>().Select(x => x.Name);

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

    private List<ReportFilterGeneric<TruckCrossReportFilterType>> AggregateFilterValues()
    {
        return ApplyFilters.GroupBy(p => p.FieldName)
                          .Select(p => new ReportFilterGeneric<TruckCrossReportFilterType>()
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
    #endregion

    #region Filter Initialization
    private void InitStaticFilters()
    {
        // Date filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromDate",
            FieldType = TruckCrossReportFilterType.FromDate,
            Type = FilterType.Static,
            EqualityType = FilterEqualityType.BiggerThan,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_FromDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToDate",
            FieldType = TruckCrossReportFilterType.ToDate,
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ToDate,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        // Driver information filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "NationalCode",
            FieldType = TruckCrossReportFilterType.NationalCode,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_NationalCode,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "DriverName",
            FieldType = TruckCrossReportFilterType.DriverName,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_DriverName,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        // Vehicle plaque filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PlaqueFirstPart",
            FieldType = TruckCrossReportFilterType.PlaqueFirstPart,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " - " + "قسمت اول",
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PlaqueCharacter",
            FieldType = TruckCrossReportFilterType.PlaqueCharacter,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " - " + "حرف",
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PlaqueSecondPart",
            FieldType = TruckCrossReportFilterType.PlaqueSecondPart,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " - " + "قسمت دوم",
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PlaqueCityPart",
            FieldType = TruckCrossReportFilterType.PlaqueCityPart,
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_Plaque + " - " + "کد شهر",
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        // Present section filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PresentCause",
            FieldType = TruckCrossReportFilterType.PresentCause,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Present_Cause,
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
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PresentOperationType",
            FieldType = TruckCrossReportFilterType.PresentOperationType,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Type,
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
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PresentShipment",
            FieldType = TruckCrossReportFilterType.PresentShipment,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Shipment,
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
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PresentOperationDestination",
            FieldType = TruckCrossReportFilterType.PresentOperationDestination,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Component = FilterComponent.Drop,
            Label = TextResources.APP_StringKeys_TruckCross_Operation_Destination,
            Items = OperationDestinations.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PresentCustomer",
            FieldType = TruckCrossReportFilterType.PresentCustomer,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_TruckCross_Customer,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = Customers.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Id.ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        // Status filter
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "Status",
            FieldType = TruckCrossReportFilterType.Status,
            Type = FilterType.Static,
            Label = TextResources.APP_StringKeys_Status,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            IsFilterShown = true,
            Items = TruckCrossStatusList.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = ((int)p.Value).ToString()
            }).ToList(),
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });

        // Product title filter
        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductTitle",
            FieldType = TruckCrossReportFilterType.ProductTitle,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            IsFilterShown = true,
            Label = TextResources.APP_StringKeys_ProductTitle,
            Component = FilterComponent.Text,
            AdditionalData = new()
            {
                { "FilterType", "Static"},
                { "FilterId", (FilterCount-1).ToString()}
            }
        });
    }

    private void InitDynamicFilters()
    {
        foreach (var field in DynamicFields)
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
                    IsFilterShown = true,
                    IsLikeCheckboxShown = true,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFieldType", field.FieldType.ToString() }
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
                    IsFilterShown = true,
                    FieldName = field.Title,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFieldType", field.FieldType.ToString() }
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
                    IsFilterShown = true,
                    IsLikeCheckboxShown = true,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFieldType", field.FieldType.ToString() }
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
                    IsFilterShown = true,
                    IsLikeCheckboxShown = false,
                    AdditionalData = new Dictionary<string, string>()
                    {
                        { "FilterType", "Dynamic"},
                        { "FilterId", field.Id.ToString()},
                        { "DynamicFieldType", field.FieldType.ToString() }
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
                        { "DynamicFieldType", field.FieldType.ToString() }
                    }
                });
            }
        }
    }
    #endregion

    #region Column Initialization
    private void InitSelectColumns()
    {
        DataColumns = new()
        {
            // ID and Status
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Code,
                Type = TruckCrossReportColumnsType.Id,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Status,
                Type = TruckCrossReportColumnsType.TruckCrossStatus,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Date columns - Present
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Full + " " + TextResources.APP_StringKeys_TruckCross_Steps_Present,
                Type = TruckCrossReportColumnsType.PersianDateFull,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Year + " " + TextResources.APP_StringKeys_TruckCross_Steps_Present,
                Type = TruckCrossReportColumnsType.PersianDateYear,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Month + " " + TextResources.APP_StringKeys_TruckCross_Steps_Present,
                Type = TruckCrossReportColumnsType.PersianDateMonth,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_PersianDate_Day + " " + TextResources.APP_StringKeys_TruckCross_Steps_Present,
                Type = TruckCrossReportColumnsType.PersianDateDay,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Driver information
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_DriverName,
                Type = TruckCrossReportColumnsType.DriverName,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_NationalCode,
                Type = TruckCrossReportColumnsType.NationalCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Phone,
                Type = TruckCrossReportColumnsType.DriverPhone,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Vehicle information
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Plaque,
                Type = TruckCrossReportColumnsType.Plaque,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_TypeTruck,
                Type = TruckCrossReportColumnsType.TruckTypeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Present section columns
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Present_DateTime,
                Type = TruckCrossReportColumnsType.PresentDateTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Present_User,
                Type = TruckCrossReportColumnsType.PresentUsername,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Present_Cause,
                Type = TruckCrossReportColumnsType.PresentCause,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_Present_Desc,
                Type = TruckCrossReportColumnsType.PresentDesc,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Turn,
                Type = TruckCrossReportColumnsType.PresentTurn,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Operation_Type,
                Type = TruckCrossReportColumnsType.PresentOperationTypeTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Shipment,
                Type = TruckCrossReportColumnsType.PresentShipmentTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Operation_Destination,
                Type = TruckCrossReportColumnsType.PresentOperationDestinationTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Customer,
                Type = TruckCrossReportColumnsType.PresentCustomerTitle,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_UserRevoke,
                Type = TruckCrossReportColumnsType.PresentRevokeUsername,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_ProductRevokeDateTime,
                Type = TruckCrossReportColumnsType.PresentRevokeDateTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Enter section columns
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Enter_DateTime,
                Type = TruckCrossReportColumnsType.EnterDateTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Enter_User,
                Type = TruckCrossReportColumnsType.EnterUsername,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage,
                Type = TruckCrossReportColumnsType.EnterWeightTonage,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },

            // Exit section columns
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Exit_DateTime,
                Type = TruckCrossReportColumnsType.ExitDateTime,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Exit_User,
                Type = TruckCrossReportColumnsType.ExitUsername,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Exit_WeightTonage,
                Type = TruckCrossReportColumnsType.ExitWeightTonage,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Exit_PureWeightCargo,
                Type = TruckCrossReportColumnsType.ExitPureWeightCargo,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_GateOpCode,
                Type = TruckCrossReportColumnsType.GateOperationCode,
                IsColumnShown = true,
                AdditionalData = new()
                {
                    { "ColumnType", "Static"},
                    { "ColumnId", (ColumnCount -1).ToString()}
                }
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = TextResources.APP_StringKeys_MovementAction,
                Type = TruckCrossReportColumnsType.MovementActionId,
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
        foreach (var field in DynamicFields)
        {
            DataColumns.Add(new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = field.Title,
                Type = TruckCrossReportColumnsType.DynamicFields,
                IsColumnShown = true,
                AdditionalData = new Dictionary<string, string>()
                {
                    { "ColumnType", "Dynamic"},
                    { "ColumnId", field.Id.ToString()},
                    { "DynamicFieldType", field.FieldType.ToString() }
                }
            });
        }
    }

    private void InitCalculatingColumns()
    {
        CalculatingColumns = new()
        {
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Count + " " + TextResources.APP_StringKeys_TruckCross,
                Type = ReportCalculatingColumnType.Count,
                GroupColumnType = TruckCrossReportColumnsType.Id
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_SumValue + " " + TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = TruckCrossReportColumnsType.EnterWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_SumValue + " " + TextResources.APP_StringKeys_TruckCross_Exit_WeightTonage,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = TruckCrossReportColumnsType.ExitWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_SumValue + " " + TextResources.APP_StringKeys_TruckCross_Exit_PureWeightCargo,
                Type = ReportCalculatingColumnType.Sum,
                GroupColumnType = TruckCrossReportColumnsType.ExitPureWeightCargo
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Average + " " + TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage,
                Type = ReportCalculatingColumnType.Avg,
                GroupColumnType = TruckCrossReportColumnsType.EnterWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Average + " " + TextResources.APP_StringKeys_TruckCross_Exit_WeightTonage,
                Type = ReportCalculatingColumnType.Avg,
                GroupColumnType = TruckCrossReportColumnsType.ExitWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Max_Count + " " + TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage,
                Type = ReportCalculatingColumnType.Max,
                GroupColumnType = TruckCrossReportColumnsType.EnterWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Min_Count + " " + TextResources.APP_StringKeys_TruckCross_Enter_WeightTonage,
                Type = ReportCalculatingColumnType.Min,
                GroupColumnType = TruckCrossReportColumnsType.EnterWeightTonage
            },
            new ReportCalculatingColumn<TruckCrossReportColumnsType>
            {
                Id = CalculationColumnCount++,
                Title = TextResources.APP_StringKeys_Percent + " " + TextResources.APP_StringKeys_Count,
                Type = ReportCalculatingColumnType.Percent,
                GroupColumnType = TruckCrossReportColumnsType.Id
            }
        };
    }

    private void InitPivotColumns()
    {
        PivotColumns = new()
        {
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_Status + " (Pivot)",
                Type = TruckCrossReportColumnsType.TruckCrossStatus
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_TypeTruck + " (Pivot)",
                Type = TruckCrossReportColumnsType.TruckTypeTitle
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Present_Cause + " (Pivot)",
                Type = TruckCrossReportColumnsType.PresentCause
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Operation_Type + " (Pivot)",
                Type = TruckCrossReportColumnsType.PresentOperationTypeTitle
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Shipment + " (Pivot)",
                Type = TruckCrossReportColumnsType.PresentShipmentTitle
            },
            new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = PivotColumnCount++,
                Title = TextResources.APP_StringKeys_TruckCross_Customer + " (Pivot)",
                Type = TruckCrossReportColumnsType.PresentCustomerTitle
            }
        };
    }

    private void InitDataMiningElementColumns()
    {
        DataMiningElementColumns = new();

        foreach (var element in DataMiningElements)
        {
            DataMiningElementColumns.Add(new ReportColumnGeneric<TruckCrossReportColumnsType>
            {
                Id = ColumnCount++,
                Title = element.Title,
                Type = TruckCrossReportColumnsType.DataMiningElements,
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
    #endregion
}
