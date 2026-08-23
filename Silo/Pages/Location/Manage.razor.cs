using Silo.Application.Features;

namespace Silo.Pages.Location;
public partial class Manage
{
    public bool IsLoading = false;
    public GetProductByZoneAndProductCodeQuery Request = new();
    public List<GetProductByZoneAndProductCodeVm> Positions;
    public List<GetProductDetailsByZoneAndProductCodeVm> Details;
    public string DetailsZone = string.Empty;
    public string DetailsCode = string.Empty;

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationModal { get; set; }
    public Modal ModalDetails { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    public async Task OnSelectProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnSelectLocationCode(string code)
    {
        Request.ZoneCode = code;
    }

    public async Task OnClickRowDetails(string productCode, string zoneCode)
    {
        IsLoading = true;

        DetailsZone = zoneCode;

        DetailsCode = productCode;

        Details = (await Api.PostAsync<List<GetProductDetailsByZoneAndProductCodeVm>>("SGetProductDetails",
                new("code", productCode),
                new("zone", zoneCode))).Value;

        await ModalDetails.Open(new());

        IsLoading = false ;
    }

    public async Task OnSubmitClick(MouseEventArgs e)
    {
        IsLoading = true;
        
        GetProductByZoneAndProductCodeQuery  request = FixEmptiness();
        
        Positions = (await Api.PostAsync<List<GetProductByZoneAndProductCodeVm>>("SPositionSearch",
            new KeyValuePair<string, object>[] { new("search", request) })).Value;

        IsLoading = false ;

        GetProductByZoneAndProductCodeQuery FixEmptiness()
        {
            GetProductByZoneAndProductCodeQuery request = new();

            if (string.IsNullOrEmpty(Request.ZoneCode))
            {
                request.ZoneCode = "-1";
            }
            else
            {
                request.ZoneCode = Request.ZoneCode;
            }

            if (string.IsNullOrEmpty(Request.ProductCode))
            {
                request.ProductCode = "-1";
            }
            else
            {
                request.ProductCode = Request.ProductCode;
            }

            return request;
        }
    }
}
