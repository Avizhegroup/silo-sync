using Microsoft.AspNetCore.Identity;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Infrastructure.Web;

namespace Silo.Identity.Client;
public partial class ClaimManager(RfidConnectApi Api) : IClaimManager
{
    private List<Claim> claims;
    private List<IdentityRole> roles;
    private List<GetAllMenuLinksVm> links;
    private List<GetUserQuickAccessVm> quickAccess;
    private bool? isAdmin = null;

    public async Task<List<Claim>> GetUserClaims()
    {
        if (claims is null)
        {
            claims = (await Api.PostAsync<List<ClaimVm>>("GetUserClaimsByToken")).Value
                    .Select(p => new Claim()
                    {
                        Type = p.Type, 
                        Value = p.Value
                    }).ToList();
        }

        return claims;
    }

    public async Task<List<IdentityRole>> GetUserRoles()
    {
        if (roles is null)
        {
            roles = (await Api.PostAsync<List<IdentityRole>>("GetUserRolesByToken")).Value;
        }

        return roles;
    }

    public async Task ClearDataLists()
    {
        claims = null;

        roles = null;

        isAdmin = null;
    }

    public async Task<bool> IsUserAdmin()
    {
        if (isAdmin is null)
        {
            var localRoles = (await GetUserRoles());

            if (localRoles is not null)
            {
                isAdmin = localRoles.Any(p => p.Name.ToLower() == "admin");
            }
            else
            {
                return false;
            }
        }

        return (bool)isAdmin;
    }

    public async Task<List<NavbarAllTitle>> GetAllLinks()
    {
        if (links is null)
        {
            links = (await Api.PostAsyncByContext<List<GetAllMenuLinksVm>>("SGetAllMenuLinks", new GetAllMenuLinksVmContext())).Value;
        }

        List<NavbarAllTitle> titles = new();

        foreach (var mainLink in links.Where(p=> p.Level == 1))
        {
            NavbarAllTitle mainTitle = new() 
            { 
                Title = mainLink.Title,
                IconName = mainLink.IconName,
                Id = mainLink.Id
            };

            foreach (var innerLink in links.Where(p=> p.Level == 2 && p.ParentId == mainLink.Id))
            {
                NavbarCategory innerTitle = new()
                {
                    Title = innerLink.Title,
                    Id = innerLink.Id
                };

                foreach (var hrefLink in links.Where(p => p.Level == 3 && p.IsShown && p.ParentId == innerLink.Id))
                {
                    NavbarCategory hrefTitle = new()
                    {
                        Title = hrefLink.Title,
                        Id = hrefLink.Id
                    };

                    innerTitle.Children.Add(new NavbarLink()
                    {
                        Id = hrefLink.Id,
                        Title = hrefLink.Title,
                        Url = hrefLink.Url
                    });
                }

                mainTitle.Children.Add(innerTitle);
            }

            titles.Add(mainTitle);
        }

        return titles;
    }

    /// <summary>
    /// Get the title of a URL from links
    /// </summary>
    /// <param name="fullUrl"></param>
    /// <returns></returns>
    public async Task<string> GetUrlTitle(string fullUrl)
    {
        fullUrl = UrlTools.RemoveUrlSitePart(fullUrl, "/");

        if (!fullUrl.StartsWith("/"))
        {
            fullUrl = $"/{fullUrl}";
        }

        if (links is null)
        {
            await GetAllLinks();
        }

        var link = links.FirstOrDefault(p => p.Url is not null 
        && ((p.Url.StartsWith("/") && p.Url.Equals(fullUrl)) || ($"/{p.Url}".Equals(fullUrl))));

        return link?.Title;
    }

    public async Task<List<GetUserQuickAccessVm>> GetQuickAccessLinks()
    {
        if (quickAccess is null)
        {
            quickAccess = (await Api.PostAsyncByContext<List<GetUserQuickAccessVm>>("SGetUserQuickAccess", new GetUserQuickAccessVmContext())).Value ?? new();
        }

        return quickAccess;
    }

    public async Task<bool> SaveQuickAccessLink(int menuLinkId)
    {
        var result = (await Api.PostAsyncByContext<bool>("SSaveUserQuickAccess", new SaveUserQuickAccessCommandContext(),
            new KeyValuePair<string, object>[] { new("menuLinkId", menuLinkId) })).Value;

        quickAccess = null;

        return result;
    }

    public async Task<bool> RemoveQuickAccessLink(int id)
    {
        var result = (await Api.PostAsyncByContext<bool>("SRemoveUserQuickAccess", new RemoveUserQuickAccessCommandContext(),
            new KeyValuePair<string, object>[] { new("id", id) })).Value;

        quickAccess = null;

        return result;
    }

    public void ClearQuickAccessCache()
    {
        quickAccess = null;
    }
}
