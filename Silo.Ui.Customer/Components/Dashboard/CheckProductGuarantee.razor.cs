using Silo.Application.Features;
using Silo.Identity.Client;

namespace Silo.Ui.Customer.Components.Dashboard;
public partial class CheckProductGuarantee
{
    public bool IsLoading = true;
    public string Company;
    public CheckCustomerGuaranteeVm CustomerGuarantee;
    public CheckCustomerGuaranteeQuery Command = new();
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductSubGroupVm> SubGroups;
    public List<GetAllProductSubGroupVm> FilteredSubGroups;
    public CustomerCheckGuaranteePageMode ActiveSection = CustomerCheckGuaranteePageMode.Search;
    public List<GetProductModelsVm> ProductModels;
    public List<GetProductModelsVm> FilteredProductModels;
    public SearchProductModelDto SearchProductModel = new();
    public GuaranteeTypes GuaranteeStartType;

    [Parameter] public bool IsComponentShown { get; set; } = false;

    [Inject] public ISiloApiClient Api { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
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

        var roles = await ClaimManager.GetUserRoles();

        if (roles.Any(p => p.Name.ToLower().Equals("shop")))
        {
            GuaranteeStartType = GuaranteeTypes.Sell;
        }
        else
        {
            GuaranteeStartType = GuaranteeTypes.Install;
        }

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext e)
    {
        IsLoading = true;

        CustomerGuarantee = (await Api.PostAsyncByUri<CheckCustomerGuaranteeVm>("wms/CustomerGuarantee"
                   , "SCheckProductGuarantee"
                   , new KeyValuePair<string, object>("command", Command))).Value;

        if (CustomerGuarantee is not null)
        {
            if (CustomerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.Exist)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.Exist;
            }
            else if (CustomerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.ActivedNow)
            {
                ActiveSection = CustomerCheckGuaranteePageMode.ActivedNow;
            }
            else if (CustomerGuarantee.GuaranteeCheckResultStatus == CustomerCheckGuaranteePageMode.NotExist)
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
}
