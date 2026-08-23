using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Silo.Application.Features;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Document.Pages;

public partial class DocumentRemove
{
    public bool IsLoading = true;
    public string CartableTitle;
    public string DetailModalHeaderData;
    public bool IsAllSelected;
    public GetAllDocumentByStatusQuery Request = new();
    public List<GetAllDocumentByStatusVm> Documents = new();
    public List<GetAllDocumentItemByStatusVm> DocumentDetails = new();
    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public List<GetAllDocumentStatusVm> DocumentStatuses = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<string> DynamicFieldColumns = new();
    public string RemoveDocumentDescription = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ApproveModal { get; set; }
    public Modal RevokeModal { get; set; }
    public Modal DetailsModal { get; set; }
    public Modal ModalPrint { get; set; }

    public TelerikGrid<GetAllDocumentByStatusVm> GridDocuments { get; set; }

    protected override async Task SiloInitializer()
    {
        IsLoading = true;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
                            , new GetAllDocumentStatusVmContext())).Value;

        CartableTitle = TextResources.APP_StringKeys_Document_Remove_Cartable;

        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType"
        , new GetAllDocumentTypesVmContext())).Value;

        if (DocumentTypes.Count == 1)
        {
            Request.DocumentType = DocumentTypes.First()?.Code;
        }

        IsLoading = false;

        StateHasChanged();

        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir").GetAwaiter();
    }

    #region Search
    public async Task OnSearchValidSubmit()
    {
        IsLoading = true;

        if (Request.DocumentType.HasValue())
        {
            DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", Request.DocumentType))).Value;

            if (DynamicFields.Any())
            {
                DynamicFieldColumns = DynamicFields.Where(p => p.FieldShowColumn).Select(p => p.Title).ToList();
            }
        }

        GetAllDocumentByStatusQuery request = FixEmptiness();

        Documents = (await Api.PostAsyncByContext<List<GetAllDocumentByStatusVm>>("SGetAllMainDocuments"
                , new GetAllDocumentByStatusVmContext()
                , new KeyValuePair<string, object>("request", request)
                )).Value;

        IsLoading = false;
    }

    public async Task OnDocumentDetailsClick(string documentKey, string documentType, int documentStatus)
    {
        IsLoading = true;

        GetAllDocumentByStatusQuery request = FixEmptiness();

        request.DocumentKey = documentKey;

        request.DocumentType = documentType;

        request.Status = documentStatus;

        DocumentDetails = (await Api.PostAsyncByContext<List<GetAllDocumentItemByStatusVm>>("SGetAllDocumentItemByStatusVm"
        , new GetAllDocumentItemByStatusVmContext()
        , new KeyValuePair<string, object>("status", request)
        )).Value;

        DetailModalHeaderData = Documents.FirstOrDefault(p => p.DocumentKey.Equals(request.DocumentKey)).HeaderData;

        IsLoading = false;

        await DetailsModal.Open(new());
    }
    #endregion

    #region Remove
    public async Task OnRemoveButtonClick(MouseEventArgs e)
    {
        if (Documents.Exists(p => p.IsChoosed))
        {
            await ApproveModal.Open(e);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose_One_Document, "error");
        }
    }

    public async Task OnApproveClick()
    {
        IsLoading = true;

        RemoveDocumentCommand command = new()
        {
            DocumentKeyTypes = new(),
            Description = RemoveDocumentDescription
        };

        Documents.Where(p => p.IsChoosed).ToList().ForEach(p =>
        {
            command.DocumentKeyTypes.Add(new() { Key = p.DocumentKey, Type = p.DocumentType });
        });

        var result = (await Api.PostAsync<int>("SRemoveDocuments",
            new KeyValuePair<string, object>("command", command))).Value;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            foreach (var keyTypes in command.DocumentKeyTypes)
            {
                Documents.RemoveAll(p => p.DocumentKey.Equals(keyTypes.Key)
                                          && p.DocumentType.Equals(keyTypes.Type));
            }

            GridDocuments.Rebind();

            Documents.ForEach(p => p.IsChoosed = false);

            RemoveDocumentDescription = string.Empty;
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }


        IsLoading = false;
    }
    #endregion

    #region Events
    public void OnClickProductCode(string code)
    {
        Request.ProductCode = code;
    }

    public void OnToggleSelectAll()
    {
        Documents.ForEach(p => p.IsChoosed = IsAllSelected);
    }

    public void OnClearClick(MouseEventArgs e)
    {
        Request = new();

        Documents = new();

        IsAllSelected = false;

        if (DocumentTypes.Count == 1)
        {
            Request.DocumentType = DocumentTypes.First()?.Code;
        }
    }
    #endregion

    #region Private Mehtods
    private GetAllDocumentByStatusQuery FixEmptiness()
    {
        GetAllDocumentByStatusQuery search = new();

        if (Request.FromDate.HasValue())
        {
            search.FromDate = Request.FromDate;
        }

        if (Request.ToDate.HasValue())
        {
            search.ToDate = Request.ToDate;
        }

        if (Request.DocumentType.HasValue())
        {
            search.DocumentType = Request.DocumentType;
        }

        if (Request.ProductCode.HasValue())
        {
            search.ProductCode = Request.ProductCode;
        }

        if (Request.DocumentKey.HasValue())
        {
            search.DocumentKey = Request.DocumentKey;
        }

        search.Status = Request.Status;

        search.GetCurrentStatusOnly = Request.GetCurrentStatusOnly;

        return search;
    }
    #endregion
}
