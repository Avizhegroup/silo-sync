namespace Silo.Identity.Client;

public interface IAuthenticationService
{
    Task<bool> TruckAuthenticate(string username, string password);
    Task<bool> Authenticate(string username, string password);
    Task Logout();

    /// <summary>
    /// Roles with names : 'Install' and 'Shop' cannot login in Main software 'Silo'
    /// </summary>
    /// <returns></returns>
    Task<bool> IsUserInNonFactorialRoles();
    Task<bool> IsUserInRole(string roleName);
}
