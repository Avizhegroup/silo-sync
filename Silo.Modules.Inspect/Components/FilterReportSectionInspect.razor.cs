using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Inspect;
public partial class FilterReportSectionInspect
{
    public Guid Id = Guid.NewGuid();
    public ReportFilterGeneric<InspectReportDynamicFilterType> Request = new();
    public List<ReportFilterGeneric<InspectReportDynamicFilterType>> ApplyFilters = new();
    public int? SelectedFilterId;

    public Modal ModalDetails { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }
    public TelerikDropDownList<ReportFilterGeneric<InspectReportDynamicFilterType>, int?> DropRef { get; set; }

    [Parameter][EditorRequired] public List<ReportFilterGeneric<InspectReportDynamicFilterType>> Filters { get; set; }
    [Parameter] public string InputClass { get; set; }
    [Parameter] public EventCallback<List<ReportFilterGeneric<InspectReportDynamicFilterType>>> OnSearchClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<InspectReportDynamicFilterType>> OnAddClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<InspectReportDynamicFilterType>> OnRemoveClick { get; set; }

    public async Task OnAddFilterClick(MouseEventArgs e)
    {
        if (Request.Value.HasNoValue())
        {
            return;
        }

        await OnAddClick.InvokeAsync(Request);

        ReportFilterGeneric<InspectReportDynamicFilterType> request = new()
        {
            FieldName = Request.FieldName,
            Value = Request.Value,
            Label = Request.Label,
            AddType = Request.AddType,
            Component = Request.Component,
            Type = Request.Type,
            IsLike = Request.IsLike,
            EqualityType = Request.IsLike ? FilterEqualityType.Like : Request.EqualityType
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

    public async Task OnFilterRemoveClick(ReportFilterGeneric<InspectReportDynamicFilterType> filter)
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
            EqualityType = Request.IsLike ? FilterEqualityType.Like : Request.EqualityType,
            Items = filter.Items,
            Label = filter.Label,
            FieldName = filter.FieldName,
            Type = filter.Type,
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
