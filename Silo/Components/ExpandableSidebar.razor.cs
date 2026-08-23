namespace Silo.Components;

public partial class ExpandableSidebar
{
    [Parameter] public bool IsExpanded { get; set; } = false;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public ExpandableSidebarDirection Direction { get; set; } = ExpandableSidebarDirection.RightToLeft;
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public EventCallback<bool> OnExpandStateChanged { get; set; }

    public async Task OnExpandClick(MouseEventArgs e)
    {
        IsExpanded = !IsExpanded;

        await OnExpandStateChanged.InvokeAsync(IsExpanded);
    }
}

public enum ExpandableSidebarDirection
{
    RightToLeft,
    LeftToRight
}
