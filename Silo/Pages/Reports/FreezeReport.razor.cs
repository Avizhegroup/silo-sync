using AutoMapper;

namespace Silo.Pages.Reports;
public partial class FreezeReport
{
    public bool IsLoading = true;
    public GetAllFreezeReportQuery Search = new();
    public List<GetAllFreezeHeaderVm> FreezeHeaders;
    public List<GetAllFreezeItemVm> FreezeItems;
    public List<UserDropDownableDto> Users = new();

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetails { get; set; }
    public ProductSerialModal ProductModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }

    protected override async Task SiloInitializer()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
        new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<UserDropDownableDto>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        IsLoading = false;
    }

    public async Task OnClickRowDetails(int code)
    {
        IsLoading = true;

        GetAllFreezeItemReportQuery search = FixItemEmptiness();

        search.HeaderId = code;

        FreezeItems = (await Api.PostAsync<List<GetAllFreezeItemVm>>("SGetFreezeItemReport"
                    , new KeyValuePair<string, object>[] { new("search", search) })).Value;

        IsLoading = false;

        await ModalDetails.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        GetAllFreezeReportQuery search = FixEmptiness();

        FreezeHeaders = (await Api.PostAsync<List<GetAllFreezeHeaderVm>>("SGetFreezeHeaderReport",
            new KeyValuePair<string, object>[] { new("search", search) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickProductCode(string code)
    {
        Search.ProductCode = code;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Search = new GetAllFreezeReportQuery();

        FreezeHeaders = null;

        FreezeItems = null;
    }

    public async Task OnSelectSerials(List<GetAllProductBySerialVm> products)
    {
        Search.ProductSerial = products[0].ProductSerial;
    }

    private GetAllFreezeReportQuery FixEmptiness()
    {
        GetAllFreezeReportQuery search = new();

        if (Search.ProductCode.HasNoValue())
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Search.ProductCode;
        }

        if (Search.ProductSerial.HasNoValue())
        {
            search.ProductSerial = "-1";
        }
        else
        {
            search.ProductSerial = Search.ProductSerial;
        }

        if (Search.FromDate.HasNoValue())
        {
            search.FromDate = "-1";
        }
        else
        {
            search.FromDate = Search.FromDate;
        }

        if (Search.ToDate.HasNoValue())
        {
            search.ToDate = "-1";
        }
        else
        {
            search.ToDate = Search.ToDate;
        }

        if (Search.UserId.HasNoValue())
        {
            search.UserId = "-1";
        }
        else
        {
            search.UserId = Search.UserId;
        }

        if (Search.TechnicalCode.HasNoValue())
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Search.TechnicalCode;
        }

        search.TechnicalCodeLike = Search.TechnicalCodeLike;

        return search;
    }

    private GetAllFreezeItemReportQuery FixItemEmptiness()
    {
        GetAllFreezeItemReportQuery search = new();

        if (Search.ProductCode.HasNoValue())
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Search.ProductCode;
        }

        if (Search.ProductSerial.HasNoValue())
        {
            search.ProductSerial = "-1";
        }
        else
        {
            search.ProductSerial = Search.ProductSerial;
        }

        if (Search.FromDate.HasNoValue())
        {
            search.FromDate = "-1";
        }
        else
        {
            search.FromDate = Search.FromDate;
        }

        if (Search.ToDate.HasNoValue())
        {
            search.ToDate = "-1";
        }
        else
        {
            search.ToDate = Search.ToDate;
        }

        if (Search.UserId.HasNoValue())
        {
            search.UserId = "-1";
        }
        else
        {
            search.UserId = Search.UserId;
        }

        if (Search.TechnicalCode.HasNoValue())
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Search.TechnicalCode;
        }

        search.TechnicalCodeLike = Search.TechnicalCodeLike;

        return search;
    }
}
