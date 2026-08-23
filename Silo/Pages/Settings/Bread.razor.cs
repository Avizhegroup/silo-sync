namespace Silo.Pages.Settings;

public partial class Bread
{
    public new bool mustCheckAccess = false;
    public bool IsLoading = true;
    List<Application.Dto.Claim> Claims;
    public List<NavbarAllTitle> MenuLinks = new();
    public NavbarAllTitle SelectedAllTitle;

    [Inject] public IClaimManager ClaimManager { get; set; }

    [Parameter] public int? LinkId { get; set; }

    protected override async Task SiloInitializer()
    {
        Claims = await ClaimManager.GetUserClaims();

        if (!MenuLinks.Any())
        {
            MenuLinks = await ClaimManager.GetAllLinks();
        }

        if (!await ClaimManager.IsUserAdmin())
        {
            await CheckAccess();
        }

        var link = FindLink();

        if (link is not null)
        {
            SelectedAllTitle = link;
        }

        IsLoading = false;
    }

    public async Task CheckAccess()
    {
        List<NavbarAllTitle> accessLinks = new();

        foreach (var title in MenuLinks)
        {
            var accessTitle = accessLinks.FirstOrDefault(t => t.Title == title.Title);

            if (accessTitle is null && title.Children.Any())
            {
                accessTitle = new()
                {
                    Id = title.Id,
                    Title = title.Title,
                    IconName = title.IconName,
                    Children = new()
                };

                accessLinks.Add(accessTitle);
            }

            foreach (var category in title.Children)
            {
                var titleCategory = accessTitle.Children.FirstOrDefault(c => c.Title == category.Title);

                if (titleCategory is null && category.Children.Any())
                {
                    titleCategory = new()
                    {
                        Id = category.Id,
                        Title = category.Title,
                        Children = new()
                    };

                    accessTitle.Children.Add(titleCategory);
                }

                foreach (var link in category.Children)
                {
                    if (!link.Url.StartsWith("/"))
                    {
                        link.Url = $"/{link.Url}";
                    }

                    foreach (var claim in Claims)
                    {
                        var claimValue = claim.Value;

                        if (!claimValue.StartsWith("/"))
                        {
                            claimValue = $"/{claimValue}";
                        }

                        if (!claimValue.Contains(".cshtml"))
                        {
                            claimValue = $"/Views{claimValue}.cshtml";
                        }

                        if (claimValue.ToLower() == $"/views{link.Url.ToLower()}.cshtml")
                        {
                            titleCategory.Children.Add(link);
                        }
                    }
                }
            }

            MenuLinks = accessLinks;
        }
    }

    public NavbarAllTitle FindLink()
    {
        foreach (NavbarAllTitle link in MenuLinks)
        {
            if (link.Id == LinkId)
            {
                return link;
            }

            foreach (var category in link.Children)
            {
                if (category.Id == LinkId)
                {
                    return new()
                    {
                        Children = new()
                        {
                            category 
                        }
                    };
                }
            }
        }

        return null;
    }
}
