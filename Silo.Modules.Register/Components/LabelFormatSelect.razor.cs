using System.Linq.Expressions;
using Silo.Modules.Register.Models;

namespace Silo.Modules.Register.Components;

/// <summary>The drop-down of the label formats (ZPL, ZMIN, FastReport) that the device host offers; the value is <see cref="DeviceFormat.Key"/>.</summary>
public partial class LabelFormatSelect
{
    [Parameter] public IEnumerable<DeviceFormat> Formats { get; set; } = [];
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }
    [Parameter] public string DefaultText { get; set; } = RegisterTexts.LabelFormat;
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;

    private async Task OnValueChanged(string? value)
    {
        Value = value ?? string.Empty;
        await ValueChanged.InvokeAsync(Value);
    }
}
