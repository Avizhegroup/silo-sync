

using Silo.Application.Features;
using System.Text.Json;

namespace Silo.Pages.Reports;

public partial class Error
{
    public bool IsLoading = true;
    public List<GetAllPackingTagsVm> Packings;
    public List<GetAllWithOutEpcTagsVm> NoEpcs;
    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task SiloInitializer()
    {
        string fromDate = PersianCalendarTools.GetUnixPersianTime(-1);

        Packings = (await Api.PostAsyncByOption<List<GetAllPackingTagsVm>>("SRepRegisterTagByTagStatus0"
            , new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.WriteAsString
            }
          , new KeyValuePair<string, object>("FromRegisterShamsiUnixDate", fromDate))).Value;

        NoEpcs = (await Api.PostAsync<List<GetAllWithOutEpcTagsVm>>("SRepRegisterTagWithOutEPC")).Value;

        IsLoading = false;
    }
}
