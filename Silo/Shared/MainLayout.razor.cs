using Microsoft.AspNetCore.Components.Routing;

namespace Silo.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    public bool IsAutheticated = false;
    public bool IsTruckRoute = false;
    public string Title;
    private bool firstRenderDone = false;

    private bool isLoginPage;
    public bool IsLoginPageFlag => isLoginPage;

    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthState { get; set; }

    public TelerikNotification Notification { get; set; }
    public SiloComponentsContext SiloContext { get; set; } = new();

    private IDisposable? locationHandler;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState.GetAuthenticationStateAsync();
        IsAutheticated = state.User.Identity.IsAuthenticated;

        AuthState.AuthenticationStateChanged += AuthState_AuthenticationStateChanged;

        locationHandler = NavigationManager.RegisterLocationChangingHandler(OnLocationChanged);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && !firstRenderDone)
        {
            UpdateCurrentPageFlags();
            StateHasChanged();
            firstRenderDone = true;
        }
    }

    private void AuthState_AuthenticationStateChanged(Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> task)
    {
        IsAutheticated = task.GetAwaiter().GetResult().User.Identity.IsAuthenticated;
        StateHasChanged();
    }

    private void UpdateCurrentPageFlags()
    {
        var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);

        isLoginPage = string.IsNullOrEmpty(relativePath) || relativePath.StartsWith("account/login", StringComparison.OrdinalIgnoreCase);

        IsTruckRoute = relativePath.StartsWith("truck/", StringComparison.OrdinalIgnoreCase);
    }

    public async ValueTask OnLocationChanged(LocationChangingContext context)
    {
        string relativePath;

        if (Uri.TryCreate(context.TargetLocation, UriKind.Absolute, out var absoluteUri))
        {
            relativePath = absoluteUri.AbsolutePath.TrimStart('/');
        }
        else
        {
            relativePath = context.TargetLocation.TrimStart('/');
        }

        isLoginPage = string.IsNullOrEmpty(relativePath) || relativePath.StartsWith("account/login", StringComparison.OrdinalIgnoreCase);

        
        IsTruckRoute = relativePath.StartsWith("truck/", StringComparison.OrdinalIgnoreCase);

        StateHasChanged();

        await ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        
        locationHandler?.Dispose();
        AuthState.AuthenticationStateChanged -= AuthState_AuthenticationStateChanged;
    }
}



