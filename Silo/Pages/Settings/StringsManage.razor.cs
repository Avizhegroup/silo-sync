namespace Silo.Pages.Settings;
public partial class StringsManage
{
    public bool IsLoading = true;
    public List<StringResourceModel> Strings = new();
    public List<StringResourceModel> SearchedStrings = new();
    public string SearchText;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache Cache { get; set; }

    protected override async Task SiloInitializer()
    {
        var resources = await Cache.GetTextResources();

        Strings = resources
            .Select(x => new StringResourceModel
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .ToList();

        SearchedStrings = Strings;

        IsLoading = false;
    }

    public async Task OnSaveClick(MouseEventArgs e)
    {
        if (!IsValidForSave())
        {
            return;
        }

        IsLoading = true;

        SaveTextResourcesCommand command = new()
        {
            Items = Strings
                .Where(x => !x.IsDeleted)
                .Select(x => new TextResourceDto
                {
                    Id = x.Id,
                    Key = x.Key,
                    Value = x.Value
                })
                .ToList(),
            DeletedIds = Strings
                .Where(x => x.IsDeleted && x.Id > 0)
                .Select(x => x.Id)
                .ToList()
        };

        var result = await Api.SendAsyncObjectByUri<SaveTextResourcesVm>(HttpMethod.Post
            , "TextResource/Save", command, new SaveTextResourcesVmContext());

        if (result.Successful && result.Value is { Result: true })
        {
            var updated = await Cache.RefreshTextResources();

            Strings = updated
                .Select(x => new StringResourceModel
                {
                    Id = x.Id,
                    Key = x.Key,
                    Value = x.Value
                })
                .ToList();

            SearchedStrings = Strings;
        }

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        IsLoading = false;
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        if (SearchText.HasNoValue())
        {
            SearchedStrings = Strings;
        }
        else
        {
            SearchedStrings = Strings
                .Where(p => (p.Key.Contains(SearchText) || p.Value.Contains(SearchText)) && !p.IsDeleted)
                .ToList();
        }
    }

    public void OnAddClick(MouseEventArgs e)
    {
        StringResourceModel newItem = new()
        {
            Id = 0,
            Key = "APP_StringKeys_",
            Value = string.Empty,
            IsNew = true
        };

        Strings.Insert(0, newItem);

        SearchedStrings = Strings.Where(x => !x.IsDeleted).ToList();
    }

    public void OnDeleteClick(StringResourceModel item)
    {
        item.IsDeleted = true;

        SearchedStrings = Strings.Where(x => !x.IsDeleted).ToList();
    }

    public bool IsValidForSave()
    {
        var activeStrings = new List<StringResourceModel>();

        foreach (var item in Strings.Where(x => !x.IsDeleted))
        {
            item.Key = item.Key.Trim();

            if (item.Key.HasNoValue())
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

                return false;
            }

            if (activeStrings.Any(x => x.Key.Equals(item.Key, StringComparison.Ordinal)))
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Remote, item.Key), "error");

                return false;
            }

            activeStrings.Add(item);
        }

        return true;
    }
}
