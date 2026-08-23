using AutoMapper;
using System.Text.Json;
using System.Collections.Concurrent;
using Silo.Application;

namespace Silo.Pages.Reports;
public partial class InventoryConflicts
{
    public bool IsLoading = true;
    public string seriesLabelTamplateNoPercent = "#=category#\n #=value#";
    public ChartSeriesLabelsPosition seriesLabelPosition = ChartSeriesLabelsPosition.Right;
    public string Username;
    public bool IsDetailsShown = false;
    public bool IsAllDetailsShown = false;
    public GetInventoryConflictsVm ProductForDetails = new();
    public GetInventoryConflictsVm ProductFixConflicts = new();
    public List<GetAllProductQcsVm> Qcs;
    public List<UserDropDownableDto> Users;
    public List<GetAllProductTypeVm> Types;
    public List<GetAllWarehousesVm> Warehouses;
    public List<ReportDataItem> WarehouseItems = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public GetInventoryConflictsQuery Request = new();
    public List<GetInventoryConflictDetailsVm> Details;
    public List<GetInventoryConflictDetailsVm> FilteredDetails;
    public SearchInventoryConflictDetailsDto SearchDetails = new();
    public List<GetInventoryConflictDetailsVm> AllDetails;
    public List<GetWarehouseProductsVm> TagsInWarehouse;
    public List<GetWarehouseProductsVm> Accounting;
    public List<GetInventoryResultTagVm> TagsReadedInInventory;
    public List<GetInventoryConflictsVm> Result = new();
    public SaveFixedConflictsCommand FixConflictCommand = new();
    public bool IsAllProductConflictSelected = false;
    public GetEnterAndExitProductValueVm EnterExitValue = new();
    public List<TelerikDropDownItemGeneric<decimal>> ChartValues = new();
    public long MaxAllowedSizeMB = 5;

    #region Summaries
    public int ProductCodeCount = 0;
    public int RfidCount = 0;
    public decimal RfidSumCount = 0;
    public decimal AccountingSumCount = 0;
    public decimal RealitySumCount = 0;
    public int RfidConflictCount = 0;
    public decimal RfidConflictSumCount = 0;
    public decimal AccountingConflictSumCount = 0;
    public decimal RealityConflictSumCount = 0;
    public int InventoryCount = 0;
    public decimal InventorySumCount = 0;
    #endregion      

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ModalDetailsAll { get; set; }
    public Modal ModalFixConflicts { get; set; }
    public Modal ModalWarehouses { get; set; }
    public TelerikGrid<GetInventoryConflictsVm> MainGrid { get; set; }
    public TelerikGrid<GetInventoryConflictDetailsVm> DetailsGridRef { get; set; }
    public LocationModal LocationModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        Username = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Qcs = (await Api.PostAsync<List<GetAllProductQcsVm>>("SGetAllQcs")).Value;

        Sizes = await FormalCache.GetSizes();

        Types = await FormalCache.GetTypes();

        Warehouses = await FormalCache.GetWarehouses();

