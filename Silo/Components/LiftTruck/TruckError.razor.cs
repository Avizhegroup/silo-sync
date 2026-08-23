namespace Silo.Components.LiftTruck;

public partial class TruckError
{
    public string ErrorMessage;

    [Parameter] public EventCallback OnCloseClick { get; set; }

    public void Show(string message)
    {
        ErrorMessage = message;
    }

    public void Hide()
    {
        ErrorMessage = null;

        StateHasChanged();
    }

    public async Task OnCloseButtonClicked(MouseEventArgs e)
    {
        Hide();

        await OnCloseClick.InvokeAsync();
    }
}
