using Silo.Application;
using Silo.Application.Features;

namespace Silo.Pages.Reports;

public partial class Back
{
    public bool IsLoading = true;
    public GetAllReturnProductQuery Request = new();
    public List<GetAllShiftsVm> Shifts;
    public List<GetAllReturnProductVm> Products;
    public List<GetAllReturnProductDetailsVm> Details;

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetails { get; set; }
    
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] IFormalDataCache FormalDataCache { get; set; }

    protected override async Task SiloInitializer()
    {
        Shifts = await FormalDataCache.GetShifts();

        IsLoading = false;
    }

    public async Task OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnClickSubmit()
    {
        IsLoading = true;

        GetAllReturnProductQuery request = FixEmptiness();

        Products = (await Api.PostAsync<List<GetAllReturnProductVm>>("SReportBackEnterTajamoeeProductCode"
          , new KeyValuePair<string, object>("search", request))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Products = null;
    }

    public async Task OnClickRowDetails(string productCode)
    {
        IsLoading = true;

        GetAllReturnProductQuery request = FixEmptiness();

        request.ProductCode = productCode;

        Details = (await Api.PostAsyncByContext<List<GetAllReturnProductDetailsVm>>("SReportBackEnterProductList"
            , new GetAllReturnProductDetailsVmContext()
            , new KeyValuePair<string, object>("search", request))).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    private GetAllReturnProductQuery FixEmptiness()
    {
        GetAllReturnProductQuery search = new();

        search.Shift = "-1";
        if (string.IsNullOrEmpty(Request.ProductName))
        {
            search.ProductName = "-1";
        }
        else
        {
            search.ProductName = Request.ProductName;
        }

        if (string.IsNullOrEmpty(Request.ProductCode))
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Request.ProductCode;
        }

        if (string.IsNullOrEmpty(Request.FromDate))
        {
            search.FromDate = "-1";
        }
        else
        {
            search.FromDate = Request.FromDate;
        }

        if (string.IsNullOrEmpty(Request.ToDate))
        {
            search.ToDate = "-1";
        }
        else
        {
            search.ToDate = Request.ToDate;
        }

        if (string.IsNullOrEmpty(Request.ProductSerial))
        {
            search.ProductSerial = "-1";
        }
        else
        {
            search.ProductSerial = Request.ProductSerial;
        }

        if (string.IsNullOrEmpty(Request.TechnicalCode))
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Request.TechnicalCode;
        }

        if (string.IsNullOrEmpty(Request.Shift))
        {
            search.Shift = "-1";
        }
        else
        {
            search.Shift = Request.Shift;
        }

        search.TechnicalCodeLike = Request.TechnicalCodeLike;

        return search;
    }
}
