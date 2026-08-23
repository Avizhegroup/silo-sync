using System.Text.Json;
using AutoMapper;

namespace Silo.Pages.Reports;

public partial class UhfLog
{
    public bool IsLoading = true;
    public string UserId;
    public List<GetUhfLogReportVm> UhfLogs;
    public List<GetUhfLogReportSerialsVm> DetailSerials;
    public List<GetUhfLogReportProductsVm> DetailProducts;
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<UserDropDownableDto> Users;
    public List<GetAllStationsVm> Station;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    public Modal ModalDetailSerials { get; set; }
    public Modal ModalDetailProducts { get; set; }
    public Modal FiltersModal { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
        new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = Mapper.Map<List<ApplicationUser>, List<UserDropDownableDto>>(
                applicationUsers.Where(p => p.IsActive).ToList());

        Station = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
       , new GetAllStationsVmContext())).Value;

        InitFilters();

        IsLoading = false;
    }

    #region Events
    public async Task OnSearchClick(MouseEventArgs e)
    {
        IsLoading = true;

        var filters = ApplyFilters.GroupBy(p => p.FieldName)
                              .Select(p => new ReportFilter()
                              {
                                  FieldName = p.Key,
                                  Type = p.First().Type,
                                  Component = p.First().Component,
                                  EqualityType = p.First().EqualityType,
                                  AddType = p.First().AddType,
                                  Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                              }).ToList();

        UhfLogs = (await Api.PostAsyncByOption<List<GetUhfLogReportVm>>("SReportUhfLog",
            new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.WriteAsString
            },
            new KeyValuePair<string, object>("reportFilters", filters))).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        UhfLogs = new();

        DetailSerials = null;

        DetailProducts = null;
    }
    #endregion

    #region Details
    public async Task OnClickRowSerials(GetUhfLogReportVm uhfLog)
    {
        IsLoading = true;

        var filters = ApplyFilters
            .GroupBy(p => p.FieldName)
            .Select(p => new ReportFilter()
            {
                FieldName = p.Key,
                Type = p.First().Type,
                Component = p.First().Component,
                EqualityType = p.First().EqualityType,
                AddType = p.First().AddType,
                Values = p.SelectMany(q =>
                        q.Values ?? new List<string>() { q.Value })
                    .Distinct()
                    .ToList()
            }).ToList();

        filters.RemoveAll(p =>
            p.FieldName.Equals("InventoryId") ||
            p.FieldName.Equals("ActionStatus") ||
            p.FieldName.Equals("Station"));

        filters.Add(new ReportFilter()
        {
            FieldName = "InventoryId",
            Type = FilterType.Static,
            Values = new()
        {
            uhfLog.InventoryId
        }
        });

        filters.Add(new ReportFilter()
        {
            FieldName = "ActionStatus",
            Type = FilterType.Static,
            Values = new()
        {
            uhfLog.ActionStatus
        }
        });

        filters.Add(new ReportFilter()
        {
            FieldName = "Station",
            Type = FilterType.Static,
            Values = new()
        {
            uhfLog.ReaderGateCode
        }
        });

        DetailSerials =
            (await Api.PostAsync<List<GetUhfLogReportSerialsVm>>(
                "SReportUhfLogBySerials",
                new KeyValuePair<string, object>(
                    "reportFilters",
                    filters))).Value;

        await ModalDetailSerials.Open(new());

        IsLoading = false;
    }

    public async Task OnClickRowProducts(GetUhfLogReportVm uhfLog)
    {
        IsLoading = true;

        var filters = ApplyFilters
            .GroupBy(p => p.FieldName)
            .Select(p => new ReportFilter()
            {
                FieldName = p.Key,
                Type = p.First().Type,
                Component = p.First().Component,
                EqualityType = p.First().EqualityType,
                AddType = p.First().AddType,
                Values = p.SelectMany(q =>
                        q.Values ?? new List<string>() { q.Value })
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct()
                    .ToList()
            })
            .ToList();

        filters.RemoveAll(p =>
            p.FieldName == "InventoryId" ||
            p.FieldName == "ActionStatus" ||
            p.FieldName == "Station");

        if (!string.IsNullOrWhiteSpace(uhfLog.InventoryId)
            && uhfLog.InventoryId != "0")
        {
            filters.Add(new ReportFilter()
            {
                FieldName = "InventoryId",
                Type = FilterType.Static,
                SqlWhereCommand = "UHF.fld_InventoryId",
                Values = new()
            {
                uhfLog.InventoryId
            }
            });
        }


        if (!string.IsNullOrWhiteSpace(uhfLog.ReaderGateCode))
        {
            filters.Add(new ReportFilter()
            {
                FieldName = "Station",
                Type = FilterType.Static,
                SqlWhereCommand = "UHF.fld_Reader_Gate",
                Values = new()
            {
                uhfLog.ReaderGateCode
            }
            });
        }

        filters.Add(new ReportFilter()
        {
            FieldName = "ActionStatus",
            Type = FilterType.Static,
            SqlWhereCommand = "UHF.ActionStatus",
            Values = new()
        {
            string.IsNullOrWhiteSpace(uhfLog.ActionStatus)
                ? ""
                : uhfLog.ActionStatus
        }
        });

        DetailProducts =
            (await Api.PostAsync<List<GetUhfLogReportProductsVm>>(
                "SReportUhfLogByProducts",
                new KeyValuePair<string, object>(
                    "reportFilters",
                    filters)))
            .Value;

        await ModalDetailProducts.Open(new());

        IsLoading = false;
    }

    #endregion

    #region Dynamic Filters
    public async void OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterModalClick(MouseEventArgs e)
    {
        Filters = new();

        InitFilters();

        await FiltersModal.Open(e);
    }
    #endregion

    private void InitFilters()
    {
        Filters.Clear();

        int indexer = 0;

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_FromDate,
            Component = FilterComponent.PersianDate,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "FromDate",
            EqualityType = FilterEqualityType.BiggerThan,
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ToDate,
            Component = FilterComponent.PersianDate,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "ToDate",
            EqualityType = FilterEqualityType.SmallerThan,
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_FromTime,
            Component = FilterComponent.Time,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "FromTime",
            EqualityType = FilterEqualityType.BiggerThan,
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ToTime,
            Component = FilterComponent.Time,
            Type = FilterType.Static,
            IsLikeCheckboxShown = false,
            FieldName = "ToTime",
            EqualityType = FilterEqualityType.SmallerThan,
        });


        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_RegUser,
            Component = FilterComponent.Drop,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "RegUser",
            Items = Users.Select(p => new ReportDataItem()
            {
                Label = p.Name,
                Value = p.Id
            }).ToList()
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductCode,
            Component = FilterComponent.ProductCodeModal,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductCode"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ProductSerial,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            IsLikeCheckboxShown = true,
            FieldName = "ProductSerial"
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_OperationCode,
            Component = FilterComponent.Text,
            Type = FilterType.Static,
            FieldName = "InventoryId",
        });

        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_ActionStatus,
            Component = FilterComponent.Drop,
            Type = FilterType.Static,
            FieldName = "ActionStatus",
            Items = GetActionStatusItems()
        });


        Filters.Add(new()
        {
            Id = indexer++,
            Label = TextResources.APP_StringKeys_Station,
            Component = FilterComponent.Drop,
            Type = FilterType.Static,
            FieldName = "Station",
            Items = Station.Select(p => new ReportDataItem
            {
                Label = p.Name,
                Value = p.Code,
                IsChoosen = false
            }).ToList()
        });
    }

    private static List<ReportDataItem> GetActionStatusItems() => new()
    {
        new() { Label = "بدون مشکل", Value = ((int)UhflogActionStatusTitleEnum.Ok).ToString() },
        new() { Label = "ناموجود در مبدا", Value = ((int)UhflogActionStatusTitleEnum.NotInSource).ToString() },
        new() { Label = "رجیستر نشده", Value = ((int)UhflogActionStatusTitleEnum.NotRegistered).ToString() },
        new() { Label = "فریز شده", Value = ((int)UhflogActionStatusTitleEnum.Frozen).ToString() },
        new() { Label = "کنترل کیفیت مردود شده", Value = ((int)UhflogActionStatusTitleEnum.QcFailed).ToString() },
        new() { Label = "بازرسی نشده", Value = ((int)UhflogActionStatusTitleEnum.NotInspected).ToString() },
        new() { Label = "نادیده گرفتن", Value = ((int)UhflogActionStatusTitleEnum.Ignored).ToString() },
    };
}
