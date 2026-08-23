using AutoMapper;
using Silo.Application;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddProductType
{
    public bool IsLoading = true;
    public string UserId;
    public string MessageText;
    public SaveProductTypeCommand Request = new();
    public List<GetAllProductTypeVm> ProductTypes;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    public Modal ModalDetails { get; set; }
    public Modal ModalMessage { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        ProductTypes = (await Api.PostAsync<List<GetAllProductTypeVm>>("SPSearchProductTypeWeb",
            new KeyValuePair<string, object>[] { new("search", new GetAllProductTypeQuery()
            {
                MCode  = "-1",
                MTitle = "-1"
            }) })).Value;

        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = false;
    }

    public async Task OnSelectType(GetAllProductTypeVm type)
    {
        Request = Mapper.Map<SaveProductTypeCommand>(type);

        await ModalDetails.Close(new());
    }

    public async Task OnSaveProductType(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SSaveProductType",
                 new("ProductTypeTitle", Request.Title),
                 new("ProductTypeParentId", "1"),
                 new("ProductTypeParentsId", "0"),
                 new("ProductTypeCode", Request.Code),
                 new("userToken", UserId))).Value;

        if (result)
        {
            MessageText = TextResources.APP_StringKeys_Alert_Success;

            ProductTypes = (await Api.PostAsync<List<GetAllProductTypeVm>>("SPSearchProductTypeWeb",
            new KeyValuePair<string, object>[] { new("search", new GetAllProductTypeQuery()
            {
                MCode  = "-1",
                MTitle = "-1"
            }) })).Value;
        }
        else
        {
            MessageText = TextResources.APP_StringKeys_Alert_Fail;
        }

        IsLoading = false;

        var ProductType= (await Api.PostAsync<List<GetAllProductTypeVm>>("SGetAllProductType"
      , new("userToken", "")
      , new("haveNotSelect", false))).Value;

        await FormalCache.UpdateType(ProductType);

        await ModalMessage.Open(e);
    }
}
