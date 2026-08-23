using AutoMapper;
using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddSubGroup
{
    public bool IsLoading = false;
    public SaveProuctSubGroupCommand SaveCommand = new();
    public List<GetAllProductSubGroupVm> ProductSubGroups;
    public List<GetAllProductGroupVm> Groups;

    public Modal ModalProductSubGroups { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper{ get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Groups = await FormalCache.GetGroups(); 
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        SaveCommand = new();
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (SaveCommand.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(new());
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        ProductSubGroups = (await Api.PostAsync<List<GetAllProductSubGroupVm>>("SGetAllProductSubGroups")).Value;

        IsLoading = false;

        await ModalProductSubGroups.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        if (SaveCommand.Id == 0)
        {
            bool isCodeUniqueness = (await Api.PostAsync<bool>("SCheckSubGroupUniqueness",
                new KeyValuePair<string, object>("value", SaveCommand.Code))).Value;

            if (!isCodeUniqueness)
            {
                IsLoading = false;
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
                return;
            }
        }

        int result = (await Api.PostAsync<int>("SSaveSubGroup",
            new KeyValuePair<string, object>("subGroup", SaveCommand))).Value;

        IsLoading = false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            if (SaveCommand.Id == 0)
            {
                SaveCommand.Id = result;
            }

            var subGroups = (await Api.PostAsync<List<GetAllProductSubGroupVm>>("SGetAllProductSubGroups")).Value;
            await FormalCache.UpdateSubGroups(subGroups);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }


    public async Task OnChooseProductClass(GetAllProductSubGroupVm productClass)
    {
        SaveCommand = Mapper.Map<SaveProuctSubGroupCommand>(productClass);

        await ModalProductSubGroups.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SRemoveSubGroup",
            new KeyValuePair<string, object>("subGroupCode", SaveCommand.Code))).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");
            return;
        }

        SaveCommand = new();

        var subGroups = (await Api.PostAsync<List<GetAllProductSubGroupVm>>("SGetAllProductSubGroups")).Value;
        await FormalCache.UpdateSubGroups(subGroups);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        StateHasChanged();
    }

}
