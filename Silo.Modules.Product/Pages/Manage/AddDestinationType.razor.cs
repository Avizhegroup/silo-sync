using AutoMapper;
using Silo.Application;
using Silo.Application.Features;
using Silo.Application.Shared.Features;
using Silo.Domains.Entities;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddDestinationType
{
    public bool IsLoading = true;
    public string UserId;
    public string MessageText;


    public CreateNewDestinationTypeCommand Request = new();

    public List<GetAllDestinationTypeDto> DestinationTypes;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [CascadingParameter] public DialogFactory Dialog { get; set; }

    public bool IsAllActionControlsChoosen = false;

    public Modal ModalAction { get; set; }
    public Modal ModalMessage { get; set; }

    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
    }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        DestinationTypes = (await Api.SendAsyncObjectByUri<GetAllDestinationTypeVm>(HttpMethod.Get
                , "DestinationType/ReadAll")).Value.List;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        if (Request.Id.HasNoValue())
        {
            bool CheckCode = (await Api.SendAsyncObjectByUri<CheckDestinationTypeCodeVm>(HttpMethod.Get, "DestinationType/CheckCode", Request)).Value.Result;

            if (CheckCode)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness
                , "error");
                return;
            }
               
            var result = (await Api.SendAsyncObjectByUri<CreateNewDestinationTypeVm>(HttpMethod.Post
               , "DestinationType/Create"
               , Request)).Value.Result;

            if (result > 0)
            {
                Request.Id = result;

                await ReloadDestinationTypes();

                Notification.Show(TextResources.APP_StringKeys_Alert_Success
                    , "success");

            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail
                    , "error");

            }

        }
        else
        {

            var response = (await Api.SendAsyncObjectByUri<UpdateDestinationTypeVm>(HttpMethod.Put
               , "DestinationType/Update"
               , Request)).Value.Result;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                   , "success");

        }

        IsLoading = false;

    }
    private async Task ReloadDestinationTypes()
    {
        IsLoading = true;

        DestinationTypes = (await Api.SendAsyncObjectByUri<GetAllDestinationTypeVm>(HttpMethod.Get
               , "DestinationType/ReadAll")).Value.List;

        IsLoading = false;
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        DestinationTypes = (await Api.SendAsyncObjectByUri<GetAllDestinationTypeVm>(HttpMethod.Get
                 , "DestinationType/ReadAll")).Value.List;

        IsLoading = false;

        await ModalAction.Open(new());
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        var resultDialog = await Dialog.ConfirmAsync(
            TextResources.APP_StringKeys_Message_Delete,
            TextResources.APP_StringKeys_Attention,
            okButtonText: TextResources.APP_StringKeys_Approve,
            cancelButtonText: TextResources.APP_StringKeys_Return
        );

        if (!resultDialog)
        {
            return;
        }

        IsLoading = true;

        var result = (await Api.SendAsyncObjectByUri<DeleteDestinationTypeVm>(HttpMethod.Delete
           , "DestinationType/Delete"
           , Request)).Value.Result;

        if (result)
        {
            await ReloadDestinationTypes();

            Request = new CreateNewDestinationTypeCommand();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                , "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail_Delete
                , "error");
        }

        IsLoading = false;
    }
    public async Task OnSelectType(GetAllDestinationTypeDto destinationtype)
    {
        Request = Mapper.Map<CreateNewDestinationTypeCommand>(destinationtype);

        await ModalAction.Close(new());
    }
}
