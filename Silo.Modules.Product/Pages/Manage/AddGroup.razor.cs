using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddGroup
{
    public bool IsLoading = false;
    public GetAllProductGroupVm Request = new();
    public List<GetAllProductGroupVm> Groups;

    public Modal ModalGroups { get; set; }
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

        Groups = (await Api.PostAsync<List<GetAllProductGroupVm>>("SGetAllProductGroups")).Value;

        IsLoading = false;

        await ModalGroups.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (Request.Id == 0)
        {
            bool isCodeUniqueness = (await Api.PostAsync<bool>(
                "SCheckGroupUniqueness",
                new KeyValuePair<string, object>("value", Request.Code)
            )).Value;

            if (!isCodeUniqueness)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
                return;
            }
        }

        int result = (await Api.PostAsync<int>(
            "SSaveGroup",
            new KeyValuePair<string, object>("group", Request)
        )).Value;

        if (result > 0)
        {
            if (Request.Id == 0)
            {
                Request.Id = result;
            }

            var groups = (await Api.PostAsync<List<GetAllProductGroupVm>>("SGetAllProductGroups")).Value;

            await FormalCache.UpdateGroups(groups);


            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }


    public async Task OnChooseGroup(GetAllProductGroupVm group)
    {
        Request = group;

        await ModalGroups.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SRemoveGroup",
            new KeyValuePair<string, object>("groupCode", Request.Code))).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");
            return;
        }

        Request = new();

        var groups = (await Api.PostAsync<List<GetAllProductGroupVm>>("SGetAllProductGroups")).Value;
        await FormalCache.UpdateGroups(groups);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
    }

}
