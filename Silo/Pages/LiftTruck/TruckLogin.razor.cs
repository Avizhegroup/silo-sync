using Silo.Components.LiftTruck;

namespace Silo.Pages.LiftTruck;

public partial class TruckLogin
{
    public bool IsLoading = false;
    public string Username = "";
    public string Password = "";

    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    [Parameter] public string TimeoutLogout { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

#if DEBUG
    protected override async Task OnInitializedAsync()
    {
        Username = "trucktest";
        
        Password = "rfidadmin";
    }
#endif

    public async Task OnSubmit(MouseEventArgs e)
    {
        if (Username.HasNoValue()
         || Password.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }

        IsLoading = true;

        if (await AuthenticationService.TruckAuthenticate(Username, Password))
        {
            NavigationManager.NavigateTo("/truck/index", true);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_LoginFail, "error");
        }

        IsLoading = false;
    }

    public async Task OnPasswordKeyDown(KeyboardEventArgs e)
    {
        if (e.Key.HasNoValue())
        {
            return;
        }

        if (e.Key.Equals("Enter"))
        {
            await OnSubmit(new());
        }
    }
}
