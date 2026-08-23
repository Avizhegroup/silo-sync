namespace Silo.Pages.Account;
public partial class Login
{
    public bool IsLoading = false;
    public GetLoginUserQuery Request = new();
    private bool ShowPassword = false;

    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

#if DEBUG
    protected override async Task OnInitializedAsync()
    {
        Request.Username = "admin";

        Request.Password = "rfidadmin";
    }
#endif

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("localStorage.clear");
        }
    }

    public async Task OnValidSubmit()
    {
        IsLoading = true;

        if (await AuthenticationService.Authenticate(Request.Username, Request.Password))
        {
            if (await AuthenticationService.IsUserInNonFactorialRoles())
            {
                await AuthenticationService.Logout();

                IsLoading = false;

                Notification.Show(TextResources.APP_StringKeys_Validation_LoginFail, "error");

                return;
            }

            NavigationManager.NavigateTo("/", true);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_LoginFail, "error");
        }

        IsLoading = false;
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        foreach (string message in context.GetValidationMessages())
        {
            Notification.Show(message, "error");
        }
    }

    public async Task OnPasswordKeyDown(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await OnValidSubmit();
        }
    }
    private void TogglePassword()
    {
        ShowPassword = !ShowPassword;
    }
}
