using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Silo.Shared;
public partial class Navbar : IDisposable
{
    public bool IsLoading = true;
    public string UserName = string.Empty;
    public List<NavbarAllTitle> NavBreadItems = new();
    public string Image = "-1";
    public string IsSearchActive = "";
    public bool IsMenuActive = false;
    public bool IsProfileActive = false;
    public string CurrentDateTime = PersianCalendarTools.PersianDayName(DateTime.Now) + " "
        + PersianCalendarTools.GregorianToPersian(DateTime.Now);
    public List<NavbarAllTitle> MenuLinks = new();
    public List<Application.Dto.Claim> Claims;
    public NavbarAllTitle SelectedAllTitle = new();
    public List<KeyValuePair<string, string>> Items;
    public List<ChoosableKeyValue> Pages = new();
    public string Version;
    public bool IsDarkMode = false;

    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IJSRuntime JS { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthState { get; set; }

    [CascadingParameter] public SiloComponentsContext SiloContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Version = GetType().Assembly.GetName().Version.ToString();

        Claims = await ClaimManager.GetUserClaims();

        if (!MenuLinks.Any())
        {
            MenuLinks = await ClaimManager.GetAllLinks();
        }

        if (!await ClaimManager.IsUserAdmin())
        {
            CheckAccess();
        }

        var user = (await AuthState.GetAuthenticationStateAsync()).User;

        UserName = user.GetUsername();

        Image = user.GetUserImage();

        GetAccessedPages();

        NavigationManager.LocationChanged += PageChanged;

        SiloContext.NavbarFilterStatusChanged += StateHasChanged;

        var relativeLocation = '/' + (NavigationManager.Uri.Replace(NavigationManager.BaseUri, ""));

        SetNavBread(relativeLocation);

        IsLoading = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var themeGetResult = await JS.InvokeAsync<string?>("localStorage.getItem", "silo-theme");

            bool prefersDark;

            if (themeGetResult.HasValue())
            {
                prefersDark = themeGetResult.Equals("dark");
            }
            else
            {
                prefersDark = await JS.InvokeAsync<bool>("eval",
                    "window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches");

                await JS.InvokeAsync<string?>("localStorage.setItem", "silo-theme", prefersDark ? "dark" : "light");
            }

            IsDarkMode = prefersDark;

            StateHasChanged();

            SiloContext.SetDarkModeStatus(IsDarkMode);
        }
    }

    public async Task OnThemeToggleChanged(bool newValue)
    {
        IsDarkMode = newValue;

        var objectRef = DotNetObjectReference.Create(this);

        await JS.InvokeVoidAsync("siloSetTheme", newValue, true);
    }

    public async Task OnLogoClick(MouseEventArgs e)
    {
        NavigationManager.NavigateTo("/", true);
    }

    public async Task OnLogoutClick(MouseEventArgs e)
    {
        await AuthState.SetUserLoggedOut();

        NavigationManager.NavigateTo("/account/login", true);
    }

    public async Task OnExpandMenuSpanClick(MouseEventArgs e)
    {
        IsProfileActive = false;

        IsSearchActive = "";

        if (IsMenuActive)
        {
            IsMenuActive = false;
        }
        else
        {
            IsMenuActive = true;
        }

        SiloContext.SetTabStatus(IsMenuActive);
    }

    public async Task OnSearchSpanClick(MouseEventArgs e)
    {
        IsMenuActive = false;

        IsProfileActive = false;

        if (IsSearchActive == "span-search-expand")
        {

            IsSearchActive = "";

        }
        else
        {
            IsSearchActive = "span-search-expand";
        }

    }

    public async Task OnProfileClick(MouseEventArgs e)
    {
        NavigationManager.NavigateTo("/account/profile");
    }

    public async Task OnProfileSpanClick(MouseEventArgs e)
    {
        IsMenuActive = false;

        IsSearchActive = "";

        if (IsProfileActive)
        {
            IsProfileActive = false;
        }
        else
        {
            IsProfileActive = true;
        }
    }

    public async Task OnNavLinkClick(string link)
    {
        IsMenuActive = false;

        IsSearchActive = "";

        IsProfileActive = false;

        NavigationManager.NavigateTo(link);
    }

    public TelerikContextMenu<TelerikContextMenuItem> NavContextMenuRef { get; set; }
    public NavbarLink NavContextMenuTarget = null;
    public List<TelerikContextMenuItem> NavContextMenuItems = new();

    public async Task OnNavLinkRightClick(NavbarLink link, MouseEventArgs e)
    {
        NavContextMenuTarget = link;

        var quickLinks = await ClaimManager.GetQuickAccessLinks();

        bool alreadyAdded = quickLinks.Any(p => p.Url == link.Url);
        bool atLimit = quickLinks.Count >= 4;

        string label;
        bool disabled;

        if (alreadyAdded)
        {
            label = "قبلاً افزوده شده";
            disabled = true;
        }
        else if (atLimit)
        {
            label = "دسترسی سریع پر است (حداکثر ۴)";
            disabled = true;
        }
        else
        {
            label = "افزودن به دسترسی سریع";
            disabled = false;
        }

        NavContextMenuItems = new()
        {
            new TelerikContextMenuItem
            {
                Text = label,
                Disabled = disabled,
                Icon = MaterialIconsHelper.Bookmark
            }
        };

        if (NavContextMenuRef is not null)
        {
            await NavContextMenuRef.ShowAsync(e.ClientX, e.ClientY);
        }
    }

    public async Task OnNavContextMenuItemClick(TelerikContextMenuItem item)
    {
        if (NavContextMenuTarget is null || item.Disabled) return;

        item.Disabled = true;

        await ClaimManager.SaveQuickAccessLink(NavContextMenuTarget.Id);

        NavContextMenuTarget = null;

        if (SiloContext.QuickAccessChanged is not null)
            await SiloContext.QuickAccessChanged.Invoke();
    }

    public async Task OnAutoCompleteChange(object selectedPage)
    {
        if (selectedPage is null)
        {
            return;
        }

        foreach (var page in Pages)
        {
            if (page.Value.ToLower() == selectedPage.ToString().ToLower())
            {
                NavigationManager.NavigateTo(page.Key);
            }
        }
    }

    public async Task OnClickBackDrop(MouseEventArgs e)
    {
        IsProfileActive = false;

        IsMenuActive = false;

        SelectedAllTitle = new();

        Items = null;
    }

    public async Task OnCategoryClick(NavbarAllTitle title)
    {
        SelectedAllTitle = title;
    }



    public async Task OnFiltersSpanClick(MouseEventArgs e)
    {
        SiloContext.NavbarFilterClicked?.Invoke();
    }

    #region Bread Crump
    private void PageChanged(object sender, LocationChangedEventArgs args)
    {
        var relativeLocation = '/' + ((NavigationManager)sender).Uri.Replace(((NavigationManager)sender).BaseUri, "");

        SetNavBread(relativeLocation);

        StateHasChanged();
    }

    private void SetNavBread(string relativeLocation)
    {
        NavBreadItems = new();

        foreach (var menuTitle in MenuLinks)
        {
            foreach (var category in menuTitle.Children)
            {
                foreach (var link in category.Children)
                {
                    var formattedLink = link.Url.StartsWith('/') ? link.Url : '/' + link.Url;

                    if (formattedLink == relativeLocation)
                    {
                        NavBreadItems = new()
                        {
                            new()
                            {
                                Id = menuTitle.Id,
                                Title = menuTitle.Title,
                                IconName = menuTitle.IconName,
                                Children = new()
                                {
                                    new()
                                    {
                                        Id = category.Id,
                                        Title = category.Title,
                                        Children = new()
                                        {
                                            link
                                        }
                                    }
                                }
                            }
                        };
                    }
                }
            }
        }
    }

    /// <summary>
    /// Find click menu item from MenuLinks, if found redirect to page /settings/bread/{LinkId}
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public void OnBreadCrumbItemClick(int id)
    {
        NavigationManager.NavigateTo($"/settings/bread/{id}");
    }
    #endregion

    private void CheckAccess()
    {
        List<NavbarAllTitle> accessLinks = new();

        foreach (var title in MenuLinks)
        {
            var accessTitle = accessLinks.FirstOrDefault(t => t.Title == title.Title);

            if (accessTitle is null)
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

                if (titleCategory is null)
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

    private void GetAccessedPages()
    {
        foreach (var title in MenuLinks)
        {
            foreach (var category in title.Children)
            {
                foreach (var link in category.Children)
                {
                    Pages.Add(new()
                    {
                        IsChoosed = false,
                        Key = link.Url,
                        Value = link.Title
                    });
                }
            }
        }
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= PageChanged;
        SiloContext.NavbarFilterStatusChanged -= StateHasChanged;
    }
}
