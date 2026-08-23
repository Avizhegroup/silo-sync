using AutoMapper;
using Silo.Application;

namespace Silo.Pages.Reports;
public partial class Inventory
{
    public bool IsLoading = true;
    public GetAllInventoryQuery Request = new();
    public SaveInventoryByEpcsCommand UploadCommand = new();
    public List<UserDropDownableDto> Users;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllInventoryVm> Inventories;
    public List<string> InventorySerials;
    public List<GetAllInventoryBySerialVm> InventoriesBySerial;

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal ModalDetails { get; set; }
    public Modal ModalUploadText { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }

    protected override async Task SiloInitializer()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<UserDropDownableDto>>(applicationUsers.Where(p => p.IsActive).ToList());

        Warehouses = await FormalCache.GetWarehouses();

        IsLoading = false;
    }

    public async Task OnCompleteUploadText(string path)
    {
        UploadCommand.Epcs = (await File.ReadAllLinesAsync(path)).ToList();

        IsLoading = false;
    }

    public async Task OnInventoryEpcValidSubmit(EditContext context)
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SSaveInventoryBySerials"
            , new KeyValuePair<string, object>("command", UploadCommand))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await ModalUploadText.Close(new());

        IsLoading = false;
    }

    public async Task OnModalPrintOpenClick(MouseEventArgs e)
    {
        UploadCommand = new();

        await ModalUploadText.Open(new());
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Inventories = null;

        UploadCommand = new();
    }

    public async Task OnClickSubmit(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllInventoryQuery request = FixEmptiness();

        Inventories = (await Api.PostAsyncByContext<List<GetAllInventoryVm>>("SReportInventory"
            , new GetInventoryResponseContext()
            , new KeyValuePair<string, object>("request", request))).Value;

        InventoriesBySerial = (await Api.PostAsyncByContext<List<GetAllInventoryBySerialVm>>("SReportInventoryBySerial"
            , new GetAllInventoryBySerialVmContext()
            , new KeyValuePair<string, object>("request", request))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickRowDetails(string productCode)
    {
        IsLoading = true;

        GetAllInventoryQuery request = FixEmptiness();

        request.ProductCode = productCode;

        InventorySerials = (await Api.PostAsync<List<string>>("SReportInventorySerials"
            , new KeyValuePair<string, object>("request", request))).Value;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    private GetAllInventoryQuery FixEmptiness()
    {
        GetAllInventoryQuery search = new();

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

        if (string.IsNullOrEmpty(Request.FromTime))
        {
            search.FromTime = "-1";
        }
        else
        {
            search.FromTime = Request.FromTime;
        }

        if (string.IsNullOrEmpty(Request.ToTime))
        {
            search.ToTime = "-1";
        }
        else
        {
            search.ToTime = Request.ToTime;
        }


        if (string.IsNullOrEmpty(Request.Desc))
        {
            search.Desc = "-1";
        }
        else
        {
            search.Desc = Request.Desc;
        }

        if (string.IsNullOrEmpty(Request.Place))
        {
            search.Place = "-1";
        }
        else
        {
            search.Place = Request.Place;
        }

        if (string.IsNullOrEmpty(Request.Code))
        {
            search.Code = "-1";
        }
        else
        {
            search.Code = Request.Code;
        }

        if (string.IsNullOrEmpty(Request.User))
        {
            search.User = "-1";
        }
        else
        {
            search.User = Request.User;
        }

        if (string.IsNullOrEmpty(Request.WarehouseCode))
        {
            search.WarehouseCode = "-1";
        }
        else
        {
            search.WarehouseCode = Request.WarehouseCode;
        }

        if (string.IsNullOrEmpty(Request.ProductCode))
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Request.ProductCode;
        }

        if (string.IsNullOrEmpty(Request.TechnicalCode))
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Request.TechnicalCode;
        }

        return search;
    }
}
