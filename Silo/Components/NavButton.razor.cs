namespace Silo.Components;

public partial class NavButton
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string Link { get; set; }
    [Parameter] public bool Force { get; set; } = false;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] public NavigationManager NavigationManager { get; set; }

    public async Task OnButtonClick(MouseEventArgs e)
    {
        NavigationManager.NavigateTo(Link, Force);
    }
}
