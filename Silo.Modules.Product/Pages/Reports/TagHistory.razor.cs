using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Shared.Components;
using Silo.Shared.Components.Print;
using Silo.Shared.Tools;
using Silo.Application.Dto.DynamicField;

namespace Silo.Modules.Product.Pages;
public partial class TagHistory
{
    public bool IsLoading = true;
    public string UserId;
    public string NewProductSerial = string.Empty;
    public string ActiveProductSerial = string.Empty;
    public int ActiveTabIndex = 0;
    public int ActiveStepperIndex = 0;
    public List<GetTagHistoryTimeLineVm> TimeLine = new();
    public List<GetProductInfosBySerialVm> ProductInfos = new();
    public List<GetProductExitInfoBySerialVm> Sales = new();
    public List<GetProductStoreTransactionsBySerialVm> Movements = new();
    public List<GetProductPlacementInfoBySerialVm> Placements = new();
    public List<GetProductInventoryInfoBySerialVm> Inventories = new();
    public List<GetProductReadByGateLogBySerialVm> ReadGates = new();
    public List<GetInspectResultsBySerialVm> Inspects = new();
    public List<GetFreezeHeadersBySerialVm> Freezes = new();
    public List<GetGateAlertsBySerialVm> GateAlerts = new();
    public List<GetProductGuaranteeBySerialVm> Guarantees = new();
    public List<GetProductExpireBySerialVm> Expires = new();
    public List<GetTagRelatedTagsVm> RelatedTags = new();
    public List<GetTablesChangeLogDto> TagChangeLogs = new();
    public JToken TechnicalData;
    public JToken ProductProperties;
    public JToken MovementActionData;
    public List<GetAllInspectElementVm> InspectElements;
    public List<DynamicFieldWithValueDto> DynamicFields = new();
    public List<GetGpsLogDto> GpsLogs = new();

