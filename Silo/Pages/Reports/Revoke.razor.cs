using AutoMapper;
using Silo.Application.Features;

namespace Silo.Pages.Reports;

public partial class Revoke
{
    public bool IsLoading = true;
    public GetAllRevokeBySerialQuery Request = new();
    public List<GetAllRevokeBySerialVm> Products;
    public List<AddAccountCommand> Users;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task SiloInitializer()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.ProjectTo<AddAccountCommand>(
                applicationUsers.AsQueryable().Where(p => p.IsActive)).ToList();

        IsLoading = false;
    }

    public async Task OnClickSubmit(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllRevokeBySerialQuery search = FixEmptiness();

        Products = (await Api.PostAsync<List<GetAllRevokeBySerialVm>>("SRepSCancelRegisterTag",
            new KeyValuePair<string, object>[] {
            new("ProductSerial", search.ProductSerial) ,
            new("FromDate", search.FromDate) ,
            new("ToDate", search.ToDate) ,
            new("User", search.User)
            })).Value;

        if (Products.Any())
        {
            IsFiltersShown = false;
        }

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();
        Products = null;
    }

    private GetAllRevokeBySerialQuery FixEmptiness()
    {
        GetAllRevokeBySerialQuery search = new();

        if (string.IsNullOrEmpty(Request.ProductSerial))
        {
            search.ProductSerial = "-1";
        }
        else
        {
            search.ProductSerial = Request.ProductSerial;
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

        if (string.IsNullOrEmpty(Request.User))
        {
            search.User = "-1";
        }
        else
        {
            search.User = Request.User;
        }

        return search;
    }

}
