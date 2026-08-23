using AutoMapper;
using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;

public partial class AddSize
{
    public bool IsLoading = true;
    public SaveProductSizeCommand Request = new();
    public List<GetAllProductSizeVm> Sizes;
    public string UserToken = string.Empty;

    public Modal ModalSizes { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        UserToken = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = false;
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Request.Code.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(new());
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        Sizes = (await Api.PostAsync<List<GetAllProductSizeVm>>("SGetAllSizes")).Value;

        IsLoading = false;

        await ModalSizes.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        
        if (Request.Id == 0)
        {
            bool isCodeUnique = (await Api.PostAsync<bool>(
                "SCheckSizeUniqueness",
                new KeyValuePair<string, object>("value", Request.Code)
            )).Value;

            if (!isCodeUnique)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
                return;
            }
        }

        int result = (await Api.PostAsync<int>(
            "SSaveSize",
            new KeyValuePair<string, object>("size", Request)
        )).Value;

        if (result > 0)
        {
            
            if (Request.Id == 0)
            {
                Request.Id = result;
            }

            
            var sizes = (await Api.PostAsync<List<GetAllProductSizeTitleAndCodeVm>>(
                "SGetAllProductPropertyC",
                new("userToken", ""),
                new("haveNotSelect", false)
            )).Value;

            await FormalCache.UpdateSizes(sizes);

            
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
          
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }


    public async Task OnChooseSize(GetAllProductSizeVm size)
    {
        Request = Mapper.Map<SaveProductSizeCommand>(size);

        await ModalSizes.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>(
            "SRemoveSize",
            new KeyValuePair<string, object>("sizeCode", Request.Code)
        )).Value;

        IsLoading = false;

        if (result == -2)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_HaveChild, "error");
            return;
        }

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            return;
        }
       
        Request = new();

        var sizes = (await Api.PostAsync<List<GetAllProductSizeTitleAndCodeVm>>(
            "SGetAllProductPropertyC",
            new("userToken", ""),
            new("haveNotSelect", false)
        )).Value;

        
        await FormalCache.UpdateSizes(sizes);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        StateHasChanged();
    }

}
