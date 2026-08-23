using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Silo.Application.Features;

namespace Silo.Pages.Settings;
public partial class ReportLinks
{
    public bool IsLoading = true;
    public bool IsShownTreeview = false;
    public SaveMenuLinkOfDynamicReportCommand Request = new();
    public GetReportFormatByIdVm Format = new();
    public List<UserChoosableDto> AllUsers;
    public List<UserChoosableDto> Users;
    public string UserSearch = string.Empty;
    public List<NavbarAllTitle> Links;
    public List<TreeviewNode> Nodes;
    public int ActiveStepperIndex = 0;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IClaimManager ClaimManager { get; set; }

    [Parameter] public int? FormatId { get; set; }
    [Parameter] public string? Link { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        if (FormatId is not null)
        {
            Format = (await Api.PostAsyncByUriAndContext<GetReportFormatByIdVm>("wms/ReportFormat"
                              , "SGetReportFormatById"
                              , new GetReportFormatByIdVmContext()
                              , new KeyValuePair<string, object>("query", new GetReportFormatByIdQuery()
                              {
                                  FormatId = (int)FormatId
                              }))).Value;
        }

        if (Link.HasValue())
        {
            Request.Url = Link.Replace(",", "/");
        }

        await LoadUsers();

        await LoadPreviousData();

        await LoadTreeviewAndLink();

        IsShownTreeview = true;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (!IsFormValid())
        {
            return;
        }

        IsLoading = true;

        foreach (var user in Users.Where(p => p.IsChoosed))
        {
            Request.UserIds.Add(user.Id);
        }

        bool result = (await Api.PostAsyncByUri<bool>("wms/ReportFormat"
                               , "SSaveLinkForReportFormat"
                               , new KeyValuePair<string, object>("command", Request))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request.Title = string.Empty;

        UserSearch = string.Empty;

        Users = AllUsers;

        Users.ForEach(user => user.IsChoosed = false);

        Request.SelectedCategoryId = null;

        ActiveStepperIndex = 0;

        foreach (var treeNode in Nodes)
        {
            foreach (var innerNode in treeNode.nodes)
            {
                innerNode.isSelected = false;
            }
        }
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (UserSearch.HasValue())
        {
            Users = AllUsers.Where(p => p.Name.Contains(UserSearch)
                                                  || p.UserName.Contains(UserSearch))
                            .ToList();
        }
        else
        {
            Users = AllUsers;
        }
    }

    public async Task OnNodeClick(TreeviewNode node)
    {
        if (!node.selectable)
        {
            return;
        }

        foreach (var treeNode in Nodes)
        {
            foreach (var innerNode in treeNode.nodes)
            {
                innerNode.isSelected = false;
            }
        }

        node.isSelected = true;

        Request.SelectedCategoryId = int.Parse(node.thisnodeid);
    }

    #region Private
    private List<TreeviewNode> ConvertToTreeviewNodes(List<NavbarAllTitle> navbarAllTitles)
    {
        List<TreeviewNode> treeviewNodes = new();

        foreach (var navbarAllTitle in navbarAllTitles)
        {
            TreeviewNode node = new()
            {
                thisnodeid = navbarAllTitle.Id.ToString(),
                text = navbarAllTitle.Title,
                selectable = false,
                href = null,
                value = navbarAllTitle.IconName,
                index = 0
            };

            node.nodes = ConvertCategoriesToTreeviewNodes(navbarAllTitle.Children.ToArray(), node);

            treeviewNodes.Add(node);
        }

        return treeviewNodes;
    }

    private TreeviewNode[] ConvertCategoriesToTreeviewNodes(NavbarCategory[] categories, TreeviewNode parentNode)
    {
        List<TreeviewNode> treeviewNodes = new();

        foreach (var category in categories)
        {
            TreeviewNode node = new()
            {
                thisnodeid = category.Id.ToString(),
                text = category.Title,
                selectable = true,
                nodes = [],
                href = null,
                value = null,
                index = 0
            };

            if (category.Id == Request.SelectedCategoryId)
            {
                node.isSelected = true;

                parentNode.IsExpanded = true;
            }

            treeviewNodes.Add(node);
        }

        return treeviewNodes.ToArray();
    }

    private bool IsFormValid()
    {
        if (Users.Neither(p => p.IsChoosed))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ChooseUser, "error");

            return false;
        }

        if (Request.SelectedCategoryId is null)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ChooseCategory, "error");

            return false;
        }

        return true;
    }

    private async Task LoadUsers()
    {
        var applicationUsers = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
                new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") })).Value;

        AllUsers = Mapper.Map<List<UserChoosableDto>>(applicationUsers);

        Users = AllUsers;
    }

    private async Task LoadTreeviewAndLink()
    {
        Links = await ClaimManager.GetAllLinks();

        Nodes = ConvertToTreeviewNodes(Links);
    }

    private async Task LoadPreviousData()
    {
        var loadedData = (await Api.PostAsyncByUriAndContext<GetMenuLinkOfDynamicReportVm>("wms/ReportFormat"
                              , "SGetLinkForReportFormat"
                              , new GetMenuLinkOfDynamicReportVmContext()
                              , new KeyValuePair<string, object>("query", new GetMenuLinkOfDynamicReportQuery()
                              {
                                  FormatId = (int)FormatId,
                                  FullUrl = Request.Url
                              }))).Value;

        Request.UserIds = loadedData.UserIds;

        Request.Title = loadedData.Title;

        Request.SelectedCategoryId = loadedData.CategoryId;
    }
    #endregion
}
