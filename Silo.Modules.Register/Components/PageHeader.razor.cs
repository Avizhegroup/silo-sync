namespace Silo.Modules.Register.Components;

/// <summary>
/// The top of every Register page: title, one-line purpose, the device status (pages that use the reader or printer),
/// navigation buttons (child content) and the collapsible "how to use this page" steps.
/// </summary>
public partial class PageHeader
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string? Subtitle { get; set; }

    /// <summary>True on pages that talk to the device host; shows its connection status.</summary>
    [Parameter] public bool ShowDevice { get; set; }

    [Parameter] public bool IsDeviceConnected { get; set; }

    /// <summary>The ordered steps of the "how to use" panel; empty hides the panel.</summary>
    [Parameter] public IReadOnlyList<string> Steps { get; set; } = [];

    /// <summary>Navigation buttons shown at the end of the header.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
