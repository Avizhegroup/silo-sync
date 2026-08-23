using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Silo.Application.Features;

namespace Silo.Components.LiftTruck;

public partial class TruckSettings
{
    private bool IsComponentShown = false;
    public TruckConfigDto TruckConfig = new();

    public EditForm EditForm { get; set; }

    [Parameter] public EventCallback<TruckConfigDto> OnSettingsSave { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    [Inject] public ProtectedLocalStorage Storage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var truckStorageResult = await Storage.GetAsync<string>("truck");

        if (!truckStorageResult.Success)
        {
            TruckConfig.TruckNumber = truckStorageResult.Value;
        }
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        foreach (string validation in context.GetValidationMessages())
        {
            Notification.Show(validation, "error");
        }
    }

    public async Task OnValidSubmit(EditContext context)
    {
        await Storage.SetAsync("truck", TruckConfig.TruckNumber);

        await OnSettingsSave.InvokeAsync(TruckConfig);

        Hide();
    }

    public void Show()
    {
        IsComponentShown = true;

        StateHasChanged();
    }

    public void Hide()
    {
        IsComponentShown = false;

        TruckConfig = new();
    }
}
