namespace Silo.Components.LiftTruck;

public partial class TruckButton
{
    public Guid Id = Guid.NewGuid();

    [Parameter] public string Class { get; set; }
    [Parameter] public ButtonType ButtonType { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    public async Task OnButtonClick(MouseEventArgs e)
    {
        await OnClick.InvokeAsync(e);
    }
}

public enum ButtonType
{
    Button,
    Submit
}
