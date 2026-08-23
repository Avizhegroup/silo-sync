using Silo.Application.Features;
using IAuthenticationService = Silo.Identity.Client.IAuthenticationService;
namespace Silo.Ui.Customer.Pages.Account;

public partial class Login
{
    public string Company;
    public bool IsLoading = false;
    public GetLoginUserQuery Request = new();

    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Company = Configuration["Settings:Company"];
    }

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
                NavigationManager.NavigateTo("/dashboard", true);
            }
            else
            {
                await AuthenticationService.Logout();

                IsLoading = false;

                Notification.Show(TextResources.APP_StringKeys_Validation_LoginFail, "error");

                return;
            }
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

}
