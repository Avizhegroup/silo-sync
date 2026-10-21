namespace Silo.Modules.Register.Components;

/// <summary>A card with the numbered steps that explain how to work with a page. It can be collapsed.</summary>
public partial class HowToUse
{
    [Parameter] public IReadOnlyList<string> Steps { get; set; } = [];

    public bool IsOpen { get; set; } = true;

    private void OnToggleClick() => IsOpen = !IsOpen;
}
