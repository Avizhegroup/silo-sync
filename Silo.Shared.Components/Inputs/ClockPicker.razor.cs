namespace Silo.Shared.Components;

public partial class ClockPicker
{
    public string Id = Guid.NewGuid().ToString();

    public string? CurrentValue
    {
        get => Value;
        set
        {
            {
                Value = value;
                _ = ValueChanged.InvokeAsync(CurrentValue);
            }
        }
    }

    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public string? Value { get; set; }
    [Parameter] public bool IsEnabled { get; set; } = true;

    [Parameter] public EventCallback<string?> OnChange { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var objectRf = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("initClockpicker", objectRf, Id);
        }
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (ValueChanged.HasDelegate is false) return;

        if (!IsEnabled)
        {
            return;
        }

        await OnChange.InvokeAsync(Value);
    }

    [JSInvokable]
    public async Task SetComponentValue(string date)
    {
        if (!IsEnabled)
        {
            return;
        }

        CurrentValue = date;

        StateHasChanged();
    }
}
