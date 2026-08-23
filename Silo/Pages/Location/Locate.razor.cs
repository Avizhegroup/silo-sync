using System.Text.Json;
using Silo.Application;
using Silo.Application.Features;

namespace Silo.Pages.Location;
public partial class Locate
{
    public bool IsLoading = true;
    public bool IsAllSelected = false;
    public GetAllZonesVm NewTagZone;
    public List<GetAllActionTypesDto> ActionTypes;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductGroupVm> Groups;
    public List<ReportFilter> Filters = new();
    public List<string> TechnicalInfoDataKeys;
    public List<ReportFilter> ApplyFilters = new();
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllDynamicFieldVm> DynamicFields;
    public List<string> TechnicalFilterColumns = new();
    public List<GetLocateProductVm> Products;

    public Modal FiltersModal { get; set; }
    public LocationModal LocationModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
       , "ActionType/ReadAll")).Value.List;

        Warehouses = await FormalCache.GetWarehouses();
        SearchWarehouses = Warehouses;

        Sizes = await FormalCache.GetSizes();

        Qcs = await FormalCache.GetQcs();
        Groups = await FormalCache.GetGroups();

        Brands = await FormalCache.GetBrands();

        ProductTypes = await FormalCache.GetTypes();


        TechnicalInfoDataKeys = (await Api.PostAsyncByUri<List<string>>("wms/Product", "SGetAllTechnicalInfoDataKeys")).Value;

        InitFilters();

        IsLoading = false;
    }

    public async Task OnSearchButtonClick()
    {
        List<ReportFilter> filters = ApplyFilters.GroupBy(p => p.FieldName)
                                                 .Select(p => new ReportFilter()
                                                 {
                                                     FieldName = p.Key,
                                                     Type = p.First().Type,
                                                     Component = p.First().Component,
                                                     EqualityType = p.First().EqualityType,
                                                     AddType = p.First().AddType,
                                                     Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                                                 }).ToList();

        if (!filters.Exists(p => p.FieldName.Equals("Destination")))
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required,
                                TextResources.APP_StringKeys_Warehouse)
                  , "error");

            return;
        }

        IsLoading = true;

        TechnicalFilterColumns = new();

        foreach (var filter in ApplyFilters.Where(p => p.Type == FilterType.TechnicalInfo))
        {
            TechnicalFilterColumns.Add(filter.Label);
        }

        TechnicalFilterColumns = TechnicalFilterColumns.Distinct().ToList();

        Products = (await Api.PostAsyncByContext<List<GetLocateProductVm>>("SSearchProductForLocate",
            new GetLocateProductVmContext(),
            new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        ApplyFilters = new();

        Products = null;

        IsAllSelected = false;

        TechnicalInfoDataKeys = null;

        TechnicalFilterColumns = new();
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        Filters = new();

        InitFilters();

        await FiltersModal.Open(e);

        StateHasChanged();
    }

    public async Task OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnAddNewFilterInComponentClick(ReportFilter filter)
    {
        if (filter.FieldName.Equals("ActionType"))
        {
            await ChangeActionType(filter);
        }
    }

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        ApplyFilters.Remove(filter);
    }

    public async Task OnToggleSelectAll()
    {
        Products.ForEach(p => p.IsSelected = IsAllSelected);
    }

    public async Task OnToggleSelectChange(object value)
    {
        bool castedValue = (bool)value;

        if (!castedValue)
        {
            IsAllSelected = false;
        }
    }

    public async Task OnChooseNewTagZone(GetAllZonesVm zone)
    {
        NewTagZone = zone;
    }

    public async Task OnSaveClick(MouseEventArgs e)
    {
        if (NewTagZone is null)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Zone), "error");

            return;
        }

        if (Products is not null && !Products.Any(p => p.IsSelected))
        {
            Notification.Show(TextResources.APP_StringKeys_Error_SelectProduct, "error");

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsync<int>("SPSetTagZone"
            , new("ProductSerial", Products.Where(p => p.IsSelected)
                                           .Select(p => p.ProductSerial)
                                           .Aggregate(string.Empty, (first, next) => first + (next.HasNoValue() ? string.Empty : (",'" + next + "'")))
                                           .Remove(0, 1))
            , new("TagZone", NewTagZone.ZoneCode)
            , new("userToken", (await AuthState.GetAuthenticationStateAsync()).User.GetUserId()))).Value;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    private void InitFilters()
    {
        Filters.Clear();

        int indexer = 1;
        #region Static Filters

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "Destination",
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
            Id = indexer++,
            FieldName = "Zone",
            Type = FilterType.Static,
            Component = FilterComponent.LocationModal,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Zone
        });

        Filters.Add(new()
        {
            Id = indexer++,
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
            Id = indexer++,
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
            Id = indexer++,
            FieldName = "ProductCode",
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_ProductCode
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "TechnicalCode",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Chart_Regcode
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "OperationCode",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_OperationCode
        });

        Filters.Add(new()
        {
            Id = indexer++,
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
            Id = indexer++,
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
            Id = indexer++,
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
            Id = indexer++,
            FieldName = "FromDate",
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            EqualityType = FilterEqualityType.BiggerThan,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "ToDate",
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            IsLikeCheckboxShown = false,
            EqualityType = FilterEqualityType.SmallerThan,
            Label = TextResources.APP_StringKeys_ToDate
        });
        #endregion

        #region Technical Filters
        if (TechnicalInfoDataKeys is not null)
        {
            foreach (var field in TechnicalInfoDataKeys)
            {
                Filters.Add(new()
                {
                    Id = indexer,
                    Label = field,
                    FieldName = field,
                    Type = FilterType.TechnicalInfo,
                    IsLikeCheckboxShown = true,
                    Component = FilterComponent.Text
                });

                indexer++;
            }
        }
        #endregion
    }

    private async Task ChangeActionType(ReportFilter actionType)
    {
        int? actionTypeId = actionType is null ? 0 : int.Parse(actionType.Value);

        IsLoading = true;

        IsLoading = false;

        StateHasChanged();
    }
}
