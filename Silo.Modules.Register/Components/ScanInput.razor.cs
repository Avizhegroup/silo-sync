using System.Linq.Expressions;

namespace Silo.Modules.Register.Components;

/// <summary>
/// The text box used for serials, barcodes and EPCs that are typed, scanned or filled by a reader. A scanner or the
/// Enter key commits the value; <see cref="OnKeyUp"/> passes every key up so a page can react to Enter.
/// </summary>
public partial class ScanInput
{
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }
    [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public int MaxLength { get; set; } = 100;
    [Parameter] public bool Enabled { get; set; } = true;

    private TelerikTextBox? Box { get; set; }

    public async Task FocusAsync()
    {
        if (Box is not null)
        {
            await Box.FocusAsync();
        }
    }

    private async Task OnValueChanged(string value)
    {
        Value = value ?? string.Empty;
        await ValueChanged.InvokeAsync(Value);
    }

    private Task OnKeyUpAsync(KeyboardEventArgs args) => OnKeyUp.InvokeAsync(args);
}
