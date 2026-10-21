namespace Silo.Modules.Register.Components;

/// <summary>The card every block of a page is built from: numbered title, one-line hint, optional header actions.</summary>
public partial class SectionCard
{
    /// <summary>Step number shown in a badge; 0 hides it.</summary>
    [Parameter] public int Step { get; set; }
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? BodyClass { get; set; }

    /// <summary>Buttons or counters at the end of the card header.</summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
