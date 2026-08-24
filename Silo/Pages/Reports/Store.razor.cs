using System.Text.Json;
using Silo.Application;
using Silo.Shared.Tools;

namespace Silo.Pages.Reports;
public partial class Store
{
    public bool IsLoading = true;
    public string UserId;
    public int FilterCount = 1;
    public GetAllProductInStoreQuery Request = new();
    public List<GetAllProductInStoreVm> Products;
    public List<GetAllProductInStoreDetailsVm> Details;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllLinesVm> Lines;
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<TelerikDropDownItem> RangeAges = new()
    {
        new() { Name = "تا یک ماه", Value = "1" },
        new() { Name = "یک تا سه ماه", Value = "2" },
        new() { Name = "سه تا شش ماه", Value = "3" },
        new() { Name = "شش ماه تا یک سال", Value = "4" },
        new() { Name = "بالای یک سال", Value = "5" }
    };
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<GetAllDynamicFieldVm> DynamicFieldsForFilters = new();
    //Action Type = 0 - Update april 2025
    public List<string> DynamicFieldRegisterDataColumns = new();

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    public Modal ModalDetails { get; set; }
    public Modal FiltersModal { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Warehouses = await FormalCache.GetWarehouses();

        Lines = await FormalCache.GetLines();

        Shifts = await FormalCache.GetShifts();

        await InitFilters();

        IsLoading = false;
    }

    public async Task OnSearchClick()
    {
        IsLoading = true;

        var filters = DynamicFilterTools.AggregateFilterValues(ApplyFilters);

        if (!filters.Exists(p => p.FieldName.Equals("WarehouseCode")))
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Warehouse), "error");

            IsLoading = false;

            return;
        }

        Products = (await Api.PostAsyncByOption<List<GetAllProductInStoreVm>>("SInStoreReport",
            new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.WriteAsString
            },
            new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickRowDetails(string code, string warehouseCode)
    {
        IsLoading = true;

        foreach (var filter in ApplyFilters.Where(p => p.Type == FilterType.Dynamic))
        {
            if (filter.AdditionalData.First(p => p.Key.Equals("DynamicFilterActionType")).Value == "0")
            {
                DynamicFieldRegisterDataColumns.ReplaceOrAdd(p => p.Equals(filter.Label), filter.Label);
            }
        }

        foreach (var dynamicField in DynamicFieldsForFilters)
        {
            if (dynamicField.ActionType != 0 || !dynamicField.FieldShowColumn)
            {
                continue;
            }

            DynamicFieldRegisterDataColumns.ReplaceOrAdd(p => p.Equals(dynamicField.Title), dynamicField.Title);
        }

        var filters = ApplyFilters.GroupBy(p => p.FieldName)
                                  .Select(p => new ReportFilter()
                                  {
                                      FieldName = p.Key,
                                      Type = p.First().Type,
                                      Component = p.First().Component,
                                      EqualityType = p.First().EqualityType,
                                      AddType = p.First().AddType,
                                      Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                                  }).ToList();


        ReportFilter productCodeFilter = Filters.First(p => p.FieldName.Equals("ProductCode"));

        ReportFilter warehouseCodeFilter = Filters.First(p => p.FieldName.Equals("WarehouseCode"));

        productCodeFilter.Values = new()
        {
            code
        };

        warehouseCodeFilter.Values = new()
        {
            warehouseCode
        };

        filters.RemoveAll(p => p.FieldName.Equals("ProductCode"));

        filters.RemoveAll(p => p.FieldName.Equals("WarehouseCode"));

        filters.Add(productCodeFilter);

        filters.Add(warehouseCodeFilter);

        Details = (await Api.PostAsyncByContext<List<GetAllProductInStoreDetailsVm>>("SGetInStoreProductsByProductCode"
            , new GetAllProductInStoreDetailsVmContext()
            , new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Details = null;

        Products = null;
    }

    public async void OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        Filters = new();

        await InitFilters();

        await FiltersModal.Open(e);
    }

    public async Task OnExcelBeforeExport(GridBeforeExcelExportEventArgs args, string fileName)
    {
        IsLoading = true;

        var dataTable = ExcelExportTools.GetDataTableWithDynamicColumnAndValues(args);

        var stream = ExcelExporter.ExportDatatable(dataTable);

        stream.Seek(0, SeekOrigin.Begin);

        await Exporter.ExportAndDownload(stream, $"{fileName}.xlsx");

        IsLoading = false;
    }

    private async Task InitFilters()
    {
        Filters.Clear();

        #region Static Filters
        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_Warehouse,
            Type = FilterType.Static,
            Component = FilterComponent.Modal,
            FieldName = "WarehouseCode",
            IsLikeCheckboxShown = false,
            Items = Warehouses.Select(p => new ReportDataItem()
            {
                Label = p.DestinationTitle,
                Value = p.DestinationCode
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_ProductCode,
            Component = FilterComponent.ProductCodeModal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductCode"
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_ProductSerial,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductSerial"
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_Location,
            Component = FilterComponent.LocationModal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "TagZone"
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_FromDate,
            Component = FilterComponent.PersianDate,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "FromDate",
            EqualityType = FilterEqualityType.BiggerThan,
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_ToDate,
            Component = FilterComponent.PersianDate,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "ToDate",
            EqualityType = FilterEqualityType.SmallerThan,
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_Chart_Regcode,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "TechnicalCode"
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_QC,
            Component = FilterComponent.Modal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "Qc",
            Items = Qcs.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_Report_RangeAge,
            Component = FilterComponent.Modal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "AgeRange",
            Items = RangeAges.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Value
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            Label = TextResources.APP_StringKeys_Product_Size,
            Component = FilterComponent.Modal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "Size",
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
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

        #region Dynamic Filters
        await SetDynamicFiltersByActionType(new() { 0 });
        #endregion
    }

    private async Task SetDynamicFiltersByActionType(List<int> actionTypeIds)
    {
        if (actionTypeIds.Neither())
        {
            Filters.RemoveAll(p => p.Type == FilterType.Dynamic);

            return;
        }

        DynamicFieldsForFilters = new();

        foreach (var actionTypeId in actionTypeIds)
        {
            var dynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
            new KeyValuePair<string, object>("actionTypeId", actionTypeId))).Value;

            DynamicFieldsForFilters.AddRange(dynamicFields);
        }

        Filters.RemoveAll(p => p.Type == FilterType.Dynamic);

        foreach (var field in DynamicFieldsForFilters.Where(p => p.FieldType == DynamicFieldType.HeaderData))
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
                    IsLikeCheckboxShown = true,
                    AdditionalData = new()
                    {
                        { "DynamicFilterActionType", field.ActionType is not null ? field.ActionType.Value.ToString() : string.Empty }
                    }
                });
            }
        }

        Filters = Filters.OrderBy(p => p.Type).ToList();
    }
}
