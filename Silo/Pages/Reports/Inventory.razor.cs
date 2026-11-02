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
    public List<GetAllProductTypeVm> Types;
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;
    public List<string> DynamicFieldColumns = new();

    private static string GetDynamicValue(string properties, string field)
    {
        if (string.IsNullOrWhiteSpace(properties))
        {
            return string.Empty;
        }

        try
        {
            return Newtonsoft.Json.Linq.JToken.Parse(properties)[field]?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
    public List<GetAllInventoryVm> Inventories;
    public List<string> InventorySerials;
    public List<GetAllInventoryBySerialVm> InventoriesBySerial;
    public List<GetInventoryResultTagVm> UnregisteredEpcs = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();

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
        Types = await FormalCache.GetTypes();
        Groups = await FormalCache.GetGroups();
        Sizes = await FormalCache.GetSizes();
        Qcs = await FormalCache.GetQcs();

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", 0))).Value ?? new();

        DynamicFieldColumns = DynamicFields
            .Where(p => p.ActionType == 0 && p.FieldShowColumn)
            .Select(p => p.Title)
            .Distinct()
            .ToList();

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

        UnregisteredEpcs = new();

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

        UnregisteredEpcs = (await Api.PostAsyncByContext<List<GetInventoryResultTagVm>>("SReportInventoryUnregisteredEpcs"
            , new GetInventoryResultTagVmContext()
            , new KeyValuePair<string, object>("request", request))).Value ?? new();

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

        search.ProductType = string.IsNullOrEmpty(Request.ProductType) ? "-1" : Request.ProductType;
        search.ProductGroup = string.IsNullOrEmpty(Request.ProductGroup) ? "-1" : Request.ProductGroup;
        search.ProductSize = string.IsNullOrEmpty(Request.ProductSize) ? "-1" : Request.ProductSize;
        search.ProductStatus = string.IsNullOrEmpty(Request.ProductStatus) ? "-1" : Request.ProductStatus;

        return search;
    }
}
