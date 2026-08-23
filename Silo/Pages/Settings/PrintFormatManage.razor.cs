namespace Silo.Pages.Settings;

public partial class PrintFormatManage
{
    public bool IsLoading = true;
    public CreatePrintFormatCommand Request = new();
    public List<string> SelectedPageTitles = new();
    public List<GetAllPrintFormatDto> PrintFormats;
    public List<NavbarAllTitle> Links;
    public List<NavbarLink> FlatLinks = new();
    public List<NavbarLink> FilteredLinks = new();
    public string LinkFilter = string.Empty;

    public Modal ModalPrintFormats { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Links = await ClaimManager.GetAllLinks();

        FlatLinks = Links.SelectMany(l1 => l1.Children.SelectMany(l2 => l2.Children.Select(l3 => l3))).ToList();

        FilteredLinks = FlatLinks;

        IsLoading = false;
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        PrintFormats = (await Api.SendAsyncObjectByUri<GetAllPrintFormatVm>(HttpMethod.Get
            , "PrintFormat/GetAll")).Value.List;

        IsLoading = false;

        await ModalPrintFormats.Open(new());
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
        SelectedPageTitles = new();
        LinkFilter = string.Empty;
        FilteredLinks = FlatLinks;
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (!Request.Id.HasValue)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");
            return;
        }

        var confirmed = await Dialog.ConfirmAsync(TextResources.APP_StringKeys_Message_Delete
            , TextResources.APP_StringKeys_Attention);

        if (!confirmed)
        {
            return;
        }

        IsLoading = true;

        var result = (await Api.SendAsyncObjectByUri<DeletePrintFormatVm>(HttpMethod.Delete
            , "PrintFormat/Delete"
            , new DeletePrintFormatCommand { Id = Request.Id.Value })).Value?.Result;

        if (result == true)
        {
            Request = new();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        Request.PageTitle = string.Join("|", SelectedPageTitles);

        var result = (await Api.SendAsyncObjectByUri<CreatePrintFormatVm>(HttpMethod.Post
            , "PrintFormat/Create"
            , Request)).Value?.Result;

        if (result > 0)
        {
            Request = new();
            SelectedPageTitles = new();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public void OnLinkFilterChanged(string value)
    {
        LinkFilter = value;

        FilteredLinks = value.HasNoValue()
            ? FlatLinks
            : FlatLinks.Where(l =>
                l.Title.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                l.Url.Contains(value, StringComparison.OrdinalIgnoreCase))
              .ToList();
    }

    public void OnLinkCheckedChanged(NavbarLink link, bool isChecked)
    {
        var segment = link.Url.Replace("/", "-");

        if (isChecked)
        {
            if (!SelectedPageTitles.Contains(segment))
                SelectedPageTitles.Add(segment);
        }
        else
        {
            SelectedPageTitles.Remove(segment);
        }

        Request.PageTitle = SelectedPageTitles.Count > 0
            ? string.Join("|", SelectedPageTitles)
            : null;
    }

    public async Task OnChoosePrintFormat(GetAllPrintFormatDto printFormat)
    {
        Request = new()
        {
            Id = printFormat.Id,
            Name = printFormat.Name,
            PageTitle = printFormat.PageTitle,
            Path = printFormat.Path
        };

        SelectedPageTitles = printFormat.PageTitle?.Split('|').ToList() ?? new();

        await ModalPrintFormats.Close(new());
    }
}
