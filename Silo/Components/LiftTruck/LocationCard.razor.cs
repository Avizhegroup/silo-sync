using Silo.Application.Features;

namespace Silo.Components.LiftTruck;

public partial class LocationCard
{
    [Parameter] public int WidthCols { get; set; } = 3;

    [Parameter] public TruckCargoDto Cargo { get; set; }
}
