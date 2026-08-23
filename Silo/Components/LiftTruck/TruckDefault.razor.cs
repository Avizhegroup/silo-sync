using Silo.Application.Features;
using Silo.Pages.LiftTruck;

namespace Silo.Components.LiftTruck;

public partial class TruckDefault
{
    public TruckCargoDto Cargo;
    public bool IsComponentShown = true;
    public bool IsVerifyButtonShown = false;

    [Parameter] public EventCallback OnVerifyClick { get; set; }
    [Parameter] public EventCallback<TruckIndexMode> OnChangeModeClick { get; set; }

    public void Show()
    {
        IsComponentShown = true;
    }

    public void Show(TruckCargoDto cargo)
    {
        Cargo = cargo;

        IsComponentShown = true;

        IsVerifyButtonShown = true;

        StateHasChanged();
    }

    public void Hide()
    {
        IsComponentShown = false;

        IsVerifyButtonShown = false;

        Cargo = null;
    }

    public async Task OnActionVerifyClick(MouseEventArgs e)
    {
        await OnVerifyClick.InvokeAsync();
    }

    public async Task OnChangePageActiveModeClick(TruckIndexMode type)
    {
        await OnChangeModeClick.InvokeAsync(type);
    }
}
