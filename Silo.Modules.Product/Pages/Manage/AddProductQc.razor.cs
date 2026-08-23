using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddProductQc
{
    public bool IsLoading = false;
    public GetAllProductQcsVm Request = new();
    public List<GetAllProductQcsVm> Qcs;

    public Modal ModalQc { get; set; }
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
        if (Request.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(new());
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        Qcs = (await Api.PostAsync<List<GetAllProductQcsVm>>("SGetAllQcs")).Value;

        IsLoading = false;

        await ModalQc.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        
        if (Request.Id == 0)
        {
            bool isCodeUnique = (await Api.PostAsync<bool>("SCheckQcUniqueness",
                new KeyValuePair<string, object>("value", Request.Code))).Value;

            if (!isCodeUnique)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
                return;
            }
        }

        int result = (await Api.PostAsync<int>("SSaveQc",
            new KeyValuePair<string, object>("qc", Request))).Value;

        if (result > 0)
        {
            
            if (Request.Id == 0)
            {
                Request.Id = result;
            }

            Qcs = (await Api.PostAsync<List<GetAllProductQcsVm>>(
                "SGetAllProductStatus",
                new("userToken", ""),
                new("haveNotSelect", false))).Value;

            await FormalCache.UpdateQcs(Qcs);

         
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
           
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }


    public async Task OnChooseBrand(GetAllProductQcsVm qc)
    {
        Request = qc;

        await ModalQc.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

     
        int result = (await Api.PostAsync<int>("SRemoveQc",
            new KeyValuePair<string, object>("qc", Request.Code))).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");
            return;
        }

       
        Request = new();

        Qcs = (await Api.PostAsync<List<GetAllProductQcsVm>>(
            "SGetAllProductStatus",
            new KeyValuePair<string, object>("userToken", ""),
            new KeyValuePair<string, object>("haveNotSelect", false))).Value;


        await FormalCache.UpdateQcs(Qcs);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        StateHasChanged();
    }

}
