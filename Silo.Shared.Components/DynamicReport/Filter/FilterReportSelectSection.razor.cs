using Silo.Application.Dto;
using Silo.Shared.Components.Modals;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;

public partial class FilterReportSelectSection
{
    public Guid Id = Guid.NewGuid();
    public ReportFilter Request = new();
    public List<ReportFilter> ApplyFilters = new();
    public int? SelectedFilterId;

    public Modal ModalDetails { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationCodeModal { get; set; }
    public TelerikDropDownList<ReportFilter, int?> DropRef { get; set; }

    [Parameter][EditorRequired] public List<ReportFilter> Filters { get; set; }
    [Parameter] public string InputClass { get; set; }
    [Parameter] public EventCallback<List<ReportFilter>> OnSearchClick { get; set; }
    [Parameter] public EventCallback<ReportFilter> OnAddClick { get; set; }
    [Parameter] public EventCallback<ReportFilter> OnRemoveClick { get; set; }

    public async Task OnAddFilterClick(MouseEventArgs e)
    {
        if (Request.Value.HasNoValue())
        {
            return;
        }

        await OnAddClick.InvokeAsync(Request);

        ReportFilter request = new()
        {
            FieldName = Request.FieldName,
            Value = Request.Value,
            Values = Request.Values,
            Label = Request.Label,
            AddType = Request.AddType,
            Component = Request.Component,
            Type = Request.Type,
            IsLike = Request.IsLike,
            EqualityType = Request.IsLike ? FilterEqualityType.Like : Request.EqualityType,
            IsLikeCheckboxShown = Request.IsLikeCheckboxShown,
            AdditionalData = Request.AdditionalData
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

    public async Task OnFilterRemoveClick(ReportFilter filter)
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
            Value = filter.Value,
            IsLikeCheckboxShown = filter.IsLikeCheckboxShown,
            AdditionalData = filter.AdditionalData
        };
    }

    public async Task OnModalCheckboxChange(object e)
    {
        var items = Request.Items.Where(p => p.IsChoosen).ToList();

        if (items.Any())
        {
            Request.Values = items.Select(p => p.Value).ToList();
            Request.Value = string.Join(',', items.Select(p => p.Value));
        }
        else
        {
            Request.Value = string.Empty;
            Request.Values = new();
        }
    }

    public async Task OnModalOpenClick(MouseEventArgs e)
    {
        Request.Items.ForEach(p => p.IsChoosen = false);

        await ModalDetails.Open(e);
    }

    public async Task OnCustomModalsChoose(string code)
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