        WarehouseItems = Warehouses.Select(p => new ReportDataItem()
        {
            IsChoosen = false,
            Label = p.DestinationTitle,
            Value = p.DestinationCode
        }).ToList();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<UserDropDownableDto>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        IsLoading = false;
    }

    public async Task OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        GetInventoryConflictsQuery request = FixEmptiness();

        TagsInWarehouse = new();

        TagsReadedInInventory = new();

        Result = new();

        Accounting = new();

        if (request.IsMovementFiltered)
        {
            var warehouseProducts = (await Api.PostAsync<GetWarehouseProductsListsVm>("SGetFreezedTagProductsBeforeMovements"
                   , new KeyValuePair<string, object>("search", request))).Value;

            var inventoryProducts = (await Api.PostAsync<GetInventoryProductsListsVm>("SGetFreezedInventoryProductsBeforeMovements"
                        , new KeyValuePair<string, object>("search", request))).Value;

            CalculateFreezedLists(warehouseProducts, inventoryProducts);
        }
        else
        {
            TagsInWarehouse = (await Api.PostAsyncByContext<List<GetWarehouseProductsVm>>("SGetProductOfWarehouse"
                   , new GetWarehouseProductsVmContext()
                   , new KeyValuePair<string, object>("search", request))).Value;

            TagsReadedInInventory = (await Api.PostAsyncByContext<List<GetInventoryResultTagVm>>("SSearchInventoryTags"
                    , new GetInventoryResultTagVmContext()
                    , new KeyValuePair<string, object>("search", request))).Value;
        }

        Accounting = (await Api.PostAsyncByContext<List<GetWarehouseProductsVm>>("SSearchLastCAD"
        , new GetWarehouseProductsVmContext()
        , new KeyValuePair<string, object>("search", request))).Value;

        CheckConflicts();

        CalculateSummaries();

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        Request = new();

        Result = null;

        FixConflictCommand = new();

        Result = new();

        Details = null;

        FilteredDetails = null;

        AllDetails = null;

        TagsInWarehouse = null;

        Accounting = null;

        TagsReadedInInventory = null;

        CalculateSummaries(0);

        EnterExitValue = new();
    }

    public async Task OnModalOpenClick(MouseEventArgs e)
    {
        WarehouseItems.ForEach(p => p.IsChoosen = false);

        Request.Warehouse = string.Empty;

        await ModalWarehouses.Open(e);
    }

    public async Task OnModalCheckboxChange(object e)
    {
        var items = WarehouseItems.Where(p => p.IsChoosen).ToList();

        if (items.Any())
        {
            Request.Warehouse = string.Join(',', items.Select(p => p.Value));
        }
        else
        {
            Request.Warehouse = string.Empty;

            WarehouseItems.ForEach(p => p.IsChoosen = false);
        }
    }

    #region Conflicts Check
    public async Task OnUploadRealityCount(IBrowserFile file)
    {
        IsLoading = true;

        var maxAllowedSizeBytes = MaxAllowedSizeMB * 1024 * 1024;

        MemoryStream ms = new();

        await file.OpenReadStream(maxAllowedSizeBytes).CopyToAsync(ms);

        var data = DataTableTools.ReadExcelDataOutDataTable(ms);

        List<SaveNonDocFileCommand> commands = new();

        foreach (var row in data.Select())
        {
            decimal realityCount = 0;

            if (decimal.TryParse(row.ItemArray[1].ToString(), out decimal count))
            {
                realityCount = Math.Round(count, 2);
            }

            GetInventoryConflictsExcelVm accountingData = new()
            {
                ProductCode = row.ItemArray[0].ToString(),
                RealityCount = realityCount,
                InventoryHeaderId = int.Parse(Request.InventoryHeaderId)
            };

            commands.Add(new()
            {
                Data = JsonSerializer.Serialize(accountingData),
                FileName = file.Name,
                Type = (int)NonDocFileTypeEnum.CustomerRealityCountData
            });
        }

        var result = (await Api.PostAsync<bool>("SSaveNdfLog"
            , new KeyValuePair<string, object>("commands", commands))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            IsLoading = false;

            return;
        }

        IsLoading = false;
    }

    public async Task OnUploadAccounting(IBrowserFile file)
    {
        IsLoading = true;

        var maxAllowedSizeBytes = MaxAllowedSizeMB * 1024 * 1024;

        MemoryStream ms = new();

        await file.OpenReadStream(maxAllowedSizeBytes).CopyToAsync(ms);

        var data = DataTableTools.ReadExcelDataOutDataTable(ms);

        List<SaveNonDocFileCommand> commands = new();

        foreach (var row in data.Select())
        {
            decimal sumCount = 0;

            if (decimal.TryParse(row.ItemArray[1].ToString(), out decimal count))
            {
                sumCount = Math.Round(count, 2);
            }

            GetInventoryConflictsExcelVm accountingData = new()
            {
                ProductCode = row.ItemArray[0].ToString(),
                SumCount = sumCount,
                InventoryHeaderId = int.Parse(Request.InventoryHeaderId)
            };

            commands.Add(new()
            {
                Data = JsonSerializer.Serialize(accountingData),
                FileName = file.Name,
                Type = (int)NonDocFileTypeEnum.CustomerAccountingData
            });
        }

        var result = (await Api.PostAsync<bool>("SSaveNdfLog"
            , new KeyValuePair<string, object>("commands", commands))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            IsLoading = false;

            return;
        }

        IsLoading = false;
    }

    public void OnValidationUploadClick()
    {
        Notification.Show(TextResources.APP_StringKeys_Validation_Upload_Inventory_Excels, "error");
    }

    public async Task OnExtraConflictModalOpen(GetInventoryConflictsVm product)
    {
        IsDetailsShown = false;

        IsAllProductConflictSelected = false;

        Details = product.Details
                         .Where(p => p.Status.Contains("اضافی"))
                         .ToList();

        FilteredDetails = Details.ToList();

        ProductFixConflicts = product;

        ProductFixConflicts.IsExtraConflict = true;

        FixConflictCommand = new();

        OnClearModalSearchClick();

        await ModalFixConflicts.Open(new());
    }

    public async Task OnShortageConflictModalOpen(GetInventoryConflictsVm product)
    {
        IsDetailsShown = false;

        IsAllProductConflictSelected = false;

        Details = product.Details
                         .Where(p => p.Status.Contains("کسری"))
                         .ToList();

        FilteredDetails = Details.ToList();

        ProductFixConflicts = product;

        ProductFixConflicts.IsExtraConflict = false;

        FixConflictCommand = new();

        OnClearModalSearchClick();

        await ModalFixConflicts.Open(new());
    }

    public async Task OnFixConflictProductCodeSelect(GetInventoryConflictDetailsVm detail)
    {
        if (detail.IsSelected)
        {
            if (FixConflictCommand.Serials.Any())
            {
                if (!FixConflictCommand.Serials.Any(p => p.Status.Equals(detail.Status)))
                {
                    Notification.Show(TextResources.APP_StringKeys_Validation_InventoryConflicts_OneStatus, "error");

                    detail.IsSelected = false;

                    return;
                }
            }

            FixConflictCommand.Serials.Add(detail);
        }
        else
        {
            FixConflictCommand.Serials.Remove(detail);
        }
    }

    public async Task OnToggleSelectAll()
    {
        if (IsAllProductConflictSelected)
        {
            if (FilteredDetails.Select(p => p.Status)
                      .Distinct()
                      .Count() > 1)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_InventoryConflicts_OneStatus, "error");

                return;
            }
        }

        FilteredDetails.ForEach(p =>
        {
            p.IsSelected = IsAllProductConflictSelected;

            FixConflictCommand.Serials.Add(p);
        });
    }

    public async Task OnFixConflictsClick(MouseEventArgs e)
    {
        if (!IsValid())
        {
            return;
        }

        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SFixInventoryConflicts"
            , new KeyValuePair<string, object>("command", FixConflictCommand))).Value;

        if (result)
        {
            RecalculateFixedConflicts();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await ModalFixConflicts.Close(new());

        IsLoading = false;

        bool IsValid()
        {
            if (!FixConflictCommand.Serials.Any())
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

                return false;
            }

            if (FixConflictCommand.WarehouseCode.HasNoValue())
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination_Warehouse), "error");

                return false;
            }

            return true;
        }

        void RecalculateFixedConflicts()
        {
            foreach (var item in FixConflictCommand.Serials)
            {
                int countDiff = ProductFixConflicts.Count - 1;

                if (countDiff < 0)
                {
                    ProductFixConflicts.Count = 0;
                }
                else
                {
                    ProductFixConflicts.Count = countDiff;
                }

                decimal sumDiff = ProductFixConflicts.SumCount - item.ProductCount;

                if (sumDiff < 0)
                {
                    ProductFixConflicts.SumCount = 0;
                }
                else
                {
                    ProductFixConflicts.SumCount = sumDiff;
                }

                int countConflict = ProductFixConflicts.Count - ProductFixConflicts.CountInventory;

                if (countConflict < 0)
                {
                    ProductFixConflicts.ConflictCount = 0;
                }
                else
                {
                    ProductFixConflicts.ConflictCount = countConflict;
                }

                decimal sumcountConflict = ProductFixConflicts.SumCount - ProductFixConflicts.SumCountInventory;

                if (sumcountConflict < 0)
                {
                    ProductFixConflicts.ConflictSumCount = 0;
                }
                else
                {
                    ProductFixConflicts.ConflictSumCount = sumcountConflict;
                }

                decimal sumcountAccountingConflict = ProductFixConflicts.SumCount - ProductFixConflicts.SumCountAccounting;

                if (ProductFixConflicts.SumCountAccounting == 0 || sumcountAccountingConflict < 0)
                {
                    ProductFixConflicts.ConflictSumCountAccounting = 0;
                }
                else
                {
                    ProductFixConflicts.ConflictSumCountAccounting = sumcountAccountingConflict;
                }

                decimal sumcountRealityConflict = ProductFixConflicts.SumCountReality - ProductFixConflicts.SumCountReality;

                if (ProductFixConflicts.SumCountReality == 0 || sumcountRealityConflict < 0)
                {
                    ProductFixConflicts.ConflictSumCountReality = 0;
                }
                else
                {
                    ProductFixConflicts.ConflictSumCountReality = sumcountRealityConflict;
                }
            }

            Result = Result.Replace(p => p.ProductCode == ProductFixConflicts.ProductCode, ProductFixConflicts);
        }
    }

    public void OnClearModalSearchClick()
    {
        SearchDetails = new();
        FilteredDetails = Details;
    }

    public void OnSearchDetailsModalClick()
    {
        IsLoading = true;

        FilteredDetails = Details.Where(detail =>
            (SearchDetails.ContractStatus.HasNoValue()
            || detail.ContractStatus.Contains(SearchDetails.ContractStatus, StringComparison.OrdinalIgnoreCase)) &&

            (SearchDetails.ProductSerial.HasNoValue()
            || detail.ProductSerial.Contains(SearchDetails.ProductSerial, StringComparison.OrdinalIgnoreCase)) &&

            (SearchDetails.Location.HasNoValue()
            || detail.Zone.Equals(SearchDetails.Location, StringComparison.OrdinalIgnoreCase)) &&

            (SearchDetails.Warehouse.HasNoValue()
            || detail.Place.Equals(SearchDetails.Warehouse, StringComparison.OrdinalIgnoreCase)) &&

            (SearchDetails.FromRegisterDate.HasNoValue()
            || DateTimeTools.ToNormalPersianDate(detail.Date).CompareTo(SearchDetails.FromRegisterDate) >= 0) &&

            (SearchDetails.ToRegisterDate.HasNoValue()
            || DateTimeTools.ToNormalPersianDate(detail.Date).CompareTo(SearchDetails.ToRegisterDate) < 0)
        ).ToList();

        DetailsGridRef.Rebind();

        IsLoading = false;
    }

    public async Task OnChooseDestinationZoneClick(GetAllZonesVm zone)
    {
        SearchDetails.Location = zone.ZoneCode;
    }
    #endregion

    #region Pdf Export
    public async Task OnClickExportToPdfMaster()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetInventoryConflictsVm), Result)
        };

        List<KeyValuePair<string, object>> variables = new()
        {
             new("PageTitle", PageTitle)
           , new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
           , new("ProductCodeCount", ProductCodeCount)
           , new("RfidCount", $"{RfidCount} | {RfidSumCount}")
           , new("AccountingCount", $"{AccountingSumCount}")
           , new("RfidConflict", $"{RfidConflictCount} | {RfidConflictSumCount}")
           , new("ConflictAccounting", $"{AccountingConflictSumCount}")
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "Inventory",
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

    public async Task OnClickExportToPdfDetails()
    {
        IsLoading = true;

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetInventoryConflictDetailsVm), FilteredDetails)
        };

        List<KeyValuePair<string, object>> variables = new()
        {
             new KeyValuePair<string, object>("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}"),
             new KeyValuePair<string, object>("PageTitle", PageTitle)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "InventoryDetails",
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
    #endregion

    #region Row handlers
    public void OnRowRenderHandler(GridRowRenderEventArgs args)
    {
        string classStr = string.Empty;

        GetInventoryConflictsVm item = args.Item as GetInventoryConflictsVm;

        if (item.ConflictCount != 0)
        {
            classStr += "rfid-conf border-bottom";
        }

        if (item.ConflictSumCount != 0)
        {
            if (!classStr.HasValue())
            {
                classStr += "rfid-conf border-bottom";
            }
        }

        if (item.ConflictSumCountAccounting != 0)
        {
            if (!classStr.HasValue())
            {
                classStr += "acc-conf border-bottom";
            }
        }

        args.Class += classStr;
    }

    public void OnDetailsRowRenderHandler(GridRowRenderEventArgs args)
    {
        string classStr = string.Empty;

        var item = args.Item as GetInventoryConflictDetailsVm;

        if (item.Status.Contains("مغایرت"))
        {
            classStr += "rfid-conf border-bottom";
        }

        if (item.Status.Contains("اضافه"))
        {
            classStr += "acc-conf border-bottom";
        }

        args.Class += classStr;
    }
    #endregion

    #region Details
    public async Task OnClickDetails(MouseEventArgs e)
    {
        IsLoading = true;

        IsDetailsShown = false;

        if (IsDetailsShown)
        {
            AllDetails = ProductForDetails.Details;
        }
        else
        {
            AllDetails = ProductForDetails.Details.Where(p => p.Status.Contains("مغایرت")).ToList();
        }

        await ModalDetailsAll.Open(new());

        IsLoading = false;
    }

    public async Task OnClickShowAllDetails(MouseEventArgs e)
    {
        IsLoading = true;

        IsAllDetailsShown = !IsAllDetailsShown;

        if (IsDetailsShown)
        {
            AllDetails = ProductForDetails.Details;
        }
        else
        {
            AllDetails = ProductForDetails.Details.Where(p => p.Status.Contains("مغایرت")).ToList();
        }

        IsLoading = false;
    }
    #endregion

    #region Privates
    private GetInventoryConflictsQuery FixEmptiness()
    {
        GetInventoryConflictsQuery search = new();

        if (Request.ProductCode.HasNoValue())
        {
            search.ProductCode = "-1";
        }
        else
        {
            search.ProductCode = Request.ProductCode;
        }

        if (Request.FromDate.HasNoValue())
        {
            search.FromDate = "-1";
        }
        else
        {
            search.FromDate = Request.FromDate;
        }

        if (Request.ToDate.HasNoValue())
        {
            search.ToDate = "-1";
        }
        else
        {
            search.ToDate = Request.ToDate;
        }

        if (Request.Desc.HasNoValue())
        {
            search.Desc = "-1";
        }
        else
        {
            search.Desc = Request.Desc;
        }

        if (Request.Qc.HasNoValue())
        {
            search.Qc = "-1";
        }
        else
        {
            search.Qc = Request.Qc;
        }

        if (Request.TechnicalCode.HasNoValue())
        {
            search.TechnicalCode = "-1";
        }
        else
        {
            search.TechnicalCode = Request.TechnicalCode;
        }

        search.TechnicalCodeLike = Request.TechnicalCodeLike;

        if (Request.Place.HasNoValue())
        {
            search.Place = "-1";
        }
        else
        {
            search.Place = Request.Place;
        }

        if (Request.User.HasNoValue())
        {
            search.User = "-1";
        }
        else
        {
            search.User = Request.User;
        }

        if (Request.Size.HasNoValue())
        {
            search.Size = "-1";
        }
        else
        {
            search.Size = Request.Size;
        }

        if (Request.Type.HasNoValue())
        {
            search.Type = "-1";
        }
        else
        {
            search.Type = Request.Type;
        }

        if (Request.Warehouse.HasNoValue())
        {
            search.Warehouse = "-1";
        }
        else
        {
            search.Warehouse = Request.Warehouse;
        }

        if (Request.InventoryHeaderId.HasNoValue())
        {
            search.InventoryHeaderId = "-1";
        }
        else
        {
            search.InventoryHeaderId = Request.InventoryHeaderId;
        }

        search.ConflictsShown = Request.ConflictsShown;

        search.IsMovementFiltered = Request.IsMovementFiltered;

        return search;
    }

    private void CalculateSummaries(int? defaultValue = null)
    {
        if (defaultValue is not null)
        {
            ProductCodeCount = (int)defaultValue;
            RfidCount = (int)defaultValue;
            RfidSumCount = (int)defaultValue;
            AccountingSumCount = (int)defaultValue;
            RealitySumCount = (int)defaultValue;
            RfidConflictCount = (int)defaultValue;
            RfidConflictSumCount = (int)defaultValue;
            AccountingConflictSumCount = (int)defaultValue;
            RealityConflictSumCount = (int)defaultValue;
            InventoryCount = (int)defaultValue;
            InventorySumCount = (int)defaultValue;
        }
        else
        {
            ChartValues.Clear();

            ProductCodeCount = Result.Select(p => p.ProductCode).Distinct().Count();
            RfidCount = Result.Sum(p => p.Count);
            RfidSumCount = Result.Sum(p => p.SumCount);
            AccountingSumCount = Result.Sum(p => p.SumCountAccounting);
            RealitySumCount = Result.Sum(p => p.SumCountReality);
            RfidConflictCount = Result.Sum(p => p.ConflictCount);
            RfidConflictSumCount = Result.Sum(p => p.ConflictSumCount);
            AccountingConflictSumCount = Result.Sum(p => p.ConflictSumCountAccounting);
            RealityConflictSumCount = Result.Sum(p => p.ConflictSumCountReality);
            InventoryCount = Result.Sum(p => p.CountInventory);
            InventorySumCount = Result.Sum(p => p.SumCountInventory);

            ChartValues.Add(new()
            {
                Name = "موجودی RFID",
                Value = RfidSumCount
            });

            ChartValues.Add(new()
            {
                Name = "موجودی حسابداری",
                Value = AccountingSumCount
            });

            ChartValues.Add(new()
            {
                Name = "موجودی شمارش فیزیکی",
                Value = RealitySumCount
            });

            ChartValues.Add(new()
            {
                Name = "موجودی انبارگردانی",
                Value = InventorySumCount
            });
        }
    }

    private void CheckConflicts()
     {
        var productCodeToResultMap = new ConcurrentDictionary<string, GetInventoryConflictsVm>(
            Result.GroupBy(p => p.ProductCode).ToDictionary(g => g.Key, g => g.First())
        );
        var productSerialToInventoryMap = new ConcurrentDictionary<string, GetInventoryResultTagVm>(
            TagsReadedInInventory.GroupBy(p => p.ProductSerial).ToDictionary(g => g.Key, g => g.First())
        );

        TagsInWarehouse.AsParallel().ForAll(rfid =>
        {
            var c = productCodeToResultMap.GetOrAdd(rfid.ProductCode, _ => new GetInventoryConflictsVm
            {
                ProductCode = rfid.ProductCode,
                ProductName = rfid.ProductTitle,
                Qc = rfid.Qc,
                ProductSize = rfid.ProductSize,
                Locations = rfid.Zones,
                LocationList = new List<string> { rfid.ThisTagZone },
                TechnicalCode = rfid.RegCode
            });

            if (!c.LocationList.Contains(rfid.ThisTagZone))
            {
                c.LocationList.Add(rfid.ThisTagZone);
            }

            c.Count++;
            c.SumCount += rfid.ProductCount;

            if (productSerialToInventoryMap.TryGetValue(rfid.ProductSerial, out var tagInInventory))
            {
                c.CountInventory++;
                c.SumCountInventory += tagInInventory.ProductCount;
            }
            else
            {
                c.ConflictCount++;
                c.ConflictSumCount -= rfid.ProductCount;
            }

            c.Details.Add(new GetInventoryConflictDetailsVm
            {
                ProductSerial = rfid.ProductSerial,
                ProductCode = rfid.ProductCode,
                ProductName = rfid.ProductTitle,
                Epc = rfid.Epc,
                ProductCount = rfid.ProductCount,
                Zone = rfid.ThisTagZone,
                Date = rfid.Date,
                RegCode = rfid.RegCode,
                ContractStatus = rfid.ContractStatus,
                DestinationTitle = rfid.DestinationTitle,
                Place = rfid.Place,
                Status = tagInInventory == null ? "مغایرت کسری" : string.Empty
            });
        });

        TagsReadedInInventory.AsParallel().ForAll(b =>
        {
            if (TagsInWarehouse.Any(p => p.ProductSerial.Equals(b.ProductSerial)))
            {
                return;
            }

            var mainResult = productCodeToResultMap.GetOrAdd(b.ProductCode, _ => new GetInventoryConflictsVm
            {
                ProductCode = b.ProductCode,
                ProductName = b.ProductName
            });

            mainResult.ConflictCount++;
            mainResult.CountInventory++;
            mainResult.ConflictSumCount += b.ProductCount;
            mainResult.SumCountInventory += b.ProductCount;

            mainResult.Details.Add(new GetInventoryConflictDetailsVm
            {
                ProductSerial = b.ProductSerial,
                ProductCode = b.ProductCode,
                ProductName = b.ProductName,
                ProductCount = b.ProductCount,
                Epc = b.Epc,
                RegCode = b.RegCode,
                Date = b.RegisterDate,
                Zone = b.InventoryZone,
                ContractStatus = b.ContractStatus,
                DestinationTitle = b.DestinationTitle,
                Place = b.Place,
                Status = "مغایرت اضافی"
            });
        });

        if (Accounting.Any())
        {
            Accounting.AsParallel().ForAll(accountingRow =>
            {
                if (!productCodeToResultMap.TryGetValue(accountingRow.ProductCode, out var tagInResult))
                {
                    tagInResult = new GetInventoryConflictsVm
                    {
                        ProductCode = accountingRow.ProductCode,
                        ProductName = accountingRow.ProductTitle,
                        TechnicalCode = accountingRow.RegCode,
                        ProductSize = accountingRow.ProductSize,
                        SumCountAccounting = accountingRow.ProductCount,
                        ConflictSumCountAccounting = accountingRow.ProductCount,
                        ConflictSumCountReality = accountingRow.RealityCount,
                        Details = new List<GetInventoryConflictDetailsVm>
                        {
                            new GetInventoryConflictDetailsVm
                            {
                                ProductCode = accountingRow.ProductCode,
                                ProductName = accountingRow.ProductTitle,
                                ProductCount = accountingRow.ProductCount,
                                RegCode = accountingRow.RegCode,
                                Status = "مغایرت کالایی حسابداری"
                            }
                        }
                    };

                    productCodeToResultMap.TryAdd(tagInResult.ProductCode, tagInResult);

                    return;
                }

                var tagInInventories = TagsReadedInInventory
                    .Where(p => p.ProductCode.Equals(accountingRow.ProductCode))
                    .DistinctBy(p => p.Epc)
                    .ToList();

                if (!tagInInventories.Any())
                {
                    tagInResult.SumCountAccounting += accountingRow.ProductCount;
                    tagInResult.ConflictSumCountAccounting -= accountingRow.ProductCount;
                    tagInResult.Details.ForEach(d => d.Status += $" -  مغایرت کالایی حسابداری");

                    tagInResult.SumCountReality += accountingRow.RealityCount;
                    tagInResult.ConflictSumCountReality -= accountingRow.RealityCount;
                    tagInResult.Details.ForEach(d => d.Status += $" -  مغایرت کالایی شمارش فیزیکی");
                }
                else
                {
                    tagInResult.SumCountAccounting += accountingRow.ProductCount;
                    var diff = tagInInventories.Sum(p => p.ProductCount) - accountingRow.ProductCount;

                    if (diff != 0)
                    {
                        tagInResult.ConflictSumCountAccounting = diff;
                        tagInResult.Details.ForEach(d => d.Status += $" -  مغایرت مقداری حسابداری");
                    }

                    tagInResult.SumCountReality += accountingRow.RealityCount;
                    diff = tagInInventories.Sum(p => p.ProductCount) - accountingRow.RealityCount;

                    if (diff != 0)
                    {
                        tagInResult.ConflictSumCountReality = diff;
                        tagInResult.Details.ForEach(d => d.Status += $" -  مغایرت مقداری حسابداری");
                    }
                }

              
                   
            });
        }

        Result = productCodeToResultMap.Values
            .Where(p => p.ProductCode.HasValue())
            .OrderBy(p => p.ProductCode)
            .ToList();
    }

    private void CalculateFreezedLists(GetWarehouseProductsListsVm warehouseProduct, GetInventoryProductsListsVm inventoryProduct)
    {
        EnterExitValue = new();

        #region Calculate Final List A
        List<GetWarehouseProductsVm> a = new();

        a.AddRange(warehouseProduct.Mains);

        a.AddRange(warehouseProduct.Exits);

        a = a.ExceptBy(warehouseProduct.Enters.Select(p => p.ProductSerial), p => p.ProductSerial).ToList();

        TagsInWarehouse = a;
        #endregion

        #region Calculate Final List B
        List<GetInventoryResultTagVm> b = new();

        b.AddRange(inventoryProduct.Mains);

        b.AddRange(inventoryProduct.Exits);

        b = b.ExceptBy(inventoryProduct.Enters.Select(p => p.ProductSerial), p => p.ProductSerial).ToList();

        TagsReadedInInventory = b;
        #endregion

        #region Calculate ExitValue
        foreach (var warehouseExit in warehouseProduct.Exits)
        {
            var inventoryExit = inventoryProduct.Exits.FirstOrDefault(p => p.Epc == warehouseExit.Epc);

            if (inventoryExit is not null)
            {
                EnterExitValue.ExitCount++;

                EnterExitValue.ExitSumCount += warehouseExit.ProductCount;
            }
            else
            {
                EnterExitValue.ExitCount++;

                EnterExitValue.ExitSumCount += warehouseExit.ProductCount;
            }
        }

        foreach (var inventoryExit in inventoryProduct.Exits)
        {
            var warehouseExit = warehouseProduct.Exits.FirstOrDefault(p => p.Epc == inventoryExit.Epc);

            if (warehouseExit is not null)
            {
                continue;
            }
            else
            {
                EnterExitValue.ExitCount++;

                EnterExitValue.ExitSumCount += inventoryExit.ProductCount;
            }
        }
        #endregion

        #region Calculate EnterValue
        foreach (var warehouseEnter in warehouseProduct.Enters)
        {
            var inventoryEnter = inventoryProduct.Enters.FirstOrDefault(p => p.Epc == warehouseEnter.Epc);

            if (inventoryEnter is not null)
            {
                EnterExitValue.EnterCount++;

                EnterExitValue.EnterSumCount += warehouseEnter.ProductCount;
            }
            else
            {
                EnterExitValue.EnterCount++;

                EnterExitValue.EnterSumCount += warehouseEnter.ProductCount;
            }
        }

        foreach (var inventoryEnter in inventoryProduct.Enters)
        {
            var warehouseEnter = warehouseProduct.Enters.FirstOrDefault(p => p.Epc == inventoryEnter.Epc);

            if (warehouseEnter is not null)
            {
                continue;
            }
            else
            {
                EnterExitValue.EnterCount++;

                EnterExitValue.EnterSumCount += inventoryEnter.ProductCount;
            }
        }
        #endregion
    }
    #endregion
}
