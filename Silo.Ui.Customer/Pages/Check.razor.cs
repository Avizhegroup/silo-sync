using Silo.Application.Features;

namespace Silo.Ui.Customer.Pages;
public partial class Check 
{
    // Place this line in the top of class
    internal bool mustCheckAccess = false;

    public bool IsLoading = true;
    public string Company;
    public CheckCustomerGuaranteeVm CustomerGuarantee = new();
    public CheckCustomerGuaranteeForCustomerQuery Command = new();
    public List<GetAllProvinceVm> Provinces = new();
    public List<GetCitiesVm> Cities;
    public List<GetCitiesVm> FilteredCities;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductSubGroupVm> SubGroups;
    public List<GetAllProductSubGroupVm> FilteredSubGroups;
    public CustomerCheckGuaranteePageMode ActiveSection = CustomerCheckGuaranteePageMode.Search;
    public List<GetProductModelsVm> ProductModels;
    public List<GetProductModelsVm> FilteredProductModels;
    public SearchProductModelDto SearchProductModel = new();

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public ISiloApiClient Api { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    protected override async Task  OnInitializedAsync()
    {
        Company = Configuration["Settings:Company"];

        Provinces = (await Api.PostAsyncByBaseUrlAndUri<List<GetAllProvinceVm>>("RfidConnectApi:IpAno"
            , "wms/CustomerGuarantee"
            , "GetAllProvinces")).Value;

        Cities = (await Api.PostAsyncByBaseUrlAndUriAndContext<List<GetCitiesVm>>("RfidConnectApi:IpAno"
            , "wms/CustomerGuarantee"
            , "GetAllCities"
            , new GetCitiesVmContext())).Value;

        Groups = (await Api.PostAsyncByBaseUrlAndUriAndContext<List<GetAllProductGroupVm>>("RfidConnectApi:IpAno"
            , "wms/CustomerGuarantee"
            , "SGetAllProductGroups"
            , new GetAllProductGroupVmContext())).Value;

        SubGroups = (await Api.PostAsyncByBaseUrlAndUriAndContext<List<GetAllProductSubGroupVm>>("RfidConnectApi:IpAno"
            , "wms/CustomerGuarantee"
            , "SGetAllProductSubGroups"
            , new GetAllProductSubGroupVmContext())).Value;

        FilteredSubGroups = SubGroups;

        ProductModels = (await Api.PostAsyncByBaseUrlAndUriAndContext<List<GetProductModelsVm>>("RfidConnectApi:IpAno"
            , "wms/CustomerGuarantee"
            , "SGetProductModels"
            , new GetProductModelsVmContext())).Value;

        FilteredProductModels = ProductModels.DistinctBy(p => p.TechnicalCode).ToList();

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext e)
    {
        IsLoading = true;

        CheckCustomerGuaranteeVm customerGuarantee = (await Api.PostAsyncByBaseUrlAndUri<CheckCustomerGuaranteeVm>("RfidConnectApi:IpAno"
                   , "wms/CustomerGuarantee"
                   , "SCheckProductGuaranteeForCustomer"
                   , new KeyValuePair<string, object>("command", Command))).Value;

        if (customerGuarantee is not null)
        {
            if (customerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.Exist)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.Exist;

                CustomerGuarantee = customerGuarantee;
            }
            else if (customerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.ActivedNow)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.ActivedNow;

                CustomerGuarantee = customerGuarantee;
            }
            else if (customerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.CheckedBefore)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.CheckedBefore;
            }
            if (customerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.NotExist)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.NotExist;
            }
        }
        else
        {
            ActiveSection = CustomerCheckGuaranteePageMode.NotExist;
        }

        IsLoading = false;
    }

    public async Task OnProvinceChange(object e)
    {
        if (e is null)
        {
            return;
        }

        FilteredCities = Cities.Where(c => c.ProvinceId == (string)e).ToList();
    }

    public async Task OnGroupChange(object e)
    {
        if (e is null)
        {
            Command.RegCode = null;

            SearchProductModel.Group = null;

            SearchProductModel.SubGroup = null;

            FilteredSubGroups = SubGroups;

            FilteredProductModels = ProductModels.DistinctBy(p => p.TechnicalCode).ToList();

            return;
        }

        SearchProductModel.Group = (string)e;

        FilteredSubGroups = SubGroups.Where(p => p.ProductGroupCode == SearchProductModel.Group).ToList();

        SearchProductModal();
    }

    public async Task OnSubGroupChange(object e)
    {
        if (e is null)
        {
            Command.RegCode = null;

            if (SearchProductModel.Group.HasValue())
            {
                FilteredSubGroups = SubGroups.Where(p => p.ProductGroupCode == SearchProductModel.Group).ToList();
            }
            else
            {
                FilteredSubGroups = SubGroups;
            }

            return;
        }

        SearchProductModel.SubGroup = (string)e;

        SearchProductModal();
    }

    public void OnClearClick(MouseEventArgs e)
    {
        Command = new();

        ActiveSection = CustomerCheckGuaranteePageMode.Search;

        SearchProductModel = new();

        FilteredSubGroups = SubGroups;

        FilteredProductModels = ProductModels.DistinctBy(p => p.TechnicalCode).ToList();
    }

    public async Task OnReturnToSearchClick(MouseEventArgs e)
    {
        ActiveSection = CustomerCheckGuaranteePageMode.Search;
    }

    private void SearchProductModal()
    {
        FilteredProductModels = ProductModels;

        if (SearchProductModel.Group.HasValue())
        {
            FilteredProductModels = FilteredProductModels.Where(p => p.ProductGroup == SearchProductModel.Group).ToList();
        }
        if (SearchProductModel.SubGroup.HasValue())
        {
            FilteredProductModels = FilteredProductModels.Where(p => p.ProductSubGroup == SearchProductModel.SubGroup).ToList();
        }

        FilteredProductModels = FilteredProductModels.DistinctBy(p => p.TechnicalCode).ToList();
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        foreach (string message in context.GetValidationMessages())
        {
            Notification.Show(message, "error");
        }
    }
}
