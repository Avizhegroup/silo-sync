using Silo.Application.Features;

namespace Silo.Components.Filter.Customs;

public partial class FilterReportSectionExitAction
{
    public Guid Id = Guid.NewGuid();
    public ReportFilterGeneric<ExitActionDynamicReportFilterType> Request = new();
    public List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> ApplyFilters = new();
    public int? SelectedFilterId;

    public Modal ModalDetails { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }
    public TelerikDropDownList<ReportFilterGeneric<ExitActionDynamicReportFilterType>, int?> DropRef { get; set; }

    [Parameter][EditorRequired] public List<ReportFilterGeneric<ExitActionDynamicReportFilterType>> Filters { get; set; }
    [Parameter] public string InputClass { get; set; }
    [Parameter] public EventCallback<List<ReportFilterGeneric<ExitActionDynamicReportFilterType>>> OnSearchClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<ExitActionDynamicReportFilterType>> OnAddClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<ExitActionDynamicReportFilterType>> OnRemoveClick { get; set; }

    public async Task OnAddFilterClick(MouseEventArgs e)
    {
        if (Request.Value.HasNoValue())
        {
            return;
        }

        await OnAddClick.InvokeAsync(Request);

        ReportFilterGeneric<ExitActionDynamicReportFilterType> request = new()
        {
            FieldName = Request.FieldName,
            Value = Request.Value,
            Label = Request.Label,
            AddType = Request.AddType,
            Component = Request.Component,
            FieldType = Request.FieldType,
            Type = Request.Type,
            IsLike = Request.IsLike,
            AdditionalData = Request.AdditionalData,
            EqualityType = Request.IsLike ? FilterEqualityType.Like : Request.EqualityType,
        };

        foreach (var item in Request.Items)
        {
            request.Items.Add(new()
            {
                Label = item.Label,
                IsChoosen = item.IsChoosen,
                Value = item.Value
            });
        }

        ApplyFilters.Add(request);

        Clear();
    }

    public async Task OnRefreshFilterClick(MouseEventArgs e)
    {
        Clear();
    }

    public async Task OnFilterRemoveClick(ReportFilterGeneric<ExitActionDynamicReportFilterType> filter)
    {
        await OnRemoveClick.InvokeAsync(filter);

        ApplyFilters.Remove(filter);

        DropRef.Rebind();
    }

    public async Task OnDropFilterChooseChange(object e)
    {
        if (e is null)
        {
            return;
        }

        int id = (int)e;

        var filter = Filters.FirstOrDefault(p => p.Id == id);

        filter.Items.ForEach(p => p.IsChoosen = false);

        Request = new()
        {
            Component = filter.Component,
            IsLike = filter.IsLike,
            EqualityType = Request.IsLike ? FilterEqualityType.Like : filter.EqualityType,
            Items = filter.Items,
            Label = filter.Label,
            FieldName = filter.FieldName,
            Type = filter.Type,
            AdditionalData = filter.AdditionalData,
            FieldType = filter.FieldType
        };
    }

    public async Task OnModalCheckboxChange(object e)
    {
        var items = Request.Items.Where(p => p.IsChoosen).ToList();

        if (items.Any())
        {
            Request.Value = string.Join(',', items.Select(p => p.Value));
        }
        else
        {
            Request.Value = string.Empty;
        }
    }

    public async Task OnModalOpenClick(MouseEventArgs e)
    {
        Request.Items.ForEach(p => p.IsChoosen = false);

        await ModalDetails.Open(e);
    }

    public async Task OnClickProductCode(string code)
    {
        Request.Value = code;
    }

    public async Task OnSearchButtonClick(MouseEventArgs e)
    {
        await OnSearchClick.InvokeAsync(ApplyFilters);

        Clear();
    }

    private void Clear()
    {
        SelectedFilterId = null;

        Request = new();
    }
}
