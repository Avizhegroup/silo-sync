using Silo.Application.Dto;
using Silo.Shared.Components.Modals;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Report;
public partial class FilterSection<TItem> where TItem : struct
{
    public Guid Id = Guid.NewGuid();
    public ReportFilterGeneric<TItem> Request = new();
    public List<ReportFilterGeneric<TItem>> ApplyFilters = new();
    public int? SelectedFilterId;

    public Modal ModalDetails { get; set; }
    public ProductCodeModal ProductCodeModal { get; set; }
    public TelerikDropDownList<ReportFilterGeneric<TItem>, int?> DropRef { get; set; }

    [Parameter][EditorRequired] public List<ReportFilterGeneric<TItem>> Filters { get; set; }
    [Parameter] public string InputClass { get; set; }
    [Parameter] public EventCallback<List<ReportFilterGeneric<TItem>>> OnSearchClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<TItem>> OnAddClick { get; set; }
    [Parameter] public EventCallback<ReportFilterGeneric<TItem>> OnRemoveClick { get; set; }

    public async Task OnAddFilterClick(MouseEventArgs e)
    {
        if (Request.Value.HasNoValue())
        {
            return;
        }

        await OnAddClick.InvokeAsync(Request);

        ReportFilterGeneric<TItem> request = new()
        {
            FieldName = Request.FieldName,
            Value = Request.Value,
            Label = Request.Label,
            AddType = Request.AddType,
            Component = Request.Component,
            Type = Request.Type,
            IsLike = Request.IsLike,
            EqualityType = Request.IsLike ? FilterEqualityType.Like : Request.EqualityType,
            FilterId = Request.FilterId,
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

    public async Task OnFilterRemoveClick(ReportFilterGeneric<TItem> filter)
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
            FieldType = filter.FieldType,
            FilterId = filter.Id,
            IsLikeCheckboxShown = filter.IsLikeCheckboxShown,
            AdditionalData = filter.AdditionalData
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
