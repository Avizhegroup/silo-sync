using Silo.Application;
using Silo.Application.Features;

namespace Silo.Pages.Reports;
public partial class FreezeProductReport
{
    public bool IsLoading = true;
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllLinesVm> Lines;
    public List<ApplicationUser> Users;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetFreezeProductsVm> Products;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal FiltersModal { get; set; }

    protected override async Task SiloInitializer()
    {
        Qcs = await FormalCache.GetQcs();

        Lines = await FormalCache.GetLines();

        Users = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") }))
                .Value
                .Where(p => p.IsActive)
                .ToList();

        Sizes = await FormalCache.GetSizes();

        InitFilters();

        IsLoading = false;
    }

    public async Task OnSearchClick()
    {
        IsLoading = true;

        List<ReportFilter> filters = new();

        filters = ApplyFilters.GroupBy(p => p.FieldName)
                              .Select(p => new ReportFilter()
                              {
                                  FieldName = p.Key,
                                  Type = p.First().Type,
                                  Component = p.First().Component,
                                  EqualityType = p.First().EqualityType,
                                  AddType = p.First().AddType,
                                  Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                              }).ToList();

        Products = (await Api.PostAsyncByContext<List<GetFreezeProductsVm>>("SGetFreezeProducts",
        new GetFreezeProductsVmContext(),
        new KeyValuePair<string, object>("reportFilters", filters))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        IsLoading = false;

        ApplyFilters = new();

        Products = null;
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

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        ApplyFilters.Remove(filter);
    }

    private void InitFilters()
    {
        int indexer = 0;

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "ProductCode",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Component = FilterComponent.ProductCodeModal,
            Label = TextResources.APP_StringKeys_ProductCode
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "Qc",
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Modal,
            Label = TextResources.APP_StringKeys_Chart_Qc,
            Items = Qcs.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "Line",
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Modal,
            Label = TextResources.APP_StringKeys_Line,
            Items = Lines.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "FreezeUser",
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Modal,
            Label = TextResources.APP_StringKeys_Freeze_User,
            Items = Users.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Id
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "TechnicalCode",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Component = FilterComponent.Text,
            Label = TextResources.APP_StringKeys_Chart_Regcode
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "Size",
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.Modal,
            Label = TextResources.APP_StringKeys_Product_Size,
            Items = Sizes.Select(p => new ReportDataItem()
            {
                Label = p.Title,
                Value = p.Code
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "FromDate",
            EqualityType = FilterEqualityType.BiggerThan,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.PersianDate,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "ToDate",
            EqualityType = FilterEqualityType.SmallerThan,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            Component = FilterComponent.PersianDate,
            Label = TextResources.APP_StringKeys_ToDate
        });

        Filters.Add(new()
        {
            Id = indexer++,
            FieldName = "FreezeDesc",
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            Component = FilterComponent.Text,
            Label = TextResources.APP_StringKeys_Freeze_Cause
        });
    }
}
