using AutoMapper;
using Newtonsoft.Json.Linq;
using Silo.Application;

namespace Silo.Pages.Location;
public partial class Collect
{
    public bool IsLoading = true;
    public bool IsShowAddValidationMessage = false;
    public DocumentKeyTypeDto DocumentKeyTypeRequest = new();
    public string DeleteProductCode = string.Empty;
    public string DeleteLocation = string.Empty;
    public string Desc = string.Empty;
    public List<GateProductDto> Docs = new();
    public List<GateProductDto> Plans = new();
    public List<GetAllZonesVm> Docks;
    public List<GetAllTruckQuery> Trucks;
    public List<GetAllZonesVm> Zones;
    public string AllSelectedTruck;
    public string AllSelectedDock;
    public string SearchProductCode;
    public string SearchZone;
    public string SearchCount;
    public string SearchError;
    public string MessageModal = string.Empty;
    public int RevokePlacementOrder = 0;
    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public string CompanyName;
    public List<PrintableOrderDto> PrintablePlan = new();
    public List<GetAllPlacementOrderVm> PlacementOrders = new();

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Parameter] public string RedirectedDocumentCode { get; set; }
    [Parameter] public string RedirectedDocumentType { get; set; }

    public Modal Modal { get; set; }
    public Modal ModalAdd { get; set; }
    public Modal ModalRevoke { get; set; }
    public Modal ModalHistory { get; set; }

    public TelerikGrid<GateProductDto> PlanGrid { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Docks = ((await Api.PostAsync<List<GetAllZonesVm>>("SPGetAllZones")).Value)
            .Where(p => p.ParentCode == "R").ToList();

        Trucks = (await Api.PostAsyncByContext<List<GetAllTruckQuery>>("SPGetListWarehouseMachines", new GetAllTruckQueryContext())).Value;

        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType",
            new GetAllDocumentTypesVmContext())).Value;

        if (DocumentTypes.Count == 1)
        {
            DocumentKeyTypeRequest.Type = DocumentTypes.FirstOrDefault()?.Code;
        }

        if (RedirectedDocumentCode.HasValue() && RedirectedDocumentType.HasValue())
        {
            DocumentKeyTypeRequest.Key = RedirectedDocumentCode;

            DocumentKeyTypeRequest.Type = RedirectedDocumentType;

            await SearchOnDoc();
        }

        IsLoading = false;
    }

    public async Task SearchOnDoc()
    {
        if (DocumentKeyTypeRequest.Key.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_DocKey), "error");

            return;
        }
        if (DocumentKeyTypeRequest.Type.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Document_Type), "error");

            return;
        }

        IsLoading = true;

        Docs = (await Api.PostAsync<List<GateProductDto>>("SGetCollectProuductsOrderFromStore",
                       new KeyValuePair<string, object>("documentKey", DocumentKeyTypeRequest.Key),
                       new KeyValuePair<string, object>("documentType", DocumentKeyTypeRequest.Type)
                       )).Value;

        if (Docs.Any())
        {
            GateProductDto firstItem = Docs[0];
            
            Desc = $"توضیح سند: {firstItem.Desc}  تاریخ ثبت: {PersianCalendarTools.GregorianToPersian(firstItem.Date) + TextResources.APP_StringKeys_Time + " : " + firstItem.Date.ToShortTimeString()} انبار: {firstItem.StoreCode} - {firstItem.StoreName}";
            
            Docs.First().SumValue = Docs[0].SumValue;

            Plans = new();
        }

        IsLoading = false;
    }

    public async Task SortCollect(MouseEventArgs e)
    {
        if (DocumentKeyTypeRequest.Key.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_DocKey), "error");

            return;
        }
        if (DocumentKeyTypeRequest.Type.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Document_Type), "error");

            return;
        }

        IsLoading = true;

        Plans = (await Api.PostAsync<List<GateProductDto>>("SGetCollectProductList",
                       new KeyValuePair<string, object>("documentKey", DocumentKeyTypeRequest.Key),
                       new KeyValuePair<string, object>("documentType", DocumentKeyTypeRequest.Type)
                       )).Value;

        IsLoading = false;
    }

    public async Task DeletePalnRow(string productCode, string zoneCode)
    {
        DeleteProductCode = productCode;

        DeleteLocation = zoneCode;

        await Modal.Open(new());
    }

    public async Task OnYesModalDeletePlanRow(MouseEventArgs e)
    {
        Plans.RemoveAll(p =>
                        p.ProductCode.Equals(DeleteProductCode)
                     && p.Location.Equals(DeleteLocation));

        PlanGrid.Rebind();

        DeleteProductCode = string.Empty;

        DeleteLocation = string.Empty;
    }

    #region Conflict Handler
    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        GateProductDto item = (GateProductDto)args.Item;

        CollectContradictions st = CheckDocItemContradiction(item);

        if (st != CollectContradictions.NoContradiction)
        {
            args.Class += " bg-warning";
        }
        else
        {
            args.Class = "";
        }
    }

    public void OnRowPlanRenderHandler(GridRowRenderEventArgs args)
    {
        GateProductDto plan = (GateProductDto)args.Item;

        CollectContradictions st = CheckDocItemContradiction(plan);

        if (st != CollectContradictions.NoContradiction)
        {
            args.Class += " bg-warning";
        }
        else
        {
            args.Class = "";
        }
    }

    public CollectContradictions CheckDocItemContradiction(GateProductDto item)
    {
        if (Plans is not null)
        {
            var plans = Plans.Where(plan => plan.ProductCode.Equals(item.ProductCode));
            var sum = plans.Sum(p => p.SumValue);
            if (plans.Any())
            {
                if (!sum.Equals(item.SumValue)) return CollectContradictions.NotCountEquality;
                else return CollectContradictions.NoContradiction;
            }

            return CollectContradictions.NotIn;
        }

        return CollectContradictions.NoContradiction;
    }

    public decimal GetDocContradictionAmount(GateProductDto item)
    {
        var plans = Plans.Where(plan => plan.ProductCode.Equals(item.ProductCode));

        var sum = plans.Sum(p => p.SumValue);

        return item.SumValue - sum;
    }

    public CollectContradictions CheckPlanItemContradiction(GateProductDto item)
    {
        GateProductDto doc = Docs.FirstOrDefault(doc => doc.ProductCode.Equals(item.ProductCode));

        if (doc is not null)
        {
            return CollectContradictions.NoContradiction;
        }

        return CollectContradictions.NotIn;
    }
    #endregion

    public void OnChangeAllTruck(object e)
    {
        if (Plans is not null)
        {
            if ((bool)Plans?.Any(p => p.IsChoosed))
            {
                Plans?.ForEach(plan =>
                {
                    if (plan.IsChoosed)
                    {
                        plan.TruckCode = AllSelectedTruck;
                    }
                });
            }
            else
            {
                Plans?.ForEach(plan => plan.TruckCode = AllSelectedTruck);
            }
        }
    }

    public void OnChangeAllDock(object e)
    {
        if (Plans is not null)
        {
            if ((bool)Plans?.Any(p => p.IsChoosed))
            {
                Plans?.ForEach(plan =>
                {
                    if (plan.IsChoosed)
                    {
                        plan.DockCode = AllSelectedDock;
                    }
                });
            }
            else
            {
                Plans?.ForEach(plan => plan.DockCode = AllSelectedDock);
            }
        }
    }

    public async Task OnClickAddModal(MouseEventArgs e)
    {
        if (!string.IsNullOrEmpty(DocumentKeyTypeRequest.Key))
        {
            IsLoading = true;

            Zones = (await Api.PostAsync<List<GetAllZonesVm>>("SSearchZoneByCodes"
                    , new KeyValuePair<string, object>("codes", Docs.Select(p => p.ProductCode)
                    .Distinct().ToArray()))).Value;

            SearchProductCode = null;

            SearchZone = null;

            SearchCount = null;

            SearchError = string.Empty;

            IsLoading = false;

            await ModalAdd.Open(e);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Collect_EmptyCode, "error");
        }
    }

    public async Task OnModalAddOk(MouseEventArgs e)
    {
        if (string.IsNullOrEmpty(SearchZone)
         || string.IsNullOrEmpty(SearchProductCode)
         || string.IsNullOrEmpty(SearchCount))
        {
            Notification.Show("okjdfjhsadkj", "error");

            SearchError = TextResources.APP_StringKeys_Validation_EmptinessCheck;

            return;
        }

        GateProductDto doc = Docs.FirstOrDefault(p => p.ProductCode.Equals(SearchProductCode));

        Plans.Add(new()
        {
            ProductName = doc.ProductName,
            ProductCode = doc.ProductCode,
            Location = SearchZone,
            CountProduct = SearchCount,
            SumValue = doc.SumValue * int.Parse(SearchCount),
            ProductTechnicalCode = doc.ProductTechnicalCode,
        });

        PlanGrid.Rebind();

        await ModalAdd.Close(e);
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        IsLoading = false;
        IsShowAddValidationMessage = false;
        DocumentKeyTypeRequest.Key = string.Empty;
        DocumentKeyTypeRequest.Type = string.Empty;
        DeleteProductCode = string.Empty;
        Desc = string.Empty;
        Docs = new();
        Plans = new();
        Trucks = null;
        Docks = null;
        Zones = null;
        AllSelectedTruck = null;
        AllSelectedDock = null;
        SearchProductCode = null;
        SearchZone = null;
        SearchCount = null;
        MessageModal = string.Empty;

        if (DocumentTypes.Count == 1)
        {
            DocumentKeyTypeRequest.Type = DocumentTypes.FirstOrDefault()?.Code;
        }

    }

    public async Task OnClickSave(MouseEventArgs e)
    {
        if (Plans.Count == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Collect_EmptyCode, "error");

            return;
        }

        IsLoading = true;

        SavePlacementOrderCollectCommand request = new()
        {
            ProductLine = "0",
            ProductShift = "0",
            StoreCode = "1",
            POCode = "-1",
            FromZoneCode = "0",
            Type = "3",
            DocumentKey = DocumentKeyTypeRequest.Key,
            DocumentType = DocumentKeyTypeRequest.Type,
            CollectPlans = new()
        };

        foreach (GateProductDto plan in Plans)
        {
            List<string> serials = new();

            if (plan.SerialList.Contains(","))
            {
                foreach (string serial in plan.SerialList.Split(','))
                {
                    serials.Add(serial);
                }
            }
            else
            {
                if (plan.SerialList.HasValue())
                {
                    serials.Add(plan.SerialList);
                }
            }

            CollectPlanDto collectPlan = new()
            {
                ProductCode = plan.ProductCode,
                PackCount = plan.CountProduct,
                ZoneList = new List<string>() { plan.Location },
                FromZoneCode = plan.Location,
                Truck = plan.TruckCode,
                Serials = serials,
                SumValue = plan.SumValue,
                PackCountDescription = plan.CountProductDesc
            };

            request.CollectPlans.Add(collectPlan);
        }


        int result = (await Api.PostAsync<int>("SPSavePlacementOrdersBySerialsFromCollect",
            new KeyValuePair<string, object>("order", request))).Value;

        IsLoading = false;

        if (result.Equals(-1))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
    }

    #region Placement Order History And Revoke
    public async Task OnGetPlacementOrdersClick()
    {
        IsLoading = true;

        PlacementOrders = (await Api.PostAsync<List<GetAllPlacementOrderVm>> ("SGetPlacementOrders")).Value;

        await ModalHistory.Open(new());

        IsLoading = false;
    }

    public async Task OnRevokePlacementOrderClick(int operationCode)
    {
        RevokePlacementOrder = operationCode;

        await ModalRevoke.Open(new());

        await ModalHistory.Close(new());
    }

    public async Task OnApproveRevokePlacementOrderClick()
    {
        IsLoading = true;

        await ModalRevoke.Close(new());

        var result = (await Api.PostAsync<int>("SRevokePlacementOrder",
            new KeyValuePair<string, object>("operationCode", RevokePlacementOrder))).Value;

        IsLoading = false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }
    #endregion

    public async Task OnPrintPlanClick()
    {
        IsLoading = true;

        var printableDoc = new DocumentHeaderDto();

        printableDoc = (await Api.PostAsyncByUri<DocumentHeaderDto>("wms/Document", "SGetDocumentHeaderAndItems",
                 new KeyValuePair<string, object>("documentKey", DocumentKeyTypeRequest.Key),
                 new KeyValuePair<string, object>("documentType", DocumentKeyTypeRequest.Type))).Value;

        PrintablePlan = Mapper.Map<List<PrintableOrderDto>>(Plans);

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(PrintableOrderDto), PrintablePlan)
        };

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("DocumentKey", $"{DocumentKeyTypeRequest.Key}")
            , new("DocumentType", $"{DocumentTypes.FirstOrDefault(p=>p.Code.Equals(DocumentKeyTypeRequest.Type))?.Title??string.Empty}")
            , new("Description", $"توضیحات: {Desc}")
            , new("CompanyName", CompanyName)
            , new("PageTitle", PageTitle)
        };

        var headerDatas = JToken.Parse(printableDoc.HeaderData);

        if (headerDatas is not null)
        {
            foreach (JProperty prop in headerDatas)
            {
                variables.Add(new(prop.Name.ToString().Trim().Replace(' ', '_'), prop.Value.ToString()));
            }
        }

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "Collect",
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

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }
}

