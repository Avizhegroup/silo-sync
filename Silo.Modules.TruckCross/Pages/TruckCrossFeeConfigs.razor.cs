using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Pages;
public partial class TruckCrossFeeConfigs
{
    public bool IsLoading = true;
    public List<GetAllTruckCrossShipmentFeeVm> Fees = new();
    public SaveTruckCrossShipmentFeeConfigsCommand Command = new();
    public List<GetAllTruckCompaniesVm> Companies = new();
    public List<GetAllTruckCrossProductTypeVm> ProductTypes = new();
    public List<GetAllTruckCrossCustomerVm> Customers = new();
    public List<GetAllTruckCrossShipmentVm> Shipments = new();

    public Modal ModalFees { get; set; }
    public Modal ModalDelete { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Companies = (await Api.PostAsyncByUri<List<GetAllTruckCompaniesVm>>("wms/TruckCross", "SGetAllTruckCompany")).Value;

        ProductTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross", "SGetAllTruckCrossProductType")).Value;

        Customers = (await Api.PostAsyncByUri<List<GetAllTruckCrossCustomerVm>>("wms/TruckCross", "SGetAllTruckCrossCustomer")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        var result = (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCrossShipmentFee"
                    , new KeyValuePair<string, object>("truckCrossShipmentFee", Command))).Value;

        IsLoading = false;

        if (result > 0)
        {
            Command.Id = result;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        Fees = (await Api.PostAsyncByUriAndContext<List<GetAllTruckCrossShipmentFeeVm>>("wms/TruckCross"
            , "SGetAllTruckCrossShipmentFee"
            , new GetAllTruckCrossShipmentFeeVmContext())).Value;

        IsLoading = false;

        await ModalFees.Open(new());
    }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Command = new();
    }

    public async Task OnChooseFee(GetAllTruckCrossShipmentFeeVm fee)
    {
        Command = Mapper.Map<SaveTruckCrossShipmentFeeConfigsCommand>(fee);

        await ModalFees.Close(new());
    }

    #region Remove
    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Command.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(new());
    }

    public async Task OnConfirmRemove(MouseEventArgs e)
    {
        IsLoading = true;

        var result = (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossShipmentFee"
            , new KeyValuePair<string, object>("id", Command.Id))).Value;

        IsLoading = false;

        if (result)
        {
            Command = new();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }
    #endregion
}
