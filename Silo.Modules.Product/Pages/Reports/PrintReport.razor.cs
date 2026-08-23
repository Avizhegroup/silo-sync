using Silo.Application.Dto;
using Silo.Shared.Components;
using Silo.Identity.Client;

namespace Silo.Modules.Product.Pages.Reports;

public partial class PrintReport
{
    private int FilterCount = 1;

    public bool IsLoading = true;
    public List<ReportFilter> Filters = new();
    public List<ReportFilter> ApplyFilters = new();
    public List<ApplicationUser> Users = new();
    public List<PrintReportVm>? Products;

    [Inject] public RfidConnectApi Api { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    public Modal FiltersModal { get; set; } = default!;
    public TelerikGrid<PrintReportVm> Grid { get; set; }

    protected override async Task SiloInitializer()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        Users = applicationUsers.Where(p => p.IsActive).ToList();

        InitFilters();

        IsLoading = false;
    }

    public async Task OnClickSearch(MouseEventArgs e)
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
                                      AdditionalData = p.First().AdditionalData,
                                      Values = p.SelectMany(q => q.Values ?? new List<string>() { q.Value }).Distinct().ToList()
                                  }).ToList();

        Products = (await Api.PostAsync<List<PrintReportVm>>("SReportPrint",
            new KeyValuePair<string, object>[] { new("reportFilters", filters) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnClickClear(MouseEventArgs e)
    {
        IsLoading = false;

        Products = null;

        ApplyFilters.Clear();

        Filters = new();

        InitFilters();
    }

    public async Task OnAddNewFilterClick(List<ReportFilter> filters)
    {
        ApplyFilters.AddRange(filters);

        await FiltersModal.Close(new());
    }

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        ApplyFilters.Remove(filter);
    }

    public async Task OnRemovePrint(PrintReportVm print)
    {
        var resultDialog = await Dialog.ConfirmAsync(
        TextResources.APP_StringKeys_Message_Delete,
        TextResources.APP_StringKeys_Attention,
        okButtonText: TextResources.APP_StringKeys_Approve,
        cancelButtonText: TextResources.APP_StringKeys_Return);

        if (!resultDialog)
        {
            return;
        }

        IsLoading = true;

        var result = await Api.SendAsyncObjectByUri<EditPrintVm>(HttpMethod.Delete
            , "Print/DeletePrint"
            , new DeletePrintCommand()
            {
                ProductSerial = print.ProductSerial
            });

        if (result.Value.Result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Products.RemoveAll(p => p.ProductSerial.Equals(print.ProductSerial));

            Grid.Rebind();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    private void InitFilters()
    {
        Filters.Clear();

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductSerial",
            Type = FilterType.Static,
            Component = FilterComponent.Text,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_ProductSerial
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ProductCode",
            Type = FilterType.Static,
            Component = FilterComponent.ProductCodeModal,
            IsLikeCheckboxShown = true,
            Label = TextResources.APP_StringKeys_Chart_ProductCode
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "PrintFlag",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_PrintFlag,
            Items = new List<ReportDataItem>
            {
                new() { Label = "چاپ شده", Value = "1" },
                new() { Label = "چاپ نشده", Value = "0" }
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "RegisterFlag",
            Type = FilterType.Static,
            Component = FilterComponent.Drop,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_RegisterFlag,
            Items = new List<ReportDataItem>
            {
                new() { Label = "رجیستر شده", Value = "1" },
                new() { Label = "رجیستر نشده", Value = "0" }
            }
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "FromDate",
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            EqualityType = FilterEqualityType.BiggerThan,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_FromDate
        });

        Filters.Add(new()
        {
            Id = FilterCount++,
            FieldName = "ToDate",
            Type = FilterType.Static,
            Component = FilterComponent.PersianDate,
            EqualityType = FilterEqualityType.SmallerThan,
            IsLikeCheckboxShown = false,
            Label = TextResources.APP_StringKeys_ToDate
        });
    }

    private string ResolveUserName(string? userId)
    {
        if (userId.HasNoValue())
        {
            return string.Empty;
        }

        var user = Users.FirstOrDefault(p => p.Id == userId || p.UserName == userId);

        return user?.Name ?? user?.UserName ?? userId;
    }
}
