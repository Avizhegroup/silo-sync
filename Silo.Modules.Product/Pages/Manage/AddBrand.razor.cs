using Silo.Application;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddBrand
{
    public bool IsLoading = false;
    public GetAllProductBrandVm Request = new();
    public List<GetAllProductBrandVm> Brands;

    public Modal ModalBrands { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public IFormalDataCache FormalCache { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
    }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;
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

        Brands = (await Api.PostAsync<List<GetAllProductBrandVm>>("SGetAllProductBrands")).Value;

        IsLoading = false;

        await ModalBrands.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        if (Request.Id == 0)
        {
            bool isCodeUniqueness = (await Api.PostAsync<bool>(
                "SCheckBrandUniqueness",
                new KeyValuePair<string, object>("value", Request.Code)
            )).Value;

            if (!isCodeUniqueness)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness, "error");
                return;
            }
        }

        int result = (await Api.PostAsync<int>(
            "SSaveBrand",
            new KeyValuePair<string, object>("brand", Request)
        )).Value;

        if (result > 0)
        {
            if (Request.Id == 0)
            {
                Request.Id = result;
            }

            var brands = (await Api.PostAsync<List<GetAllProductBrandVm>>(
                "SGetAllProductBrands"
            )).Value;

            await FormalCache.UpdateBrands(brands);

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        StateHasChanged();
    }


    public async Task OnChooseBrand(GetAllProductBrandVm brand)
    {
        Request = brand;

        await ModalBrands.Close(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>(
            "SRemoveBrand",
            new KeyValuePair<string, object>("brandCode", Request.Code)
        )).Value;

        IsLoading = false;

        if (result == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_CascadeDelete, "error");
            return;
        }

        Request = new();

        var brands = (await Api.PostAsync<List<GetAllProductBrandVm>>(
            "SGetAllProductBrands"
        )).Value;

        await FormalCache.UpdateBrands(brands);

        Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        StateHasChanged();
    }

}
