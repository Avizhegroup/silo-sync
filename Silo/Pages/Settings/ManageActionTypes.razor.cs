using System;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json;
using Silo.Domains.Entities;

namespace Silo.Pages.Settings;
public partial class ManageActionTypes
{
    public bool IsLoading = true;
    public bool IsAllWarehouseTypeChoosen = false;
    public bool IsAllDocumentStatusChoosen = false;
    public bool IsAllActionControlsChoosen = false;
    /// <summary>
    /// Prevent from multiselect of document status in modal Document status
    /// </summary>
    public bool IsModalDocumentStatusOpenedInNonPermittedMode = false;
    public CreateNewActionTypeCommand Request = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public List<GetAllDocumentStatusVm> DocumentStatuses;
    public List<GetAllWarehouseTypesDto> WarehouseTypes;
    public bool IsModalOpenedInFrom = false;
    public List<GetAllActionTypeControlsDto> ActionControls;

    public string ModalWarehouseTypesTitle => IsModalOpenedInFrom
        ? $"{TextResources.APP_StringKeys_Field_Warehouse_Operational_Type} {TextResources.APP_StringKeys_From}"
        : $"{TextResources.APP_StringKeys_Field_Warehouse_Operational_Type} {TextResources.APP_StringKeys_Destination}";
    public Modal ModalActionTypes { get; set; }
    public Modal ModalWarehouseTypes { get; set; }
    public Modal ModalDocumentStatus { get; set; }
    public Modal ModalActionControls { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        WarehouseTypes = (await Api.SendAsyncObjectByUri<GetAllWarehouseTypesVm>(HttpMethod.Get
            , "WarehouseType/GetAll")).Value.List;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
                , new GetAllDocumentStatusVmContext())).Value;

        ActionControls = (await Api.SendAsyncObjectByUri<GetAllActionTypeControlsVm>(HttpMethod.Get
           , "ActionTypeControls/GetAll")).Value.List;

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        var result = (await Api.SendAsyncObjectByUri<CreateNewActionTypeVm>(HttpMethod.Post
           , "ActionType/Create"
           , Request)).Value.Result;

        if (result > 0)
        {
            await ReloadActionTypes();

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

    public async Task OnRefreshClick()
    {
        Request = new();
    }

    public async Task OnRemoveClick()
    {
        var resultDialog = await Dialog.ConfirmAsync(TextResources.APP_StringKeys_Message_Delete, TextResources.APP_StringKeys_Attention);

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

    public async Task OnChooseActionType(GetAllActionTypesDto actionType)
    {
        Request = Mapper.Map<CreateNewActionTypeCommand>(actionType);

        if (actionType.DocStatusChange.Contains(","))
        {
            Request.ChoosenDocumentChangeStatuses = actionType.DocStatusChange
                .Split(',')
                .Select(p => int.Parse(p))
                .ToList();
        }
        else
        {
            Request.ChoosenDocumentChangeStatuses = new() 
            {
                int.Parse(actionType.DocStatusChange)
            };
        }

        if (actionType.DocStatusPermitted.Contains(","))
        {
            Request.ChoosenDocumentPermittedStatuses = actionType.DocStatusPermitted
                .Split(',')
                .Where(p => p.HasValue())
                .Select(int.Parse)
                .ToList();
        }
        else
        {
            Request.ChoosenDocumentPermittedStatuses = new()
            {
                int.Parse(actionType.DocStatusPermitted)
            };
        }

        if (actionType.ActiveControls.Contains(","))
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(actionType.ActiveControls);

            Request.ChoosenActionControls = dict
                .Where(x => x.Value)
                .Select(x => x.Key)   
                .ToList();

        }

        if (actionType.From.Contains(","))
        {
            Request.ChoosenFromWarehouseTypes = actionType.From
                .Split(',')
                .Where(p => p.HasValue())
                .ToList();
        }

        if (actionType.To.Contains(","))
        {
            Request.ChoosenToWarehouseTypes = actionType.To
                .Split(',')
                .Where(p => p.HasValue())
                .ToList();
        }

        await ModalActionTypes.Close(new());
    }

    public async Task OnOpenModalClick()
    {
        await ReloadActionTypes();

        await ModalActionTypes.Open(new());
    }

    public async Task OnWarehouseTypesModalClick(bool isModalOpenedInFrom)
    {
        IsModalOpenedInFrom = isModalOpenedInFrom;

        foreach (var type in WarehouseTypes)
        {
            if (IsModalOpenedInFrom)
            {
                type.IsChoosen = Request.ChoosenFromWarehouseTypes.Any(p => p.Equals(type.Code));
            }
            else
            {
                type.IsChoosen = Request.ChoosenToWarehouseTypes.Any(p => p.Equals(type.Code));
            }
        }

        await ModalWarehouseTypes.Open(new());
    }

    public async Task OnWarehouseTypesModalClose()
    {
        if (IsModalOpenedInFrom)
        {
            Request.ChoosenFromWarehouseTypes.Clear();
        }
        else
        {
            Request.ChoosenToWarehouseTypes.Clear();
        }

        foreach (var type in WarehouseTypes)
        {
            if (type.IsChoosen)
            {
                if (IsModalOpenedInFrom)
                {
                    Request.ChoosenFromWarehouseTypes.Add(type.Code);
                }
                else
                {
                    Request.ChoosenToWarehouseTypes.Add(type.Code);
                }
            }
        }
    }

    public string GetWarehouseTypeText(bool isModalFrom)
    {
        if (isModalFrom)
        {
            return Request.ChoosenFromWarehouseTypes.Count > 0
                ? string.Join(", ", Request.ChoosenFromWarehouseTypes.Select(p => WarehouseTypes.First(q => q.Code == p).Title))
                : string.Empty;
        }
        else
        {
            return Request.ChoosenToWarehouseTypes.Count > 0
                ? string.Join(", ", Request.ChoosenToWarehouseTypes.Select(p => WarehouseTypes.First(q => q.Code == p).Title))
                : string.Empty;
        }
    }

    public async Task OnDocumentStatusModalClick()
    {
        IsModalDocumentStatusOpenedInNonPermittedMode = true;

        foreach (GetAllDocumentStatusVm status in DocumentStatuses)
        {
            status.IsChoosen = Request.ChoosenDocumentChangeStatuses.Any(p => p.Equals(status.Id));
        }

        await ModalDocumentStatus.Open(new());
    }

    public async Task OnDocumentStatusPermittedModalClick()
    {
        IsModalDocumentStatusOpenedInNonPermittedMode = false;

        foreach (GetAllDocumentStatusVm status in DocumentStatuses)
        {
            status.IsChoosen = Request.ChoosenDocumentPermittedStatuses.Any(p => p.Equals(status.Id));
        }

        await ModalDocumentStatus.Open(new());
    }

 
    public string GetDocumentStatusText() =>
        (Request.ChoosenDocumentChangeStatuses.Count > 0)
            ? Request.ChoosenDocumentChangeStatuses.Contains(0)
                ? string.Empty 
                : string.Join(", ", Request.ChoosenDocumentChangeStatuses
                    .Select(p => DocumentStatuses.FirstOrDefault(q => q.Id == p)?.Title)
                    .Where(title => !string.IsNullOrWhiteSpace(title))) 
            : TextResources.APP_StringKeys_NoLimit;


    public string GetPermittedDocumentStatusText() =>
        (Request.ChoosenDocumentPermittedStatuses.Count > 0)
            ? Request.ChoosenDocumentPermittedStatuses.Contains(0)
                ? string.Empty
                : string.Join(", ", Request.ChoosenDocumentPermittedStatuses
                    .Select(p => DocumentStatuses.FirstOrDefault(q => q.Id == p)?.Title)
                    .Where(title => !string.IsNullOrWhiteSpace(title)))
            : TextResources.APP_StringKeys_NoLimit;

    

    public string GetActionControlsText() => Request.ChoosenActionControls.Count > 0
           ? string.Join(", ", Request.ChoosenActionControls.Select(p => ActionControls.First(q => q.Code == p).Name))
           : string.Empty;

    public async Task OnDocumentStatusModalClose()
    {
        if (IsModalDocumentStatusOpenedInNonPermittedMode)
        {
            Request.ChoosenDocumentChangeStatuses.Clear();

            foreach (var type in DocumentStatuses)
            {
                if (type.IsChoosen)
                {
                    Request.ChoosenDocumentChangeStatuses.Add(type.Id);
                }
            }
        }
        else
        {
            Request.ChoosenDocumentPermittedStatuses.Clear();

            foreach (var type in DocumentStatuses)
            {
                if (type.IsChoosen)
                {
                    Request.ChoosenDocumentPermittedStatuses.Add(type.Id);
                }
            }
        }
    }

    public async Task OnActionControlsModalOpen()
    {
        foreach (GetAllActionTypeControlsDto control in ActionControls)
        {
            control.IsChoosen = Request.ChoosenActionControls.Any(p => p.Equals(control.Id));
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

    public async Task OnDocumentStatusChooseChange(GetAllDocumentStatusVm dto)
    {
        if (!IsModalDocumentStatusOpenedInNonPermittedMode)
        {
            return;
        }

        foreach (GetAllDocumentStatusVm item in DocumentStatuses)
        {
            item.IsChoosen = item.Id == dto.Id;
        }
    }

    private async Task ReloadActionTypes()
    {
        IsLoading = true;

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
           , "ActionType/ReadAll")).Value.List;

        IsLoading = false;
    }
}
