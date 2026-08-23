using AutoMapper;
using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddClass
{
    public bool IsLoading = false;
    public SaveProuctClassCommand SaveCommand = new();
    public List<GetAllProductClassVm> ProductClasses;

    public Modal ModalProductClasses { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper{ get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;
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

        ProductClasses = (await Api.PostAsync<List<GetAllProductClassVm>>("SGetAllProductClasses")).Value;

        IsLoading = false;

        await ModalProductClasses.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        if (SaveCommand.Id == 0)
        {
            bool isCodeUniqueness = (await Api.PostAsync<bool>("SCheckClassUniqueness"
                , new KeyValuePair<string, object>("value", SaveCommand.Code))).Value;

            if (!isCodeUniqueness)
            {
                IsLoading = false;

                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");

                return;
            }
        }

        int result = (await Api.PostAsync<int>("SSaveClass"
            , new KeyValuePair<string, object>("productClass", SaveCommand))).Value;

        IsLoading = false;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            if (SaveCommand.Id == 0)
            {
                SaveCommand.Id = result;
            }
            var productClassList = (await Api.PostAsync<List<GetAllProductClassVm>>("SGetAllProductClasses")).Value;

            await FormalCache.UpdateProductClass(productClassList);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public async Task OnChooseProductClass(GetAllProductClassVm productClass)
    {
        SaveCommand = Mapper.Map<SaveProuctClassCommand>(productClass);

        await ModalProductClasses.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SRemoveClass"
            , new KeyValuePair<string, object>("classCode", SaveCommand.Code))).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");

            return;
        }

        SaveCommand = new();

        var productClassList = (await Api.PostAsync<List<GetAllProductClassVm>>("SGetAllProductClasses")).Value;

        await FormalCache.UpdateProductClass(productClassList);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
    }
}
