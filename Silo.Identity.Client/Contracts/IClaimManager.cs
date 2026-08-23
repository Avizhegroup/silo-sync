using Microsoft.AspNetCore.Identity;
using Silo.Application.Dto;
using Silo.Application.Features;

namespace Silo.Identity.Client;
public interface IClaimManager
{
    Task<List<Claim>> GetUserClaims();
    Task<List<IdentityRole>> GetUserRoles();
    Task ClearDataLists();
    Task<bool> IsUserAdmin();
    Task<List<NavbarAllTitle>> GetAllLinks();
    Task<string> GetUrlTitle(string fullUrl);
    Task<List<GetUserQuickAccessVm>> GetQuickAccessLinks();
    Task<bool> SaveQuickAccessLink(int menuLinkId);
    Task<bool> RemoveQuickAccessLink(int id);
    void ClearQuickAccessCache();
}
