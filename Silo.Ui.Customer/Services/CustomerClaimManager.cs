using Microsoft.AspNetCore.Identity;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Identity.Client;
using Silo.Shared;

namespace Silo.Ui.Customer.Services;

/// <summary>
/// Custom claim manager for Silo.Ui.Customer that uses ISiloApiClient instead of RfidConnectApi
/// </summary>
public class CustomerClaimManager : IClaimManager
{
    private readonly ISiloApiClient _apiClient;
    private List<Claim> _claims;
    private List<IdentityRole> _roles;
    private List<GetAllMenuLinksVm> _links;
    private bool? _isAdmin = null;

    public CustomerClaimManager(ISiloApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Claim>> GetUserClaims()
    {
        if (_claims is null)
        {
            var response = await _apiClient.PostAsync<List<ClaimVm>>("GetUserClaimsByToken");
            if (response.Successful)
            {
                _claims = response.Value
                    .Select(p => new Claim()
                    {
                        Type = p.Type,
                        Value = p.Value
                    }).ToList();
            }
            else
            {
                _claims = new List<Claim>();
            }
        }

        return _claims;
    }

    public async Task<List<IdentityRole>> GetUserRoles()
    {
        if (_roles is null)
        {
            var response = await _apiClient.PostAsync<List<IdentityRole>>("GetUserRolesByToken");
            if (response.Successful)
            {
                _roles = response.Value;
            }
            else
            {
                _roles = new List<IdentityRole>();
            }
        }

        return _roles;
    }

    public async Task ClearDataLists()
    {
        _claims = null;
        _roles = null;
        _isAdmin = null;
    }

    public async Task<bool> IsUserAdmin()
    {
        if (_isAdmin is null)
        {
            var localRoles = await GetUserRoles();

            if (localRoles is not null)
            {
                _isAdmin = localRoles.Any(p => p.Name.ToLower() == "admin");
            }
            else
            {
                return false;
            }
        }

        return (bool)_isAdmin;
    }

    public async Task<List<NavbarAllTitle>> GetAllLinks()
    {
        if (_links is null)
        {
            var response = await _apiClient.PostAsyncByUriAndContext<List<GetAllMenuLinksVm>>("Wms/PostObject", "SGetAllMenuLinks", new GetAllMenuLinksVmContext());
            if (response.Successful)
            {
                _links = response.Value;
            }
            else
            {
                _links = new List<GetAllMenuLinksVm>();
            }
        }

        List<NavbarAllTitle> titles = new();

        foreach (var mainLink in _links.Where(p => p.Level == 1))
        {
            NavbarAllTitle mainTitle = new()
            {
                Title = mainLink.Title,
                IconName = mainLink.IconName,
                Id = mainLink.Id
            };

            foreach (var innerLink in _links.Where(p => p.Level == 2 && p.ParentId == mainLink.Id))
            {
                NavbarCategory innerTitle = new()
                {
                    Title = innerLink.Title,
                    Id = innerLink.Id
                };

                foreach (var hrefLink in _links.Where(p => p.Level == 3 && p.IsShown && p.ParentId == innerLink.Id))
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

        if (_links is null)
        {
            await GetAllLinks();
        }

        var link = _links.FirstOrDefault(p => p.Url is not null
        && ((p.Url.StartsWith("/") && p.Url.Equals(fullUrl)) || ($"/{p.Url}".Equals(fullUrl))));

        return link?.Title;
    }

    public Task<List<GetUserQuickAccessVm>> GetQuickAccessLinks()
        => Task.FromResult(new List<GetUserQuickAccessVm>());

    public Task<bool> SaveQuickAccessLink(int menuLinkId)
        => Task.FromResult(false);

    public Task<bool> RemoveQuickAccessLink(int id)
        => Task.FromResult(false);

    public void ClearQuickAccessCache() { }
}
