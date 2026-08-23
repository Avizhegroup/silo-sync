using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Silo.Identity.Client;

namespace Silo.Ui.Customer.Services;

/// <summary>
/// Custom authentication service for Silo.Ui.Customer that uses SiloApiClient instead of RfidConnectApi
/// </summary>
public class CustomerAuthenticationService(ISiloApiClient apiClient
    , SiloAuthenticationStateProvider authenticationStateProvider
    , IClaimManager claimManager
    , ProtectedLocalStorage storage) : IAuthenticationService
{
    public async Task<bool> Authenticate(string username, string password)
    {
        var authenticationResponse = await apiClient.PostAsyncByUri<string>("Account/AuthenticateByPassword", "AuthenticateByPassword"
            , new KeyValuePair<string, object>("Username", username)
            , new KeyValuePair<string, object>("Password", password));

        if (authenticationResponse.Successful)
        {
            await authenticationStateProvider.SetUserAuthenticated(authenticationResponse.Value);
            return true;
        }

        return false;
    }

    public async Task<bool> TruckAuthenticate(string username, string password)
    {
        var authenticationResponse = await apiClient.PostAsync<string>("STruckLogin"
            , new("username", username)
            , new("password", password));

        if (authenticationResponse.Successful && !authenticationResponse.Value.Equals("0"))
        {
            await authenticationStateProvider.SetUserAuthenticated(authenticationResponse.Value);
            return true;
        }
        
        return false;
    }

    public async Task Logout()
    {
        await authenticationStateProvider.SetUserLoggedOut();
    }

    public async Task<bool> IsUserInNonFactorialRoles()
    {
        var localRoles =await storage.GetAsync<string>("role");

        if (localRoles.Success)
        {
            return localRoles.Value.Equals("shop") 
                || localRoles.Value.ToLower().Equals("install");
        }

        return false;
    }

    public async Task<bool> IsUserInRole(string roleName)
    {
        var localRoles = await claimManager.GetUserRoles();

        if (localRoles is not null)
        {
            return localRoles.Any(p => p.Name.ToLower().Equals(roleName.ToLower()));
        }

        return false;
    }
}
