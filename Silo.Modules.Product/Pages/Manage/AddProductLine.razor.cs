using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;

public partial class AddProductLine
{
    public bool IsLoading = false;
    public GetAllLinesVm Request = new();
    public List<GetAllLinesVm> Lines;
    public Modal ModalLines { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }


    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Request.Code))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");
            return;
        }

        if (ModalDelete != null)
            await ModalDelete.Open(new());
        
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;


        Lines = (await Api.PostAsyncByContext<List<GetAllLinesVm>>("SGetAllLines"
               , new GetLineContext())).Value;

        IsLoading = false;

        await ModalLines.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (string.IsNullOrWhiteSpace(Request.Data))
        {
            Request.Data = ""; 
        }

        bool isCodeUnique = (await Api.PostAsync<bool>(
            "SCheckLineUniqueness",
            new KeyValuePair<string, object>("value", Request.Code)
        )).Value;

        if (!isCodeUnique)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
            return;
        }

        int result = (await Api.PostAsync<int>(
            "SSaveLine",
            new KeyValuePair<string, object>("line", Request)
        )).Value;

        if (result > 0)
        {
            var lines = (await Api.PostAsync<List<GetAllLinesVm>>("SGetAllLines")).Value;
            await FormalCache.UpdateLines(lines);

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
        }
        else if (result == -2)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Title_Uniqueness, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }

    public async Task OnChooseLine(GetAllLinesVm line)
    {
        Request = line;

        await ModalLines.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SRemoveLine",
            new KeyValuePair<string, object>("lineCode", Request.Code))).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");
            return;
        }

        Request = new();

        var lines = (await Api.PostAsync<List<GetAllLinesVm>>("SGetAllLines")).Value;
        await FormalCache.UpdateLines(lines);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
    }
}
