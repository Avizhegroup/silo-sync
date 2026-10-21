using System.Linq.Expressions;

namespace Silo.Modules.Register.Components;

/// <summary>The drop-down of the printers installed on the workstation of the device host. An empty value means "the default".</summary>
public partial class PrinterSelect
{
    [Parameter] public IEnumerable<string> Printers { get; set; } = [];
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }
    [Parameter] public string DefaultText { get; set; } = RegisterTexts.PrinterDefault;
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;

    private async Task OnValueChanged(string? value)
    {
        Value = value ?? string.Empty;
        await ValueChanged.InvokeAsync(Value);
    }
}
