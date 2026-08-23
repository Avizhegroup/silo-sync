using Silo.Application;

namespace Silo.Pages.Product;
public partial class Enter
{
    public bool IsLoading = true;
    public GetAllEnterProductQuery Request = new();
    public List<GetAllEnterProductVm> Products;
    public List<GetAllEnterProductDetailsVm> Details;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;
    public string UserId;
    public string CompanyName;
    public List<GetAllWarehousesVm> Warehouses;
    public List<GetAllWarehousesVm> SearchWarehouses = new();
    public List<GetAllProductGroupVm> Groups;
    public List<GetAllProductBrandVm> Brands;
    public List<GetAllProductTypeVm> ProductTypes;
    public List<GetAllActionTypesDto> ActionTypes;
    public List<GetAllStationsVm> Stations;

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetails { get; set; }
    public TelerikDropDownList<GetAllWarehousesVm, string?> WarehousesDrop { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }
    [Inject] public IExport Exporter { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthStateProvider.GetAuthenticationStateAsync()).User.GetUserId();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        Warehouses = await FormalCache.GetWarehouses();

        Groups = await FormalCache.GetGroups();

        Brands = await FormalCache.GetBrands();

        ProductTypes = await FormalCache.GetTypes();


        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Stations = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
                        , new GetAllStationsVmContext())).Value;

        SearchWarehouses = Warehouses;

        IsLoading = false;
    }

    public async Task OnClickRowDetails(GetAllEnterProductVm product)
    {
        IsLoading = true;

        GetAllEnterProductQuery search = FixEmptiness();

        search.ProductCode = product.ProductCode;

        search.DestinationCode = product.DestinationCode;

        search.ActionType = product.ActionType;

        Details = (await Api.PostAsync<List<GetAllEnterProductDetailsVm>>("SReportEnterProductsByProductCode"
                    , new KeyValuePair<string, object>[] { new("search", search) })).Value;

        IsLoading = false;

        await ModalDetails.Open(new());
    }

    public async Task OnValidSubmit()
    {
        if (ValidateSearch().Equals(false))
        {
            return;
        }

        IsLoading = true;

        GetAllEnterProductQuery search = FixEmptiness();

        Products = (await Api.PostAsync<List<GetAllEnterProductVm>>("SReportEnter",
            new KeyValuePair<string, object>[] { new("search", search) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnMasterExcelExportClick()
    {
         DataTable table = DataTableTools.GetDataTableUsingDisplayAttribute(Products, typeof(GetAllEnterProductVm));

        var stream = ExcelExporter.ExportDatatable(table);

        stream.Seek(0, SeekOrigin.Begin);

        await Exporter.ExportAndDownload(stream, $"{TextResources.APP_StringKeys_View_Report_Enter}.xlsx");
    }

    public async Task OnMasterExportToPdfClick()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetAllEnterProductVm), Products)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("CompanyName", CompanyName)
            , new("PageTitle", PageTitle)
        };

        CreatePreparedReportCommand? command = new()
        {
            Title = PageTitle,
            ReportFileName = "Enter",
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

    public async Task OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Products = null;

        Details = null;

        SearchWarehouses = Warehouses;
    }

    public async Task OnActionTypeChange(object e)
    {
        if (e is null)
        {
            SearchWarehouses = Warehouses;

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

        Request.DestinationCode = string.Empty;

        SearchWarehouses = items;

        WarehousesDrop.Rebind();

        IsLoading = false;
    }

    private GetAllEnterProductQuery FixEmptiness()
    {
        GetAllEnterProductQuery search = new();

        search.Shift = "-1";
        if (Request.ProductName.HasValue())
        {
            search.ProductName = Request.ProductName;
        }
        else
        {
            search.ProductName = "-1";
        }

        if (Request.ProductCode.HasValue())
        {
            search.ProductCode = Request.ProductCode;
        }
        else
        {
            search.ProductCode = "-1";
        }

        if (Request.FromDate.HasValue())
        {
            search.FromDate = Request.FromDate;
        }
        else
        {
            search.FromDate = "-1";
        }

        if (Request.ToDate.HasValue())
        {
            search.ToDate = Request.ToDate;
        }
        else
        {
            search.ToDate = "-1";
        }

        if (Request.ProductSerial.HasValue())
        {
            search.ProductSerial = Request.ProductSerial;
        }
        else
        {
            search.ProductSerial = "-1";
        }

        if (Request.TechnicalCode.HasValue())
        {
            search.TechnicalCode = Request.TechnicalCode;
        }
        else
        {
            search.TechnicalCode = "-1";
        }

        search.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (Request.Size.HasValue())
        {
            search.Size = Request.Size;
        }
        else
        {
            search.Size = "-1";
        }

        if (Request.Qc.HasValue())
        {
            search.Qc = Request.Qc;
        }
        else
        {
            search.Qc = "-1";
        }

        if (Request.DestinationCode.HasValue())
        {
            search.DestinationCode = Request.DestinationCode;
        }
        else
        {
            search.DestinationCode = "-1";
        }

        if (Request.ProductGroup.HasValue())
        {
            search.ProductGroup = Request.ProductGroup;
        }
        else
        {
            search.ProductGroup = "-1";
        }

        if (Request.ProductBrand.HasValue())
        {
            search.ProductBrand = Request.ProductBrand;
        }
        else
        {
            search.ProductBrand = "-1";
        }

        if (Request.ProductType.HasValue())
        {
            search.ProductType = Request.ProductType;
        }
        else
        {
            search.ProductType = "-1";
        }

        if (Request.GateCode.HasValue())
        {
            search.GateCode = Request.GateCode;
        }
        else
        {
            search.GateCode = "-1";
        }

        if (Request.ActionType is not null)
        {
            search.ActionType = Request.ActionType;
        }
       
        return search;
    }

    private bool ValidateSearch()
    {
        if (Request.ActionType is null && Request.DestinationCode.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_ChooseOneFieldRequired,
                                            TextResources.APP_StringKeys_ActionType,
                                            TextResources.APP_StringKeys_Warehouse)
                              , "error");
            return false;
        }

        return true;
    }
}
