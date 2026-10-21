namespace Silo.Modules.Register.Components;

/// <summary>The one place a page tells the user what happened and what to do next (Bootstrap kind: info, success, warning, danger).</summary>
public partial class StatusMessage
{
    [Parameter] public string Kind { get; set; } = "info";
    [Parameter] public string? Message { get; set; }
    [Parameter] public string? Class { get; set; }

    /// <summary>Optional buttons shown after the message.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string IconName => Kind switch
    {
        "success" => MaterialIconsHelper.CheckCircle,
        "warning" => MaterialIconsHelper.Warning,
        "danger" => MaterialIconsHelper.Error,
        _ => MaterialIconsHelper.Info
    };
}
