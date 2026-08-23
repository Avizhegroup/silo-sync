using Silo.Infrastructure.Web;

namespace Silo.Identity.Client;
public partial class AuthenticationService(RfidConnectApi Api
    , SiloAuthenticationStateProvider AuthenticationStateProvider
    , IClaimManager ClaimManager) : IAuthenticationService
{
    public async Task<bool> Authenticate(string username, string password)
    {
        var authenticationResponse = await Api.PostAsyncByUri<string>("Account/AuthenticateByPassword"
            , new KeyValuePair<string, object>("Username", username)
            , new KeyValuePair<string, object>("Password", password));

        if (authenticationResponse.Successful)
        {
            await AuthenticationStateProvider.SetUserAuthenticated(authenticationResponse.Value);

            return true;
        }

        return false;
    }

    public async Task<bool> TruckAuthenticate(string username, string password)
    {
        var authenticationResponse = (await Api.PostAsync<string>("STruckLogin"
            , new("username", username)
            , new("password", password))).Value;

        if (!authenticationResponse.Equals("0"))
        {
            await AuthenticationStateProvider.SetUserAuthenticated(authenticationResponse);

            return true;
        }
        return false;
    }

    public async Task Logout()
    {
        await AuthenticationStateProvider.SetUserLoggedOut();
    }

    public async Task<bool> IsUserInNonFactorialRoles()
    {
        bool isShop = false;

        var localRoles = (await ClaimManager.GetUserRoles());

        if (localRoles is not null)
        {
            isShop = localRoles.Any(p => p.Name.ToLower().Equals("shop")
                                                || p.Name.ToLower().Equals("installer"));
        }
        else
        {
            return false;
        }

        return isShop;
    }

    public async Task<bool> IsUserInRole(string roleName)
    {
        bool isShop = false;

        var localRoles = (await ClaimManager.GetUserRoles());

        if (localRoles is not null)
        {
            isShop = localRoles.Any(p => p.Name.ToLower().Equals(roleName));
        }
        else
        {
            return false;
        }

        return isShop;
    }
}
