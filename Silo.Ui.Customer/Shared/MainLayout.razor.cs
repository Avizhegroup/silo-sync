using Silo.Identity.Client;

namespace Silo.Ui.Customer.Shared;
public partial class MainLayout
{
    public bool IsAutheticated = false;
    public string Title;

    [Inject] public SiloAuthenticationStateProvider AuthState { get; set; }

    public TelerikNotification Notification { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();

        IsAutheticated = state.User.Identity.IsAuthenticated;

        AuthState.AuthenticationStateChanged += AuthState_AuthenticationStateChanged;
    }

    private void AuthState_AuthenticationStateChanged(Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> task)
    {
        IsAutheticated = task.GetAwaiter().GetResult().User.Identity.IsAuthenticated;

        StateHasChanged();
    }
}
