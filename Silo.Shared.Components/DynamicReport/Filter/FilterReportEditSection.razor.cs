using Silo.Application.Dto;
using Silo.Shared.Components.Modals;

namespace Silo.Shared.Components.Report;
public partial class FilterReportEditSection
{
    [Parameter][EditorRequired] public List<ReportFilter> Filters { get; set; }
    [Parameter] public bool IsEditable { get; set; } = true;
    [Parameter] public string InputClass { get; set; }
    [Parameter] public EventCallback<ReportFilter> OnFilterValueChanged { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public LocationModal LocationCodeModal { get; set; }
    public Modal ModalDetails { get; set; }

    public async Task OnChange(ChangeEventArgs e, ReportFilter filter)
    {
        filter.Value = e.Value?.ToString();

        await OnFilterValueChanged.InvokeAsync(filter);
    }

    public async Task OnChange(Object e, ReportFilter filter)
    {
        filter.Value = e.ToString(); 

        await OnFilterValueChanged.InvokeAsync(filter); 
    }

    public async Task OnDropChange(object newValue, ReportFilter filter)
    {
        await OnFilterValueChanged.InvokeAsync(filter);
    }

    public async Task OnFilterRemoveClick(ReportFilter filter)
    {
        Filters.Remove(filter);
    }

    public async Task OnModalCheckboxChange(ReportFilter filter)
    {
        var items = filter.Items.Where(p => p.IsChoosen).ToList();

        if (items.Any())
        {
            filter.Values = items.Select(p => p.Value).ToList();
            filter.Value = string.Join(',', items.Select(p => p.Value));
        }
        else
        {
            filter.Values = new();
            filter.Value = string.Empty;
        }

        await OnFilterValueChanged.InvokeAsync(filter);
    }

    public async Task OnProductCodeChange(string code,ReportFilter filter)
    {
        filter.Value = code;

        await OnFilterValueChanged.InvokeAsync(filter);
    }

    public async Task OnLikeCheckboxChange(ReportFilter filter)
    {
        filter.EqualityType = filter.IsLike ? FilterEqualityType.Like : FilterEqualityType.Equals;
    }

    public async Task OnModalOpenClick(ReportFilter filter)
    {
        filter.Items.ForEach(p => p.IsChoosen = false);

        await ModalDetails.Open(new());
    }
}
