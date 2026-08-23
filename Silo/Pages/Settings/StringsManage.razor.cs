using Microsoft.Extensions.Configuration;

namespace Silo.Pages.Settings;

public partial class StringsManage
{
    public bool IsLoading = true;
    public List<TelerikDropDownItem> Strings;
    public List<TelerikDropDownItem> SearchedStrings;
    public string SearchText;
    public bool IsEdited = false;

    [Inject] public IConfiguration Configuration { get; set; }

    protected override async Task SiloInitializer()
    {
        Strings = TextResourceTools.GetTextResourceList(Configuration)
                                   .Select(p=> new TelerikDropDownItem()
                                   {
                                       Name = p.Key,
                                       Value = p.Value
                                   })
                                   .ToList();

        SearchedStrings = Strings;

        IsLoading = false;
    }
    
    public async Task OnSaveClick(MouseEventArgs e)
    {
        IsLoading = true;

        foreach (var item in SearchedStrings)
        {
            var stringItem = Strings.FirstOrDefault(p => p.Name.Equals(item.Name));

            if (stringItem is null)
            {
                return;
            }

            stringItem.Value = item.Value;
        }

        TextResourceTools.SaveDictionaryToXml(Strings.ToDictionary(p=>p.Name,p=> p.Value), Configuration);

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
            SearchedStrings = Strings.Where(p => p.Name.Contains(SearchText) || p.Value.Contains(SearchText)).ToList();
        }
    }
}
