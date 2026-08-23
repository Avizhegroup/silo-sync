using AutoMapper;
using Newtonsoft.Json;
using Silo.Shared.Components;

namespace Silo.Modules.Product.Pages;
public partial class AddActionType
{
    public bool IsLoading = true;
    public string UserId;
    public string MessageText;
    public bool IsAllActionControlsChoosen = false;
    public CreateNewActionTypeCommand Request = new();
    public List<GetAllActionTypesDto> ActionTypes ;
    public List<GetAllDocumentStatusVm> DocumentStatuses;
    public List<GetAllWarehouseTypesDto> Warehouses { get; set; }
    public List<GetAllActionTypeControlsDto> ActionControls;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [CascadingParameter] public DialogFactory Dialog { get; set; }

    public Modal ModalAction { get; set; }
    public Modal ModalMessage { get; set; }
    public Modal ModalActionControls { get; set; }


    public async Task OnRefreshClick(MouseEventArgs e)
    {
        Request = new();
    }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Warehouses = (await Api.SendAsyncObjectByUri<GetAllWarehouseTypesVm>(HttpMethod.Get
               , "WarehouseType/GetAll")).Value.List;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
        , new GetAllDocumentStatusVmContext())).Value;

        ActionControls = (await Api.SendAsyncObjectByUri<GetAllActionTypeControlsVm>(HttpMethod.Get
         , "ActionTypeControls/GetAll")).Value.List;

        IsLoading = false;
    }

    public async Task OnOpenModalClick(MouseEventArgs e)
    {
        IsLoading = true;

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
              , "ActionType/ReadAll")).Value.List;

        IsLoading = false;

        await ModalAction.Open(new());
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        BuildActiveControlsJson();

        if (Request.Id.HasNoValue())
        {
            bool CheckCode = (await Api.SendAsyncObjectByUri<GetActionTypeByCodeVm>(HttpMethod.Get, "ActionType/CheckCode", Request)).Value.Result;

            if (CheckCode)
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Code_Uniqueness
                , "error");
                return;
            }

            var result = (await Api.SendAsyncObjectByUri<CreateNewActionTypeVm>(HttpMethod.Post
               , "ActionType/Create"
               , Request)).Value.Result;

            if (result > 0)
            {
                Request.Id = result; 

                await ReloadActionTypes();

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

            var response = (await Api.SendAsyncObjectByUri<UpdateActionTypeByIdVm>(HttpMethod.Put
               , "ActionType/Update"
               , Request)).Value.Result;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                   , "success");

        }

        IsLoading = false;

    }

    private void BuildActiveControlsJson()
    {
        var dict = ActionControls.ToDictionary(
            c => c.Code,
            c => Request.ChoosenActionControls.Contains(c.Code)
        );

        Request.ActiveControls = JsonConvert.SerializeObject(dict);
    }

    public async Task OnSelectType(GetAllActionTypesDto actiontype)
    {
        Request = Mapper.Map<CreateNewActionTypeCommand>(actiontype);

        if (!string.IsNullOrWhiteSpace(actiontype.From))
        {
            var fromCodes = actiontype.From.Split(',', StringSplitOptions.RemoveEmptyEntries);
            Request.ChoosenFromWarehouseTypes = Warehouses
                .Where(w => fromCodes.Contains(w.Code))
                .Select(w => w.Code!)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(actiontype.To))
        {
            var toCodes = actiontype.To.Split(',', StringSplitOptions.RemoveEmptyEntries);
            Request.ChoosenToWarehouseTypes = Warehouses
                .Where(w => toCodes.Contains(w.Code))
                .Select(w => w.Code!)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(actiontype.DocStatusPermitted))
        {
            Request.ChoosenDocumentPermittedStatuses = actiontype.DocStatusPermitted
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(actiontype.DocStatusChange))
        {
            Request.ChoosenDocumentChangeStatuses = actiontype.DocStatusChange
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }

        if (actiontype.ActiveControls.Contains(","))
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(actiontype.ActiveControls);

            Request.ChoosenActionControls = dict
                .Where(x => x.Value)
                .Select(x => x.Key)   
                .ToList();
        }

        await ModalAction.Close(new());
    }

    private async Task ReloadActionTypes()
    {
        IsLoading = true;

        var ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Warehouses = (await Api.SendAsyncObjectByUri<GetAllWarehouseTypesVm>(HttpMethod.Get
               , "WarehouseType/GetAll")).Value.List;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
        , new GetAllDocumentStatusVmContext())).Value;

        IsLoading = false;
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

        var result = (await Api.SendAsyncObjectByUri<DeleteActionTypeByIdVm>(HttpMethod.Delete
           , "ActionType/Delete"
           , Request)).Value.Result; 

        if (result)
        {
            await ReloadActionTypes();

            Request = new CreateNewActionTypeCommand();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                , "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail
                , "error");
        }

        IsLoading = false;
    }

    public async Task OnActionControlsModalOpen()
    {
        foreach (GetAllActionTypeControlsDto control in ActionControls)
        {
            control.IsChoosen = Request.ChoosenActionControls.Any(p => p.Equals(control.Code));
        }

        await ModalActionControls.Open(new());
    }

    public async Task OnActionControlsModalClose()
    {
        Request.ChoosenActionControls.Clear();

        foreach (var control in ActionControls)
        {
            if (control.IsChoosen)
            {
                Request.ChoosenActionControls.Add(control.Code);
            }
        }
    }
    public string GetActionControlsText() =>
    Request.ChoosenActionControls.Count > 0
     ? string.Join(", ",
         Request.ChoosenActionControls
             .Select(code => ActionControls.First(c => c.Code == code).Name))
     : string.Empty;

}
