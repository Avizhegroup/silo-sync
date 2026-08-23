namespace Silo.Shared.Components;
public partial class Toggle
{
    public string Id = Guid.NewGuid().ToString();

    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Style { get; set; } = string.Empty;
    [Parameter] public string IconName { get; set; } = string.Empty;
    [Parameter] public bool IsVisible { get; set; } = true;
    [Parameter] public bool Value { get; set; } = false;
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }
    [Parameter] public EventCallback<bool> OnCheckedChanged { get; set; }

    public async Task OnToggleChanged(MouseEventArgs e)
    {
        Value = !Value;
        
        await ValueChanged.InvokeAsync(Value);

        await OnCheckedChanged.InvokeAsync(Value);
    }

    private string GetIconClass()
    {
        return $"toggle-icon {(Value ? "checked" : "unchecked")}";
    }
}
