namespace Silo.Modules.Register.Components;

/// <summary>A label, an input (child content), an optional hint and an optional validation error, laid out the same everywhere.</summary>
public partial class FormField
{
    [Parameter] public string Label { get; set; } = string.Empty;

    /// <summary>The id of the input, so a click on the label focuses it.</summary>
    [Parameter] public string? For { get; set; }
    [Parameter] public string? Hint { get; set; }
    [Parameter] public string? Error { get; set; }
    [Parameter] public bool IsRequired { get; set; }

    /// <summary>Bootstrap column classes, e.g. "col-md-4".</summary>
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
