using System.Linq.Expressions;
using Silo.Modules.Register.Models;

namespace Silo.Modules.Register.Components;

/// <summary>A Telerik drop-down over <see cref="OptionItem"/>s with a string value; an empty value means "nothing chosen".</summary>
public partial class OptionSelect
{
    [Parameter] public IEnumerable<OptionItem> Options { get; set; } = [];
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }

    /// <summary>Shown while no option is chosen; null shows an empty box.</summary>
    [Parameter] public string? DefaultText { get; set; } = TextResources.APP_StringKeys_NotChoosed;
    [Parameter] public string? Id { get; set; }
    [Parameter] public bool Enabled { get; set; } = true;

    private async Task OnValueChanged(string? value)
    {
        Value = value ?? string.Empty;
        await ValueChanged.InvokeAsync(Value);
    }
}