    public Gallery GalleryRef { get; set; }
    public SelectPrintFormat SelectPrintFormatRef { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    [Parameter] public string? ProductSerial { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    public async Task OnPrintActionClick(GetPrintFormatsByPageTitleDto format)
    {
        await PrintTagHistory(format.Path);
    }

    private async Task PrintTagHistory(string reportFileName)
    {
        IsLoading = true;

        var freshData = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

        if (freshData is null || freshData.Neither())
        {
            Notification.Show(TextResources.APP_StringKeys_Message_TagNotFound, "error");
            IsLoading = false;
            return;
        }

        var info = freshData[0];

        string logoPath = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", logoPath)
        };

        string companyName = Configuration["Settings:Company"];

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("CompanyName", companyName)
            , new("ProductName", info.ProductName)
            , new("ProductCode", info.ProductCode)
            , new("ProductCount", info.ProductCount)
            , new("ProductValue", info.ProductValue)
            , new("ProductCountInPack", info.ProductCountInPack)
            , new("ProductPackWeight", info.ProductPackWeight)
            , new("ProductPackVolume", info.ProductPackVolume)
            , new("TagEpc", info.TagEpc)
            , new("TagStatusTitle", info.TagStatusTitle)
            , new("Warehouse", info.Warehouse)
            , new("TagZone", info.TagZone)
            , new("RegisterUserName", info.RegisterUserName)
            , new("ProductSerial", info.ProductSerial)
        };

        if (info.ProductProperties.HasValue())
        {
            var productProperties = JObject.Parse(info.ProductProperties);

            foreach (var property in productProperties.Properties())
            {
                variables.Add(new KeyValuePair<string, object>(
                    property.Name.Trim().Replace(' ', '_'),
                    property.Value?.ToString() ?? string.Empty));
            }
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new("Serials", new List<TelerikDropDownItem>()
            {
                new() { Value = info.ProductSerial }
            })
        };

        CreatePreparedReportCommand command = new()
        {
            Title = PageTitle,
            ReportFileName = reportFileName,
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
            , "PreparedReport/Create"
            , command);

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnOpenGallery()
    {
        await GalleryRef.Show(UserId, GalleryUsageType.Tag, ActiveProductSerial);
    }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        var dynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/document", "SGetDynamicFieldsByActionTypeId",
                             new KeyValuePair<string, object>("actionTypeId", 0))).Value;

        DynamicFields = dynamicFields.DistinctBy(p => p.Title)
                                        .Select(p => new DynamicFieldWithValueDto()
                                        {
                                            Title = p.Title,
                                            DefaultValue = p.DefaultValue,
                                            Value = p.DefaultValue,
                                            ValueOptions = p.ValueOptionList,
                                            ValueType = p.ValueType,
                                            IsReadOnly = true
                                        }).ToList();

        if (ProductSerial.HasValue())
        {
            NewProductSerial = ProductSerial;

            ActiveProductSerial = ProductSerial;

            await GetProductHistory();
        }

        InspectElements = (await Api.PostAsync<List<GetAllInspectElementVm>>("SGetAllElements")).Value;

        IsLoading = false;
    }

    #region Add Serial
    public async Task OnSerialSelected(string serial)
    {
        NewProductSerial = serial;

        ActiveProductSerial = serial;

        OnClearClick(new());

        await GetProductHistory();

        IsFiltersShown = false;
    }

    public async Task OnFilterClear()
    {
        OnClearClick(new());
    }

    #endregion

    #region Events
    public async Task OnStepperChanged(TagEventType eventType)
    {
        ActiveTabIndex = eventType switch
        {
            TagEventType.None => (int)TagHistoryTabs.ProductInfo,
            TagEventType.Product => (int)TagHistoryTabs.ProductionHistory,
            TagEventType.Inspect => (int)TagHistoryTabs.InspectAndFreeze,
            TagEventType.Freeze => (int)TagHistoryTabs.InspectAndFreeze,
            TagEventType.Movement => (int)TagHistoryTabs.WarehouseTransactions,
            TagEventType.Inventory => (int)TagHistoryTabs.Inventory,
            TagEventType.Placement => (int)TagHistoryTabs.Placement,
            TagEventType.Gate => (int)TagHistoryTabs.ReadedGate,
            TagEventType.Revoke => (int)TagHistoryTabs.ProductionHistory,
            TagEventType.GateAlert => (int)TagHistoryTabs.GateAlert,
            TagEventType.Sell => (int)TagHistoryTabs.SalesHistory,
            TagEventType.Guarantee => (int)TagHistoryTabs.Guarantee,
            TagEventType.Expire => (int)TagHistoryTabs.Expire
        };

        await GetProductHistory();
    }

    public async Task OnTabChanged(int newIndex)
    {
        ActiveTabIndex = newIndex;

        await GetProductHistory();
    }

    public void OnClearClick(MouseEventArgs e)
    {
        ActiveTabIndex = 0;
        TimeLine = new();
        ProductInfos = new();
        Sales = new();
        Movements = new();
        Placements = new();
        Inventories = new();
        ReadGates = new();
        Inspects = new();
        Freezes = new();
        GateAlerts = new();
        TechnicalData = null;
        ProductProperties = null;
        MovementActionData = null;
        Guarantees = new();
        Expires = new();
        RelatedTags = new();
        TagChangeLogs = new();

    }

    public void OnRowGuaranteeRenderHandler(GridRowRenderEventArgs args)
    {
        GetProductGuaranteeBySerialVm item = (GetProductGuaranteeBySerialVm)args.Item;

        if (item.GuaranteeStatus == 1)
        {
            args.Class = " bg-success";
        }
        else if (item.GuaranteeStatus == 2)
        {
            args.Class = " bg-danger";
        }
        else
        {
            args.Class = "";
        }

    }

    public void OnRowExpireRenderHandler(GridRowRenderEventArgs args)
    {
        GetProductExpireBySerialVm item = (GetProductExpireBySerialVm)args.Item;

        if (item.ExpireStatus == 1)
        {
            args.Class = " bg-success";
        }
        else if (item.ExpireStatus == 2)
        {
            args.Class = " bg-danger";
        }
        else
        {
            args.Class = "";
        }
    }
    #endregion

    #region Private
    private async Task GetProductHistory()
    {
        IsLoading = true;

        if (TimeLine.Count == 0)
        {
            var result = (await Api.PostAsyncByContext<List<GetTagHistoryTimeLineVm>>("SGetTagHistoryTimeLine"
                , new GetTagHistoryTimeLineVmContext()
                , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

            if (result.Any())
            {
                TimeLine = result.OrderByDescending(p => p.TagEventDateTime).ToList();

                if (result[0].TagEventType == TagEventType.Revoke)
                {
                    await GetRevokedData();
                }
            }
            else
            {
                await GetRevokedData();

                Notification.Show(TextResources.APP_StringKeys_Validation_NotFound, "error");

                IsLoading = false;

                return;
            }
        }

        if (TimeLine.Any())
        {
            switch ((TagHistoryTabs)ActiveTabIndex)
            {
                case TagHistoryTabs.ProductInfo:

                    await GetProductInfos();
                    break;

                case TagHistoryTabs.ProductProperty:

                    await GetProductInfos();
                    break;

                case TagHistoryTabs.ProductionHistory:

                    await GetProductInfos();
                    break;

                case TagHistoryTabs.SalesHistory:

                    await GetSales();
                    break;

                case TagHistoryTabs.WarehouseTransactions:

                    await GetMovements();
                    break;

                case TagHistoryTabs.Placement:

                    await GetPlacements();
                    break;

                case TagHistoryTabs.Inventory:

                    await GetInventories();
                    break;

                case TagHistoryTabs.ReadedGate:

                    await GetReadGates();
                    break;

                case TagHistoryTabs.InspectAndFreeze:

                    await GetInspects();
                    await GetFreezes();
                    break;

                case TagHistoryTabs.GateAlert:
                    await GetGateAlerts();
                    break;

                case TagHistoryTabs.Guarantee:
                    await GetGuarantees();
                    break;

                case TagHistoryTabs.Expire:
                    await GetExpires();
                    break;

                case TagHistoryTabs.HistoryDetails:
                    await GetTagChangeLogs();
                    break;

                case TagHistoryTabs.GpsLog:
                    await GetGpsLogs();
                    break;
            }
        }

        IsLoading = false;
    }

    private async Task GetProductInfos()
    {
        if (ProductInfos.Neither())
        {
            ProductInfos = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
                            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

            if (ProductInfos.Any())
            {
                if (ProductInfos[0].ProductTechnicalData.HasValue())
                {
                    TechnicalData = JToken.Parse(ProductInfos[0].ProductTechnicalData);
                }

                if (ProductInfos[0].ProductProperties.HasValue())
                {
                    ProductProperties = JToken.Parse(ProductInfos[0].ProductProperties);

                    foreach (var dynamicFields in DynamicFields)
                    {
                        var value = ProductProperties.Value<string?>(dynamicFields.Title);

                        if (value.HasValue())
                        {
                            dynamicFields.Value = value;
                        }
                        else
                        {
                            dynamicFields.Value = string.Empty;
                        }
                    }
                }

                if (ProductInfos[0].ProductGalleryId.NotEquals(0))
                {
                    var imageBytes = await Api.PostAsync("Gallery/GetGalleryImageFile",
                        new GetGalleryImageFileQuery()
                        {
                            Id = ProductInfos[0].ProductGalleryId
                        });

                    if (imageBytes is not null)
                    {
                        ProductInfos[0].ImageBase64 = ImageTools.ConvertImageByteToBase64String(imageBytes);
                    }

                }
            }
        }

        if (RelatedTags.Neither())
        {
            RelatedTags = (await Api.PostAsyncByContext<List<GetTagRelatedTagsVm>>("SGetTagRelatedTags"
                , new GetTagRelatedTagsVmContext()
                , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value ?? new();
        }
    }
    #endregion

    #region Get Data Part Timeline
    private async Task GetMovements()
    {
        if (Movements.Any())
        {
            return;
        }

        Movements = (await Api.PostAsyncByContext<List<GetProductStoreTransactionsBySerialVm>>("SGetTagHistoryMovement"
            , new GetProductStoreTransactionsBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetSales()
    {
        if (Sales.Any())
        {
            return;
        }

        Sales = (await Api.PostAsyncByContext<List<GetProductExitInfoBySerialVm>>("SGetTagSalesHistory"
            , new GetProductExitInfoBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

        if (Sales.Neither())
        {
            return;
        }

        if (Sales[0].MovementActionData.HasValue())
        {
            MovementActionData = JToken.Parse(Sales[0].MovementActionData);
        }
    }

    private async Task GetPlacements()
    {
        if (Placements.Any())
        {
            return;
        }

        Placements = (await Api.PostAsyncByContext<List<GetProductPlacementInfoBySerialVm>>("SGetTagPlacementHistory"
            , new GetProductPlacementInfoBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetInventories()
    {
        if (Inventories.Any())
        {
            return;
        }

        Inventories = (await Api.PostAsyncByContext<List<GetProductInventoryInfoBySerialVm>>("SGetTagInventoryHistory"
            , new GetProductInventoryInfoBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetReadGates()
    {
        if (ReadGates.Any())
        {
            return;
        }

        ReadGates = (await Api.PostAsyncByContext<List<GetProductReadByGateLogBySerialVm>>("SGetTagHistoryReadByGate"
            , new GetProductReadByGateLogBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetInspects()
    {
        if (Inspects.Any())
        {
            return;
        }

        Inspects = (await Api.PostAsyncByContext<List<GetInspectResultsBySerialVm>>("SReportInspects"
            , new GetInspectResultsBySerialVmContext()
            , new KeyValuePair<string, object>("serial", new GetAllInspectReportQuery()
            {
                ProductCode = "-1",
                ElementFilters = new(),
                FromDate = "-1",
                ToDate = "-1",
                ProductSerial = ActiveProductSerial,
                InspectResult = -1,
                Line = "-1",
                RegCode = "-1",
                UserId = "-1"
            }))).Value;

    }

    private async Task GetFreezes()
    {
        if (Freezes.Any())
        {
            return;
        }

        Freezes = (await Api.PostAsyncByContext<List<GetFreezeHeadersBySerialVm>>("SGetFreezeHeadersBySerial"
            , new GetFreezeHeadersBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetGateAlerts()
    {
        if (GateAlerts.Any())
        {
            return;
        }

        GateAlerts = (await Api.PostAsyncByContext<List<GetGateAlertsBySerialVm>>("SGetGateAlertsBySerialNew"
            , new GetGateAlertsBySerialVmContext()
            , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetGuarantees()
    {
        if (Guarantees.Any())
        {
            return;
        }

        Guarantees = (await Api.PostAsyncByUriAndContext<List<GetProductGuaranteeBySerialVm>>("wms/Product",
                    "SGetProductGuaranteeBySerial",
                    new GetProductGuaranteeBySerialVmContext(),
                    new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetExpires()
    {
        if (Expires.Any())
        {
            return;
        }

        Expires = (await Api.PostAsyncByUriAndContext<List<GetProductExpireBySerialVm>>("wms/Product",
                        "SGetProductExpireBySerial",
                        new GetProductExpireBySerialVmContext(),
                        new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;
    }

    private async Task GetTagChangeLogs()
    {
        if (TagChangeLogs.Any())
        {
            return;
        }

        TagChangeLogs = (await Api.SendAsyncObjectByUri<GetTablesChangeLogVm>(HttpMethod.Get
            , "TablesChangeLog/ReadAll"
            , new GetAllTagChangeLogQuery()
            {
                TableName = "tbl_Tags",
                RecordKey = ActiveProductSerial
            }
            , new GetTablesChangeLogVmContext())).Value.List ?? new();
    }

    private async Task GetRevokedData()
    {
        var printSoftDeleted = (await Api.PostAsync<PrintReportVm?>("SGetRevokedPrint"
                        , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

        if (printSoftDeleted is not null)
        {
            IsLoading = false;

            StateHasChanged();

            await Dialog.AlertAsync($$"""
                اطلاعات سریال '{{printSoftDeleted.ProductSerial}}'
                در تاریخ '{{printSoftDeleted.SoftDeleteDate}}' 
                توسط کاربر '{{printSoftDeleted.SoftDeleteUser}}' باطل شده است
                """ , "ابطال اطلاعات");
        }
    }

    private async Task GetGpsLogs()
    {
        if (GpsLogs.Any())
        {
            return;
        }

        GpsLogs = (await Api.SendAsyncObjectByUri<GetGpsLogVm?>(HttpMethod.Post
            , "GpsLog/GetGpsLog"
            , new GetGpsLogQuery()
            {
                UsageId = ActiveProductSerial
            })).Value.List ?? new();
    }
    #endregion

    #region Show Tag Change Log
    public string ToReadableTagChange(string? jsonRaw)
    {
        if (jsonRaw.HasNoValue())
        {
            return "بدون جزئیات";
        }

        jsonRaw = jsonRaw.Trim();

        if (!jsonRaw.StartsWith("{"))
        {
            return jsonRaw;
        }

        var json = JObject.Parse(jsonRaw);
        var action = json["action"]?.ToString();

        if (action == "INSERT")
        {
            return "ثبت اولیه تگ در سیستم";
        }

        if (action == "DELETE")
        {
            return "حذف رکورد تگ";
        }

        if (action == "UPDATE")
        {
            var changes = json["changes"] as JObject;

            if (changes is null || !changes.HasValues)
            {
                return "ویرایش اطلاعات تگ";
            }

            List<string> parts = new();

            foreach (var prop in changes.Properties())
            {
                var fieldFa = MapFieldToPersian(prop.Name);

                var oldVal = prop.Value?["oldValue"]?.ToString();

                var newVal = prop.Value?["newValue"]?.ToString();

                oldVal = NormalizeValue(oldVal, prop.Name);

                newVal = NormalizeValue(newVal, prop.Name);

                if (oldVal == newVal)
                {
                    continue;
                }

                parts.Add($"{fieldFa}: {oldVal} → {newVal}");
            }

            if (parts.Neither())
            {
                return "ویرایش اطلاعات (جزئیات قابل نمایش نیست)";
            }

            return "ویرایش شده: " + string.Join(" | ", parts);
        }

        return string.Empty;
    }


    public List<TablesChangeLogDto> GetUpdateRows(string? jsonRaw)
    {
        var rows = new List<TablesChangeLogDto>();

        if (string.IsNullOrWhiteSpace(jsonRaw))
            return rows;

        jsonRaw = jsonRaw.Trim();
        if (!jsonRaw.StartsWith("{"))
            return rows;

        try
        {
            var json = JObject.Parse(jsonRaw);
            var action = json["action"]?.ToString();

            if (action != "UPDATE")
                return rows;

            var changes = json["changes"] as JObject;
            if (changes == null || !changes.HasValues)
                return rows;

            foreach (var prop in changes.Properties())
            {
                if (prop.Name == "ProductProperties")
                {
                    var oldRaw = prop.Value?["oldValue"]?.ToString();

                    var newRaw = prop.Value?["newValue"]?.ToString();

                    rows.AddRange(GetProductPropertiesRows(oldRaw, newRaw));

                    continue;
                }

                var fieldFa = MapFieldToPersian(prop.Name);

                var oldVal = NormalizeValue(prop.Value?["oldValue"]?.ToString(), prop.Name);

                var newVal = NormalizeValue(prop.Value?["newValue"]?.ToString(), prop.Name);

                if (oldVal == newVal)
                {
                    continue;
                }

                rows.Add(new TablesChangeLogDto
                {
                    Field = fieldFa,
                    OldValue = oldVal,
                    NewValue = newVal
                });
            }

            return rows;
        }
        catch
        {
            return new List<TablesChangeLogDto>();
        }
    }

    public string? GetAction(string? jsonRaw)
    {
        if (string.IsNullOrWhiteSpace(jsonRaw))
        {
            return null;
        }

        jsonRaw = jsonRaw.Trim();

        if (!jsonRaw.StartsWith("{"))
        {
            return null;
        }

        var json = JObject.Parse(jsonRaw);

        return json["action"]?.ToString();
    }

    public string GetActionTitle(string? jsonRaw)
    {
        var action = GetAction(jsonRaw);

        return action switch
        {
            "INSERT" => "ایجاد",
            "UPDATE" => "ویرایش",
            "DELETE" => "حذف",
            _ => "نامشخص"
        };
    }

    private List<TablesChangeLogDto> GetProductPropertiesRows(string? oldJson, string? newJson)
    {
        var rows = new List<TablesChangeLogDto>();

        var oldDict = JsonTools.ParseToDict(oldJson);

        var newDict = JsonTools.ParseToDict(newJson);

        var allKeys = oldDict.Keys.Union(newDict.Keys).ToList();

        foreach (var key in allKeys)
        {
            oldDict.TryGetValue(key, out var oldValRaw);

            newDict.TryGetValue(key, out var newValRaw);

            var oldVal = NormalizeValue(oldValRaw);

            var newVal = NormalizeValue(newValRaw);

            if (oldVal == newVal)
            {
                continue;
            }

            if (newVal == "خالی")
            {
                continue;
            }

            rows.Add(new TablesChangeLogDto
            {
                Field = key,
                OldValue = oldVal,
                NewValue = newVal
            });
        }

        return rows;
    }

    private string MapFieldToPersian(string field) => field switch
    {
        "ProductSerial" => "سریال محصول",
        "NewProductSerial" => "سریال جدید",
        "ProductCode" => "کد محصول",
        "ProductName" => "نام محصول",
        "ProductType" => "نوع محصول",

        "TagEpc" => "EPC تگ",
        "TagEpc2" => "EPC تگ (۲)",
        "ProjectCode" => "کد پروژه",
        "ProductCount" => "تعداد",

        "ProductStatus" => "وضعیت محصول",
        "TagStatus" => "وضعیت تگ",
        "TagRegisterShamsiUnixDate" => "تاریخ ثبت",
        "TagRegisterDateTime" => "تاریخ ثبت",
        "TagRegisterUser" => "کاربر ثبت‌کننده",

        "TagTreeParentId" => "شناسه والد درخت",
        "TagTreeSecondParentId" => "شناسه والد دوم درخت",
        "TagTreeParentsId" => "شناسه والدهای درخت",
        "TagTreeParentsEpc" => "EPC والدهای درخت",
        "TagTreeParentSerial" => "سریال والد درخت",

        "ProductProperties" => "ویژگی‌های محصول",
        "fld_LastInspectResult" => "نتیجه آخرین بازرسی",

        "Lock" => "قفل",
        "Username" => "نام کاربری",
        "DeviceId" => "شناسه دستگاه",
        "DeviceIp" => "IP دستگاه",

        "Freeze" => "فریز",
        "Deactivate" => "غیرفعال",

        "TagInActionId" => "شناسه اقدام (۱)",
        "TagInDestinationId" => "شناسه مقصد (۱)",
        "TagInActionId2" => "شناسه اقدام (۲)",
        "TagInDestinationId2" => "شناسه مقصد (۲)",

        "fld_ProductPropertyAId" => "خط تولید",
        "fld_ProductPropertyBId" => "شیفت",
        "fld_ProductPropertyCId" => "سایز",

        "RegCode" => "کد رجیستر",
        "fld_LastModifierUser" => "آخرین کاربر ویرایش‌کننده",
        "ContractStatus" => "وضعیت قرارداد",
        "TagZone" => "زون/ناحیه تگ",
        "ReProduct" => "محصول مجدد",
        "fld_ProductGroup" => "گروه محصول",
        "fld_ProductBrand" => "برند محصول",
        "fld_InspectActionId" => "شناسه اقدام بازرسی",
        "fld_ProductSubGroup" => "زیرگروه محصول",
        "fld_ProductClass" => "کلاس محصول",
        "Temp" => "دمـا",
        _ => field
    };

    private static string NormalizeValue(string? value, string? fieldName = null)
    {
        if (value.HasNoValue())
        {
            return "خالی";
        }

        value = value.Trim();

        if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
        {
            return "خالی";
        }

        HashSet<string> boleanFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "Lock",
            "Freeze",
            "Deactivate",
            "ReProduct"
        };

        if (fieldName.HasValue() && boleanFields.Contains(fieldName))
        {
            var normalized = value.ToLowerInvariant();

            if (normalized is "1" or "true")
            {
                return "فعال";
            }

            if (normalized is "0" or "false")
            {
                return "غیرفعال";
            }
        }

        if (decimal.TryParse(value, out var dec))
        {
            return dec.ToString("0.##");
        }

        if (DateTime.TryParse(value, out var dt))
        {
            return PersianCalendarTools.GregorianToPersian(dt) + " " + dt.ToString("HH:mm");
        }

        return value;
    }

    #endregion
}
