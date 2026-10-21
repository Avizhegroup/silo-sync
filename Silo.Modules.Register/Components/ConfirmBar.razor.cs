namespace Silo.Modules.Register.Components;

/// <summary>An inline yes/no question that replaces a modal dialog (the page keeps working underneath).</summary>
public partial class ConfirmBar
{
    [Parameter] public string Message { get; set; } = string.Empty;

    /// <summary>"warning" or "danger".</summary>
    [Parameter] public string Kind { get; set; } = "warning";
    [Parameter] public string YesText { get; set; } = TextResources.APP_StringKeys_Yes;
    [Parameter] public string NoText { get; set; } = TextResources.APP_StringKeys_No;
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public EventCallback OnYes { get; set; }
    [Parameter] public EventCallback OnNo { get; set; }

    private Task OnYesClick() => OnYes.InvokeAsync();
    private Task OnNoClick() => OnNo.InvokeAsync();
}
