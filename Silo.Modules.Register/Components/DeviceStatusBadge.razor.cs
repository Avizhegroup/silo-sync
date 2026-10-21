namespace Silo.Modules.Register.Components;

/// <summary>A pill that tells whether the local device host (reader, printer, scanner) is connected.</summary>
public partial class DeviceStatusBadge
{
    [Parameter] public bool IsConnected { get; set; }
}
