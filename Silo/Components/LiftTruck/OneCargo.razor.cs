using Silo.Application.Features;

namespace Silo.Components.LiftTruck;

public partial class OneCargo
{
    private DateTime PointerDownTime;
    public bool IsProductSelected = false;

    [Parameter] public int WidthCols { get; set; } = 8;
    [Parameter] public string Class { get; set; }
    [Parameter] public CargoProductDto Product { get; set; }

    [Parameter] public EventCallback<CargoProductDto> OnLongPress { get; set; }


    public async Task OnPointerDown(PointerEventArgs e)
    {
        PointerDownTime = DateTime.UtcNow;
    }

    public async Task OnPointerUp(PointerEventArgs e)
    {
        var downTime = (DateTime.UtcNow.Ticks - PointerDownTime.Ticks) / TimeSpan.TicksPerMillisecond;

        if (downTime > 400)
        {
            IsProductSelected = !IsProductSelected;

            await OnLongPress.InvokeAsync(Product);
        }
    }
}
