using AutoMapper;
using Silo.Domains.Entities;
using Silo.Identity.Client;
using Silo.Shared.Components;

namespace Silo.Modules.Document.Pages;
public partial class AddDynamicField
{
    public bool IsLoading = true;
    public string UserToken;
    public SaveDynamicFieldCommand DynamicField = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<GetAllActionTypesDto> ActionTypes;
    public List<GetAllDynamicFieldSectionsVm> Sections;
    public List<GetAllDynamicFieldVm> ParentFields;
    public List<GetAllDynamicFieldSectionsVm> FilteredSections;
    public List<FieldTypeVm> FieldTypes = new();

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    public EditForm EditForm { get; set; }
    public Modal ModalRemove { get; set; }
    public Modal ModalDynamicFields { get; set; }
    public TelerikDropDownList<string, string> ValueOptionsDropDown { get; set; }

    protected override async Task SiloInitializer()
    {

        ActionTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
               , "ActionType/ReadAll")).Value.List;

        Sections = (await Api.PostAsyncByUri<List<GetAllDynamicFieldSectionsVm>>("wms/Document","GetAllDynamicFieldSections")).Value;

        FieldTypes = new List<FieldTypeVm>
    {
          
    new FieldTypeVm { Code = DynamicFieldType.HeaderData, Title = TextResources.APP_StringKeys_Document_HeaderData },
    new FieldTypeVm { Code = DynamicFieldType.ItemData, Title = TextResources.APP_StringKeys_Document_ItemData },
    new FieldTypeVm { Code = DynamicFieldType.ProductField, Title = TextResources.APP_StringKeys_Product_Field },
    new FieldTypeVm { Code = DynamicFieldType.OperationInfo, Title = TextResources.APP_StringKeys_OperationInfo },
    new FieldTypeVm { Code = DynamicFieldType.TruckCrossPresent, Title = TextResources.APP_StringKeys_TruckCrossPresent },
    new FieldTypeVm { Code = DynamicFieldType.TruckCrossEnter, Title = TextResources.APP_StringKeys_TruckCrossEnter },
    new FieldTypeVm { Code = DynamicFieldType.TruckCrossExit, Title = TextResources.APP_StringKeys_TruckCrossExit }

        };

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetAllDynamicFields")).Value;

        RebuildParentFields();

        IsLoading = false;
    }

    #region Events
    public async Task OnValidSubmit(EditContext editContext)
    {
        IsLoading = true;

        if(DynamicField.ValueType == DynamicFieldValueType.DropDown &&
           DynamicField.ValueOptionList.Count == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose_One_ValueOption, "error");

            IsLoading = false;

            return;
        }

        if (DynamicField.Id == 0)
        {
            DynamicField.DateTime = DateTime.Now;
        }

        FixRelatedTitleForSave();

        int result = (await Api.PostAsyncByUri<int>("wms/Document", "SSaveDynamicField",
            new KeyValuePair<string, object>("dynamicField", DynamicField))).Value;

        FixRelatedTitleForShow();

        if (DynamicField.Id == 0)
        {
            DynamicField.Id = result;
        }

        Notification.Show(result > 0 ? TextResources.APP_StringKeys_Message_Success : TextResources.APP_StringKeys_Alert_Fail
            , result > 0 ? "success" : "warning");

        IsLoading = false;
    }

    public void OnClearClick(MouseEventArgs e)
    {
        DynamicField = new();

        DynamicFields = new();
    }
    #endregion

    #region Dynamic Field Modal
    public async Task OnSelectDynamicField(GetAllDynamicFieldVm dynamicField)
    {
        DynamicField = Mapper.Map<SaveDynamicFieldCommand>(dynamicField);


        RebuildParentFields();

        FixRelatedTitleForShow();

        await InvokeAsync(StateHasChanged);
        await ModalDynamicFields.Close(new());
    }

    private void RebuildParentFields()
    {
        if (!DynamicField.FieldType.HasValue)
        {
            ParentFields = new();
            DynamicField.ParentId = null;
            return;
        }

        ParentFields = DynamicFields
            .Where(f =>
                f.Id != DynamicField.Id &&
                f.FieldType == DynamicField.FieldType.Value
            )
            .ToList();

        if (DynamicField.ParentId.HasValue &&
            !ParentFields.Any(p => p.Id == DynamicField.ParentId))
        {
            DynamicField.ParentId = null;
        }
    }

    private void OnFieldTypeChanged()
    {
        RebuildParentFields();
    }


    public async Task OnModalDynamicFieldsClick(MouseEventArgs e)
    {
        DynamicFields = new();

        IsLoading = true;

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetAllDynamicFields")).Value;
        foreach (var field in DynamicFields)
        {
            var typeVm = FieldTypes.FirstOrDefault(f => f.Code == field.FieldType);
            field.FieldTypeTitle = typeVm != null ? typeVm.Title : string.Empty;
        }

        IsLoading = false;

        await ModalDynamicFields.Open(e);
    }
    #endregion

    #region Option
    public async Task OnRemoveOptionClick(string option)
    {
        DynamicField.ValueOptionList.Remove(option);

        ValueOptionsDropDown.Rebind();
    }

    public async Task OnAddOptionClick(MouseEventArgs e)
    {
        if (DynamicField.ValueOption.HasValue())
        {
            DynamicField.ValueOptionList.Add(DynamicField.ValueOption);

            DynamicField.ValueOption = string.Empty;

            ValueOptionsDropDown.Rebind();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }
    }
    #endregion

    #region Remove Modal
    public async Task OnRemoveModalClick(MouseEventArgs e)
    {
        if (DynamicField.Id == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "warning");
        }
        else
        {
            await ModalRemove.Open(e);
        }
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("wms/Document", "SDeleteDynamicField",
                          new KeyValuePair<string, object>("dynamicFieldId", DynamicField.Id))).Value;

        IsLoading = false;

        Notification.Show(result ? TextResources.APP_StringKeys_Message_Success : TextResources.APP_StringKeys_Alert_Fail
                        , result ? "success" : "warning");

        if (result)
        {
            OnClearClick(new());
        }
    }
    #endregion

    #region Private
    private void FixRelatedTitleForSave()
    {
        DynamicField.RelatedTitle1 = DynamicField.RelatedTitle1.HasNoValue() ? "fake" : DynamicField.RelatedTitle1;
        DynamicField.RelatedTitle2 = DynamicField.RelatedTitle2.HasNoValue() ? "fake" : DynamicField.RelatedTitle2;
        DynamicField.RelatedTitle3 = DynamicField.RelatedTitle3.HasNoValue() ? "fake" : DynamicField.RelatedTitle3;
    }

    private void FixRelatedTitleForShow()
    {
        DynamicField.RelatedTitle1 = DynamicField.RelatedTitle1.Equals("fake") ? string.Empty : DynamicField.RelatedTitle1;
        DynamicField.RelatedTitle2 = DynamicField.RelatedTitle2.Equals("fake") ? string.Empty : DynamicField.RelatedTitle2;
        DynamicField.RelatedTitle3 = DynamicField.RelatedTitle3.Equals("fake") ? string.Empty : DynamicField.RelatedTitle3;
    }
    #endregion
}
