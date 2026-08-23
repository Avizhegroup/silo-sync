using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;

namespace Silo.Modules.Document.Pages;
public partial class DocumentCartable
{
    private int previousCartable;
    public bool IsLoading = true;
    public string CartableTitle;
    public string DetailModalHeaderData;
    public GetAllDocumentByStatusVm RevokeDocument;
    public bool IsAllSelected;
    public GetAllDocumentByStatusQuery Request = new();
    public List<GetAllDocumentByStatusVm> Documents = new();
    public List<GetAllDocumentItemByStatusVm> DocumentDetails = new();
    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public List<GetAllDocumentStatusVm> DocumentStatuses = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<string> DynamicFieldColumns = new();
    public SaveDocumentStatusCommand ChangeStatusCommand = new();
    public string CompanyName;
    public string CurrentPrintingDocumentKey = string.Empty;
    public DocumentHeaderDto PrintableDoc = new();

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter] public int CartableMode { get; set; }

    public ProductCodeModal ProductCodeModal { get; set; }
    public Modal ApproveModal { get; set; }
    public Modal RevokeModal { get; set; }
    public Modal DetailsModal { get; set; }
    public TelerikGrid<GetAllDocumentByStatusVm> GridDocuments { get; set; }

    protected override async Task SiloInitializer()
    {
        await InitCartable();
    }

    protected override async Task OnParametersSetAsync()
    {
        if ((CartableMode) != previousCartable)
        {
            previousCartable = (CartableMode);

            OnClearClick(new());

            await InitCartable();
        }

        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir").GetAwaiter();

        await base.OnParametersSetAsync();
    }

    #region Search
    public async Task OnValidSubmit(EditContext e)
    {
        await Search();
    }
    #endregion

    #region Save and Revoke
    public async Task OnSaveButtonClick(MouseEventArgs e)
    {
        if (Documents.Exists(p => p.IsChoosed))
        {
            if (Documents.Where(p => p.IsChoosed).ToList().Exists(p => p.Status != Request.Status))
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Cartable_Submited_Document, "error");
            }
            else
            {
                ChangeStatusCommand = new();

                await ApproveModal.Open(e);
            }
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose_One_Document, "error");
        }
    }

    public async Task OnApproveClick()
    {
        IsLoading = true;

        Documents.Where(p => p.IsChoosed).ToList().ForEach(p =>
        {
            ChangeStatusCommand.DocumentKeyTypes.Add(new() { Key = p.DocumentKey, Type = p.DocumentType });
        });

        var newStatus = (await Api.PostAsync<int>("SChangeDocumentStatus",
            new KeyValuePair<string, object>("command", ChangeStatusCommand))).Value;

        if (newStatus > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Documents = Documents.Where(p => !ChangeStatusCommand.DocumentKeyTypes.Any(q => q.Equals(p.DocumentKey))).ToList();

            foreach (var keyTypes in ChangeStatusCommand.DocumentKeyTypes)
            {
                Documents.FirstOrDefault(p => p.DocumentKey.Equals(keyTypes.Key)
                                          && p.DocumentType.Equals(keyTypes.Type)
                                          && p.Status == Request.Status).Status = newStatus;
            }

            Documents.ForEach(p => p.IsChoosed = false);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }

        ChangeStatusCommand = new();

        IsLoading = false;
    }

    public async Task OnRevokeApproveModalOpenClick(GetAllDocumentByStatusVm doc)
    {
        RevokeDocument = doc;

        ChangeStatusCommand = new();

        await RevokeModal.Open(new());
    }

    public async Task OnRevokeClick()
    {
        IsLoading = true;

        ChangeStatusCommand.DocumentKeyTypes.Add(new()
        {
            Key = RevokeDocument.DocumentKey,
            Type = RevokeDocument.DocumentType
        });

        var result = (await Api.PostAsync<int>("SRevokeDocumentStatus",
                new KeyValuePair<string, object>("revoke", ChangeStatusCommand))).Value;

        if (result > -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            Documents.FirstOrDefault(p => p.DocumentKey.Equals(RevokeDocument.DocumentKey)
                                          && p.DocumentType.Equals(RevokeDocument.DocumentType)
                                          && p.Status == RevokeDocument.Status).Status = Request.Status;
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }

        IsLoading = false;
    }
    #endregion

    #region Events
    public void OnToggleSelectAll()
    {
        Documents.ForEach(p => p.IsChoosed = IsAllSelected);
    }

    public void OnToggleSelectSingle(object value)
    {
        if (Documents.Any(p => p.IsChoosed == false))
        {
            IsAllSelected = false;
        }
        else
        {
            IsAllSelected = true;
        }
    }

    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        GetAllDocumentByStatusVm item = (GetAllDocumentByStatusVm)args.Item;

        if (item.Status == Request.Status)
        {
            args.Class = "";
        }
        else if (CartableMode != 0)
        {
            args.Class += " bg-success";
        }
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

    public void OnClearClick(MouseEventArgs e)
    {
        Request = new();

        Request.Status = (CartableMode);

        Documents = new();

        IsAllSelected = false;

        if (DocumentTypes.Count == 1)
        {
            Request.DocumentType = DocumentTypes.FirstOrDefault()?.Code;
        }
    }

    public async Task OnPrintDocumentClick(string documentKey, string documentType)
    {
        IsLoading = true;

        PrintableDoc = (await Api.PostAsyncByUri<DocumentHeaderDto>("wms/Document","SGetDocumentHeaderAndItems",
             new KeyValuePair<string, object>("documentKey", documentKey),
             new KeyValuePair<string, object>("documentType", documentType))).Value;

        IsLoading = false;

        CurrentPrintingDocumentKey = documentKey;

        await Print();

        IsLoading = false;
    }

    #endregion

    #region Private Mehtods
    private async Task Search()
    {
        IsLoading = true;

        if (Request.DocumentType.HasValue())
        {
            DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document","SGetDynamicFieldsByActionTypeId",
                new KeyValuePair<string, object>("actionTypeId", Request.DocumentType))).Value;

            if (DynamicFields.Any())
            {
                DynamicFieldColumns = DynamicFields.Where(p => p.FieldShowColumn).Select(p => p.Title).ToList();
            }
        }

        GetAllDocumentByStatusQuery request = FixEmptiness();

        Documents = (await Api.PostAsyncByContext<List<GetAllDocumentByStatusVm>>("SGetAllDocumentByCurrentAndNextStatus"
                , new GetAllDocumentByStatusVmContext()
                , new KeyValuePair<string, object>("status", request)
                )).Value;

        IsLoading = false;
    }

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

        if (Request.DocumentHeaderText.HasValue())
        {
            search.DocumentHeaderText = Request.DocumentHeaderText;
        }

        search.Status = Request.Status;

        search.GetCurrentStatusOnly = Request.GetCurrentStatusOnly;

        return search;
    }

    private async Task InitCartable()
    {
        IsLoading = true;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
                            , new GetAllDocumentStatusVmContext())).Value;

        if (DocumentStatuses.Count(p => p.Id == ((int)CartableMode) && p.IsCartablePermitted) == 0)
        {
            NavigationManager.NavigateTo($"/documentcartable/0");
        }

        CartableTitle = DocumentStatuses.FirstOrDefault(p => p.IsCartablePermitted && p.Id == ((int)CartableMode))?.Title ??
                        TextResources.APP_StringKeys_Report_Docs;

        Request.Status = (CartableMode);

        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType"
        , new GetAllDocumentTypesVmContext())).Value;

        if (DocumentTypes.Count == 1)
        {
            Request.DocumentType = DocumentTypes.Any() ? DocumentTypes.First().Code : string.Empty;
        }

        if (CartableMode != 0)
        {
            await Search();
        }

        IsLoading = false;

        StateHasChanged();
    }

    private async Task Print()
    {
        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("DocumentKey", $"{PrintableDoc.Key}")
            , new("DateTimeString", $"تاریخ و ساعت درج سند: {PersianCalendarTools.GregorianToPersian(PrintableDoc.ImportDateTime)}-{PrintableDoc.ImportDateTime.Value.ToShortTimeString()}")
            , new("DocumentType", $"نوع سند: {(DocumentTypes.FirstOrDefault(p=>p.Code.ToString().Equals(PrintableDoc.DocumentType))?.Title ?? string.Empty)}")
            , new("Description", $"توضیحات: {PrintableDoc.Description}")
            , new("Status", $"وضعیت: {DocumentStatuses.FirstOrDefault(p=>p.Id==PrintableDoc.DocumentStatusId)?.Title}")
            , new("CompanyName", CompanyName)
        };

        var headerDatas = JToken.Parse(PrintableDoc.HeaderData);

        if (headerDatas is not null)
        {
            foreach (JProperty prop in headerDatas)
            {
                variables.Add(new(prop.Name.ToString().Trim().Replace(' ', '_'), prop.Value.ToString()));
            }
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(DocumentItemPrintDto), Mapper.Map<List<DocumentItemPrintDto>>(PrintableDoc.DocumentItems))
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "Aggregate",
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
         , "PreparedReport/Create"
         , command);

        if (response.Value.Result < 1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");

            return;
        }

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);
    }
    #endregion
}
