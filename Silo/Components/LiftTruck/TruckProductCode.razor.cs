using Microsoft.AspNetCore.Components.Forms;

namespace Silo.Components.LiftTruck;

public partial class TruckProductCode
{
    public string ProductCode = string.Empty;
    private bool IsComponentShown = false;

    [Parameter] public EventCallback<string> OnProductCodeSave { get; set; }
    [Parameter] public EventCallback<bool> OnCloseClicked { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    public async Task OnValidSubmit(MouseEventArgs e)
    {
        if (ProductCode.HasNoValue())
        {
            Notification.Show(
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_ProductCode)
                , "error");

            return;
        }

        await OnProductCodeSave.InvokeAsync(ProductCode);
    }

    public void Show()
    {
        IsComponentShown = true;

        StateHasChanged();
    }

    public void Hide()
    {
        IsComponentShown = false;

        ProductCode = string.Empty;
    }

    public async Task OnCloseClick(MouseEventArgs e)
    {
        await OnCloseClicked.InvokeAsync();
    }
}
