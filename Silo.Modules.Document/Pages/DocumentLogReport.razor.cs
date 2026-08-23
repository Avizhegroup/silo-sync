using Microsoft.JSInterop;
using Silo.Application.Dto;
using Silo.Application.Features;
using Silo.Shared.Components;

namespace Silo.Modules.Document.Pages;
public partial class DocumentLogReport
{
    public bool IsLoading = true;
    public GetAllDocumentLogQuery Request = new();
    public List<GetAllDocumentLogVm> DocumentLogs = new();
    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public List<GetAllDocumentLogUserVm> DocumentLogUsers = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<string> DynamicFieldColumns = new();
    public List<TelerikDropDownItemGeneric<string>> DocumentEventTypes;
    public List<GetAllDocumentItemByStatusVm> DocumentDetails = new();
    public string DetailModalHeaderData;
    public bool IsMinutesUntilNextActive = false;
    public List<GetAllDocumentStatusVm> DocumentStatuses = new();

    public TelerikGrid<GetAllDocumentLogVm> GridDocumentLog { get; set; }

    public Modal DetailsModal { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter] public string DocumentKey { get; set; }
    [Parameter] public string DocumentType { get; set; }

    protected override async Task SiloInitializer()
    {
        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType"
        , new GetAllDocumentTypesVmContext())).Value;

        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir").GetAwaiter();

        DocumentLogUsers = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentLogUserVm>>("wms/DocumentLog", "SGetAllDocumentLogUser"
                , new GetAllDocumentLogUserVmContext())).Value;

        DocumentEventTypes = new()
        {
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_InsertDocument,
                Value = DocumentEventType.InsertDocument.ToString()
            },
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_InsertAggregate,
                Value = DocumentEventType.InsertAggregate.ToString()
            },
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_RemoveAggregate,
                Value = DocumentEventType.RemoveAggregate.ToString()
            },
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_InsertDivide,
                Value = DocumentEventType.InsertDivide.ToString()
            },
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_RemoveDivide,
                Value = DocumentEventType.RemoveDivide.ToString()
            },
            new()
            {
                Name = TextResources.APP_StringKeys_DocumentLog_RemoveDocument,
                Value = DocumentEventType.RemoveDocument.ToString()
            }
        };

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
        , new GetAllDocumentStatusVmContext())).Value;

        foreach (var status in DocumentStatuses)
        {
            DocumentEventTypes.Add(new()
            {
                Name = TextResources.APP_StringKeys_Change_To_Status + " " + status.Title,
                Value = "*" + status.Id.ToString()
            });
        }

        if (DocumentKey.HasValue())
        {
            Request.DocumentType = DocumentType;

            Request.DocumentKey = DocumentKey;

            await OnSearchValidSubmit();
        }

        IsLoading = false;
    }

    public async Task OnSearchValidSubmit()
    {
        IsLoading = true;

        DynamicFieldColumns = new();


        if (Request.DocumentType.HasValue())
        {
            DocumentLogs = new();

            DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document", "SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", Request.DocumentType))).Value;

            if (DynamicFields.Any())
            {
                DynamicFieldColumns = DynamicFields.Where(p => p.FieldShowColumn).Select(p => p.Title).ToList();
            }
        }

        GetAllDocumentLogQuery request = FixEmptiness();

        IsMinutesUntilNextActive = request.DocumentKey.HasValue();

        DocumentLogs = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentLogVm>>("wms/DocumentLog", "SGetAllDocumentLogs"
                , new GetAllDocumentLogVmContext()
                , new KeyValuePair<string, object>[] { new("request", request) })).Value;

        IsFiltersShown = false;

        IsLoading = false;
    }

    public async Task OnDocumentDetailsClick(string documentKey, string documentType)
    {
        IsLoading = true;

        GetAllDocumentByStatusQuery request = new()
        {
            DocumentKey = documentKey,
            DocumentType = documentType,
            Status = -1
        };

        DocumentDetails = (await Api.PostAsyncByContext<List<GetAllDocumentItemByStatusVm>>("SGetAllDocumentItemByStatusVm"
        , new GetAllDocumentItemByStatusVmContext()
        , new KeyValuePair<string, object>("status", request)
        )).Value;

        DetailModalHeaderData = DocumentLogs.FirstOrDefault(p => p.DocumentKey.Equals(request.DocumentKey) 
                                                              && p.DocumentType.Equals(request.DocumentType)).HeaderData;

        IsLoading = false;

        await DetailsModal.Open(new());
    }


    public void OnClearClick(MouseEventArgs e)
    {
        Request = new();

        DocumentLogs = new();
    }

    private GetAllDocumentLogQuery FixEmptiness()
    {
        GetAllDocumentLogQuery search = new();
        
        if (Request.DocumentKey.HasValue())
        {
            search.DocumentKey = Request.DocumentKey;
        }

        if (Request.DocumentType.HasValue())
        {
            search.DocumentType = Request.DocumentType;
        }
        
        if (Request.FromDate.HasValue())
        {
            search.FromDate = Request.FromDate;
        }

        if (Request.ToDate.HasValue())
        {
            search.ToDate = Request.ToDate;
        }

        if (Request.HeaderData.HasValue())
        {
            search.HeaderData = Request.HeaderData;
        }

        if (Request.User.HasValue())
        {
            search.User = Request.User;
        }

        if (Request.Description.HasValue())
        {
            search.Description = Request.Description;
        }

        search.DocumentEventType = Request.DocumentEventType;

        return search;
    }
}
