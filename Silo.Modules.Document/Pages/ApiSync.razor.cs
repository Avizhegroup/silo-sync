using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Document.Pages;
public partial class ApiSync
{
    public bool IsLoading = true;
    public GetApiSyncProductQuery Request = new();
    public GetApiSyncProductVm DetailsChoosed = new();
    public List<GetApiSyncProductVm> Results;
    public List<GetApiSyncProductDetailsVm> Details;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductTypeVm> Types;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses = new();
    public SendActionToApiCommand SendAction = new();
    public ProductCodesDto ProductCodes = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public string CompanyName;
    public List<GetAllStationsVm> Stations;

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetails { get; set; }
    public Modal ModalSend { get; set; }
    public TelerikDropDownList<GetAllActionTypesDto, int?> ActionTypesDrop { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Types = await FormalCache.GetTypes();


        Warehouses = await FormalCache.GetWarehouses();

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
       , "ActionType/ReadAll")).Value.List;

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
                , new GetAllStationsVmContext())).Value;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        Results = (await Api.PostAsyncByContext<List<GetApiSyncProductVm>>("SSearchApiSync"
                         , new GetApiSyncProductVmContext()
                         , new KeyValuePair<string, object>("request", Request))).Value;

        IsLoading = false;
    }

    public async Task OnValidSendSubmit(EditContext context)
    {
        IsLoading = true;

        var request = (GetApiSyncProductQuery)Request.Clone();

        request.ProductCode = DetailsChoosed.ProductCode;

        bool result = (await Api.PostAsync<bool>("SSendActionToApi"
                         , new KeyValuePair<string, object>("request", request)
                         , new KeyValuePair<string, object>("save", SendAction)
                         )).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }

        IsLoading = false;
    }

    public async Task OnValidUpdateProductCodeSubmit()
    {
        IsLoading = true;

        if (ProductCodes.NewProductCode.HasValue())
        {
            var request = (GetApiSyncProductQuery)Request.Clone();

            request.ProductCode = ProductCodes.OldProductCode;

            var result = (await Api.PostAsync<bool>("SUpdateProductCodeForSendToApi"
                             , new KeyValuePair<string, object>("request", request)
                             , new KeyValuePair<string, object>("newCode", ProductCodes.NewProductCode)
                             )).Value;

            if (result)
            {
                Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
            }
        }

        IsLoading = false;
    }

    public async Task OnProductCodeChoose(string productCode)
    {
        Request.ProductCode = productCode;
    }

    public async Task OnClickRowDetails(GetApiSyncProductVm row)
    {
        IsLoading = true;

        DetailsChoosed = row;

        var request = (GetApiSyncProductQuery)Request.Clone();

        request.ProductCode = row.ProductCode;

        Details = (await Api.PostAsyncByContext<List<GetApiSyncProductDetailsVm>>("SSearchApiSyncDetails"
                         , new GetApiSyncProductDetailsContext()
                         , new KeyValuePair<string, object>("request", request))).Value;

        IsLoading = false;

        await ModalDetails.Open(new());
    }

    public async Task OnSendClickRowDetails(GetApiSyncProductVm row)
    {
        DetailsChoosed = row;

        ProductCodes.OldProductCode = row.ProductCode;

        ProductCodes.NewProductCode = string.Empty;

        await ModalSend.Open(new());
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Results = null;
    }

    public async Task OnClearSendClick(MouseEventArgs e)
    {
        SendAction = new();

        ProductCodes.NewProductCode = string.Empty;
    }

    public async Task OnNavigateDate(bool isForward)
    {
        var date = PersianCalendarTools.PersianToGregorian(Request.Date);

        if (isForward)
        {
            Request.Date = PersianCalendarTools.GregorianToPersian(date.AddDays(1));
        }
        else
        {
            Request.Date = PersianCalendarTools.GregorianToPersian(date.AddDays(-1));
        }
    }

    public async Task OnActionTypeChange(object e)
    {
        if (e is null)
        {
            return;
        }

        IsLoading = true;

        int? actionTypeId = (int?)e;

        var actionType = ActionTypes.FirstOrDefault(p => p.Code == actionTypeId);

        List<GetAllWarehousesVm> items = new();

        foreach (string warehouseType in actionType.To.Split(','))
        {
            if (warehouseType.HasNoValue())
            {
                continue;
            }

            var warehouses = Warehouses.Where(p => p.OperationalType == Enum.Parse<DestinationOperationalType>(warehouseType))
                                       .ToList();

            foreach (var warehouse in warehouses)
            {
                items.Add(warehouse);
            }
        }

        Request.WarehouseCode = string.Empty;

        SearchWarehouses = items;

        ActionTypesDrop.Rebind();

        IsLoading = false;
    }

    public void OnRowRenderHandler(GridRowRenderEventArgs args)
    {
        string classStr = string.Empty;

        GetApiSyncProductVm item = args.Item as GetApiSyncProductVm;

        if (item.AvgApiSendStatus == item.Count)
        {
            classStr += "bg-success";
        }

        if (item.AvgApiSendStatus > 0 && item.Count > item.AvgApiSendStatus)
        {
            classStr += "bg-warning";
        }

        args.Class += classStr;
    }

    public async Task OnPdfExportClick(MouseEventArgs e)
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration["Settings:Company"];
        }

        List<KeyValuePair<string, object>> variables = new()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("CompanyName", CompanyName)
            , new("PageTitle", PageTitle)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
             new(nameof(GetApiSyncProductVm), Results)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ApiSync",
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
         , "PreparedReport/Create"
         , command);

        if (response.Value.Result < 1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            return;
        }

        await Exporter.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }
}
