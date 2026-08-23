using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;
using Silo.Components.DynamicField;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Document.Pages;
public partial class DocumentEdit
{
    public bool IsLoading = true;
    public List<GetAllActionTypesDto> DocumentTypes = new();
    public GetDocumentByKeyQuery DocumentRequest = new();
    public GetDocumentByKeyVm LoadedDocument = new();
    public GetAllDynamicFieldVm DocumentHeaderKey = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<GetAllDynamicFieldVm> ItemDynamicFields = new();
    public List<DynamicFieldWithValueDto> HeaderDynamicFields = new();
    public DynamicFieldFillValue HeaderDynamicFieldsRef { get; set; }
    public List<TelerikDropDownItemGeneric<int>> DocumentCheckTypes = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_Exact,
            Value = (int)DocumentCheckType.Exact
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_ProductCodeRemain,
            Value = (int)DocumentCheckType.ProductCodeAndDocCodeRemain
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_DocumentRemain,
            Value = (int)DocumentCheckType.DocCodeRemain
        }
    };

    public ProductCodeModal ProductCodeModal { get; set; }
    public TelerikGrid<GetSingleDocumentItemsVm> GridDocumentItems { get; set; }
    public Modal ApproveModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task SiloInitializer()
    {
        DocumentTypes = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
       , "ActionType/ReadAll")).Value.List;


        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir")
                 .GetAwaiter();

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        bool isAllowedUpdate = (await Api.PostAsync<bool>("SIsDocumentUpdateAllowed"
            , new ("documentKey", DocumentRequest.DocumentKey)
            , new ("documentType", DocumentRequest.DocumentType))).Value;

        if (!isAllowedUpdate)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ActionType_AllowUpdate, "error");

            IsLoading = false;

            return;
        }

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetDynamicFieldsByActionTypeId",
            new KeyValuePair<string, object>("actionTypeId", DocumentRequest.DocumentType))).Value;

        if (DynamicFields.Neither())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_ActionType_Any, "error");

            IsLoading = false;

            return;
        }

        LoadedDocument = new()
        {
            DocumentItems = new()
        };

        HeaderDynamicFields = DynamicFields.Where(p => p.FieldType == DynamicFieldType.HeaderData && !p.IsHeaderKey)
                                           .DistinctBy(p => p.Title)
                                           .Select(p => new DynamicFieldWithValueDto()
                                           {
                                               Title = p.Title,
                                               Value = p.DefaultValue,
                                               DefaultValue = p.DefaultValue,
                                               ValueType = p.ValueType,
                                               ValueOptions = p.ValueOptionList,
                                               IsRequired = p.IsRequired ?? false,
                                               IsReadOnly = p.IsReadOnly ?? false
                                           })
                                           .ToList();

        ItemDynamicFields = DynamicFields.Where(p => p.FieldType == DynamicFieldType.ItemData && p.IsSystematicField)
                                         .DistinctBy(p => p.Title).ToList();

        DocumentHeaderKey = DynamicFields.FirstOrDefault(p => p.FieldType == DynamicFieldType.HeaderData && p.IsHeaderKey);

        var document = (await Api.PostAsyncByUriAndContext<GetDocumentByKeyVm>("wms/Document", "SGetDocumentHeaderAndItems"
            , new GetDocumentByKeyVmContext()
            , new KeyValuePair<string, object>("documentKey", DocumentRequest.DocumentKey)
            , new KeyValuePair<string, object>("documentType", DocumentRequest.DocumentType))).Value;

        CheckDocumentEditAvailability(document);

        IsLoading = false;
    }

    public async Task OnSaveDocumentClick(MouseEventArgs e)
    {
        if (DocumentHeaderKey is null)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            return;
        }

        IsLoading = true;

        SaveDocumentCommand command = new()
        {
            DocumentJsonData = new(),
            DocumentKey = LoadedDocument.Key,
            DocumentType = LoadedDocument.DocumentType,
            DocumentType1 = LoadedDocument.DocumentType1,
            DocumentType2 = LoadedDocument.DocumentType2,
            DocumentCheckType = DocumentRequest.DocumentCheckType
        };

        //Add document json data
        foreach (var item in LoadedDocument.DocumentItems)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            //Add document headers
            foreach (var field in HeaderDynamicFields)
            {
                var relatedTitle = DynamicFields.First(p => p.Title.Equals(field.Title)).RelatedTitle1;

                if (!((IDictionary<String, Object>)exo).Any(p => p.Key.Equals(relatedTitle)))
                {
                    ((IDictionary<String, Object>)exo).Add(relatedTitle, field.Value ?? string.Empty);
                }
            }

            //Add document key
            ((IDictionary<String, Object>)exo).Add(DocumentHeaderKey.RelatedTitle1, command.DocumentKey);

            ((IDictionary<String, Object>)exo).Add("ProductCode", item.ProductCode);
            ((IDictionary<String, Object>)exo).Add("Count", item.Count);
            ((IDictionary<String, Object>)exo).Add("ProductTitle", item.ProductTitle);
            ((IDictionary<String, Object>)exo).Add("ProductUnit", item.ProductUnit);

            //Add document items
            command.DocumentJsonData.Add(Newtonsoft.Json.JsonConvert.SerializeObject(exo));
        }

        bool result = (await Api.PostAsync<bool>("SSaveDynamicManual"
            , new KeyValuePair<string, object>("command", command))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        DocumentRequest = new();

        LoadedDocument = new();

        DynamicFields = new();

        HeaderDynamicFields = new();

        ItemDynamicFields = new();
    }

    public async Task OnClickProduct(PositionProductResponse product)
    {
        var item = LoadedDocument.DocumentItems.FirstOrDefault(p => p.ProductCode == product.ProductCode);

        if (item is not null)
        {
            item.ProductCode = product.ProductCode;

            item.ProductTitle = product.ProductName;

            item.ProductUnit = product.ProductUnit;

            item.EditCount = item.Count;

            item.IsEditing = true;
        }
        else
        {
            LoadedDocument.DocumentItems.Add(new()
            {
                ProductCode = product.ProductCode,
                ProductTitle = product.ProductName,
                ProductUnit = product.ProductUnit,
                IsEditing = true
            });
        }

        GridDocumentItems.Rebind();
    }

    public async Task OnSaveEditItemClick(GetSingleDocumentItemsVm item)
    {
        item.IsEditing = false;

        item.Count = item.EditCount;
    }

    public async Task OnEditItemClick(GetSingleDocumentItemsVm item)
    {
        item.IsEditing = true;

        item.EditCount = item.Count;
    }

    public async Task OnCancelEditItemClick(GetSingleDocumentItemsVm item)
    {
        item.IsEditing = false;

        item.EditCount = 0;
    }

    public async Task OnApproveDeleteRow(GetSingleDocumentItemsVm item)
    {
        item.IsEditing = false;

        LoadedDocument.DocumentItems.Remove(item);

        GridDocumentItems.Rebind();
    }

    private void CheckDocumentEditAvailability(GetDocumentByKeyVm document)
    {
        if (document is not null)
        {
            if (document.AggStatus == 1 && document.Parent == "0")
            {
                JToken data = JToken.Parse(document?.HeaderData);

                foreach (var field in HeaderDynamicFields)
                {
                    if (field.Title is not null)
                    {
                        if (data[field.Title.Trim()] is not null)
                        {
                            field.Value = data[field.Title.Trim()].ToString();
                        }
                        else
                        {
                            field.Value = "";
                        }
                    }
                }

                LoadedDocument = document;

                DocumentRequest.DocumentCheckType = LoadedDocument.DocumentCheckType;
            }
            else if (document.AggStatus == 1 && document.Parent != "0")
            {
                if (document.Parent.ToLower() == "divided")
                {
                    Notification.Show(TextResources.APP_StringKeys_DocumentEdit_Reset_First, "error");

                    Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_DocumentEdit,
                        TextResources.APP_StringKeys_Divided_Document), "error");

                    return;
                }
                else
                {
                    Notification.Show(TextResources.APP_StringKeys_DocumentEdit_Reset_First, "error");

                    Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_DocumentEdit,
                        TextResources.APP_StringKeys_Aggregated_Document), "error");

                    return;
                }
            }
        }
        else
        {
            LoadedDocument.Key = DocumentRequest.DocumentKey;

            LoadedDocument.DocumentType = DocumentRequest.DocumentType.ToString();

            LoadedDocument.DocumentCheckType = DocumentRequest.DocumentCheckType;
        }
    }
}
