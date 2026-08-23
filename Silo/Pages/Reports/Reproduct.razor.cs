using DocumentFormat.OpenXml.Office2016.Excel;
using Silo.Application;
using Silo.Application.Features;


namespace Silo.Pages.Reports;
public partial class Reproduct
{
    public bool IsLoading = true;
    public GetAllReProductQuery Request = new();
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllReProductVm> Products;
    public List<GetAllReProductDetailsVm> Details;

    public List<TelerikDropDownItem> ListRangeAge = new()
    {
        new() { Name = "تا یک ماه", Value = "1" },
        new() { Name = "یک تا سه ماه", Value = "2" },
        new() { Name = "سه تا شش ماه", Value = "3" },
        new() { Name = "شش ماه تا یک سال", Value = "4" },
        new() { Name = "بالای یک سال", Value = "5" }
    };
    public List<TelerikDropDownItem> ListInStoreSt = new()
    {
        new() { Name = "بسته بندی", Value = "0" },
        new() { Name = "داخل انبار", Value = "1" },
        new() { Name = "خارج شده", Value = "2" }
    };

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationModal { get; set; }
    public Modal ModalDetails { get; set; }

    protected override async Task SiloInitializer()
    {
        Qcs = await FormalCache.GetQcs();
        IsLoading = false;
    }

    public async Task OnChooseProductCode(string productCode)
    {
        Request.ProductCode = productCode;
    }

    public async Task OnChooseLocation(string zone)
    {
        Request.TagZone = zone;
    }

    public async Task OnClickSearch(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllReProductQuery request = FixEmptiness();

        Products = (await Api.PostAsync<List<GetAllReProductVm>>("SInStoreReproductReport",
        new KeyValuePair<string, object>[] { new("search", request) })).Value;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        IsLoading = false;
        Request = new();
        Products = null;
    }

    public async Task OnClickRowDetails(string code)
    {
        IsLoading = true;

        GetAllReProductQuery request = FixEmptiness();

        request.ProductCode = code;

        Details = (await Api.PostAsync<List<GetAllReProductDetailsVm>>("SGetReproductsByProductCode",
          new KeyValuePair<string, object>[] { new("search", request) })).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    private GetAllReProductQuery FixEmptiness()
    {
        GetAllReProductQuery request = new();

        if (string.IsNullOrEmpty(Request.ProductSerial))
            request.ProductSerial = "-1";
        else
            request.ProductSerial = Request.ProductSerial;

        if (string.IsNullOrEmpty(Request.TagZone))
            request.TagZone = "-1";
        else
            request.TagZone = Request.TagZone;

        if (string.IsNullOrEmpty(Request.TechnicalCode))
            request.TechnicalCode = "-1";
        else
            request.TechnicalCode = Request.TechnicalCode;

        if (string.IsNullOrEmpty(Request.Qc))
            request.ProductStatus = "-1";
        else
            request.ProductStatus = Request.Qc;

        if (string.IsNullOrEmpty(Request.FromDate))
            request.FromDate = "-1";
        else
        {
            string[] temp = Request.FromDate.Split('/');
            request.FromDate = temp[0] + temp[1] + temp[2];
        }

        if (string.IsNullOrEmpty(Request.ToDate))
            request.ToDate = "-1";
        else
        {
            string[] temp = Request.ToDate.Split('/');
            request.ToDate = temp[0] + temp[1] + temp[2];
        }

        if (string.IsNullOrEmpty(Request.ProductCode))
            request.ProductCode = "-1";
        else
            request.ProductCode = Request.ProductCode;

        if (string.IsNullOrEmpty(Request.ProductSerial))
            request.ProductSerial = "-1";
        else
            request.ProductSerial = Request.ProductSerial;

        if (string.IsNullOrEmpty(Request.AgeRange))
            request.AgeRange = "-1";
        else
            request.AgeRange = Request.AgeRange;

        request.TagZoneLike = Request.TagZoneLike;

        request.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (string.IsNullOrEmpty(Request.EnterStatus))
            request.EnterStatus = "-1";
        else
            request.EnterStatus = Request.EnterStatus;

        return request;
    }
}
