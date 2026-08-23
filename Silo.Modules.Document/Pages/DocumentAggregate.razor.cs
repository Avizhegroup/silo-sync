using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Shared.Components;

namespace Silo.Modules.Document.Pages;
public partial class DocumentAggregate
{
    public bool IsLoading = true;
    public CrudDocKeyDto CrudDocKey = new();
    public bool GetSuggest = false;
    public string SearchDocText = string.Empty;

    public string CompanyName;
    public string AddDocError;
    public List<string> DocumentGroupFields = new();

    public CurrentAggregateDto CurrentAggregate = new();
    public DocumentHeaderDto PrintableDoc = new();
    public DocumentHeaderDto RevokeDocument = new();

    public List<GetAllAggDocVm> AggregatableDocs = new();
    public List<GetAllAggDocVm> AggregatableDocsSearch = new();
    public List<GetAllDocAggSuggestVm> AggregateSuggests = new();
    public List<GetAllDocAggSuggestDetailVm> AggregateSuggestDetails = new();
    public List<GetAllDocAggSuggestDetailVm> AggregateDetails = new();
    public List<GetAggregatedDocDetailsVm> AggregatedDetails = new();
    public List<DocumentHeaderDto> AggregatedDocuments = new();

    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public List<GetAllDocumentStatusVm> DocumentStatuses = new();

    public string RevokeAggregateDescription { get; set; }

    public TelerikGrid<GetAllDocAggSuggestDetailVm> GridSuggestDetails { get; set; }
    public TelerikGrid<GetAllDocAggSuggestVm> GridAggregateSuggests { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Parameter] public string RedirectedDocumentCode { get; set; }
    [Parameter] public string RedirectedDocumentType { get; set; }

    public Modal ModalSuggestDetails { get; set; }
    public Modal ModalAggregateDetails { get; set; }
    public Modal ModalAggregatedDetails { get; set; }
    public Modal ModalAggDoc { get; set; }
    public Modal ModalApprove { get; set; }
    public Modal ModalRemove { get; set; }
    public Modal ModalPrint { get; set; }
    public Modal RevokeModal { get; set; }

    public TelerikGrid<DocumentHeaderDto> GridAggregatedDocs { get; set; }

    protected override async Task SiloInitializer()
    {
        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType"
                , new GetAllDocumentTypesVmContext())).Value;

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
                , new GetAllDocumentStatusVmContext())).Value;

        if (DocumentTypes.Count == 1)
        {
            CurrentAggregate.CurrentAggType = DocumentTypes.FirstOrDefault().Code;
        }

        IsLoading = false;
    }

    #region Events
    public async Task OnClearCkick(MouseEventArgs e)
    {
        AggregateSuggests.Clear();

        GridAggregateSuggests.Rebind();

        CurrentAggregate = new();

        DocumentGroupFields = new();

        if (DocumentTypes.Count == 1)
        {
            CurrentAggregate.CurrentAggType = DocumentTypes.FirstOrDefault().Code;
        }

        AggregatedDocuments = new();

        GetSuggest = false;
    }
    #endregion

    #region Approve Aggregate
    public async Task OnApproveModalClick(string docAggCode)
    {
        if (docAggCode.Split('+').ToList().Count > 1)
        {
            CurrentAggregate.CurrentAggCode = docAggCode;
            
            CurrentAggregate.Description = string.Empty;

            await ModalApprove.Open(new());
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Aggregate_SingleDoc, "error");
        }
    }

    public async Task OnApproveAggregateClick()
    {
        var result = (await Api.PostAsyncByUri<DocumentHeaderDto>("wms/Document"
            , "SAggregateDocuments"
            , new KeyValuePair<string, object>("docAggCode", CurrentAggregate.CurrentAggCode.Replace('+', '-'))
            , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType)
            , new KeyValuePair<string, object>("documentStatus", CurrentAggregate.CurrentAggStatus)
            , new KeyValuePair<string, object>("description", CurrentAggregate.Description)
            )).Value;

        if (result is not null)
        {
            PrintableDoc = result;

            AggregatedDocuments.Add(result);

            if (GridAggregatedDocs is not null)
            {
                GridAggregatedDocs.Rebind();
            }

            RemoveAggSuggest(true);

            CurrentAggregate.Description = string.Empty;

            await ModalPrint.Open(new());
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }
    }

    public async Task OnApprovePrintClick()
    {
        IsLoading = true;

        await ModalPrint.Close(new());

        await Print();

        IsLoading = false;
    }
    #endregion

    #region Get Aggregate Suggest
    public async Task OnGetSuggestButtonClick()
    {
        if (GetSuggest)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_AggSuggest_Recieved, "error");

            return;
        }

        if (CurrentAggregate.CurrentAggType.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Document_Type, "error");

            return;
        }

        if (CurrentAggregate.CurrentAggStatus == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_DocumentStatus, "error");

            return;
        }

        IsLoading = true;

        DocumentGroupFields = (await Api.PostAsyncByUri<List<string>>("wms/Document", "SGetDocumentGroupFields"
            , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType))).Value;

        AggregateSuggests = (await Api.PostAsyncByUriAndContext<List<GetAllDocAggSuggestVm>>("wms/Document", "SGetDocAggSuggestsByDocumentTypeAndStatus"
            , new GetAllDocAggSuggestVmContext()
            , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType)
            , new KeyValuePair<string, object>("documentStatus", CurrentAggregate.CurrentAggStatus)
            )).Value;

        if (AggregateSuggests.Any())
        {
            List<string> AggSuggestKeys = AggregateSuggests.Select(p => p.DocAggCode.Split('+').ToList())
                                                 .Aggregate((p, q) => p.Concat(q).ToList());

            SetDocAggStatus(AggSuggestKeys, true);
        }

        GetSuggest = true;

        IsLoading = false;
    }
    #endregion

    #region Add Aggregate Suggest
    public async Task OnAddNewAggSuggestClick()
    {
        if (GetSuggest == false)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_GetAggSuggest_First, "error");

            return;
        }

        if (CurrentAggregate.CurrentAggType.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Document_Type, "error");

            return;
        }

        if (CurrentAggregate.CurrentAggStatus == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_DocumentStatus, "error");

            return;
        }

        IsLoading = true;

        AggregatableDocs = (await Api.PostAsyncByUriAndContext<List<GetAllAggDocVm>>("wms/Document", "SGetAggDocsByDocTypeAndStatus"
            , new GetAllAggDocVmContext()
            , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType)
            , new KeyValuePair<string, object>("documentStatus", CurrentAggregate.CurrentAggStatus)
            )).Value;

        if (AggregateSuggests.Any())
        {
            List<string> AggSuggestKeys = AggregateSuggests.Select(p => p.DocAggCode.Split('+').ToList())
                                                 .Aggregate((p, q) => p.Concat(q).ToList());

            SetDocAggStatus(AggSuggestKeys, true);
        }

        AggregatableDocsSearch = AggregatableDocs.Where(p => p.IsChoosed == false).ToList();

        CurrentAggregate.CurrentAggCode = string.Empty;

        AggregateSuggestDetails = new();

        await OpenModalSuggestDetails();

        IsLoading = false;
    }

    public async Task OnAggSuggestDetailsModalClick(string aggCode)
    {
        IsLoading = true;

        CurrentAggregate.CurrentAggCode = aggCode;

        AggregateSuggestDetails = (await Api.PostAsyncByUriAndContext<List<GetAllDocAggSuggestDetailVm>>("wms/Document",
            "SGetAllDocAggSuggestDetailByAggCode"
            , new GetAllDocAggSuggestDetailVmContext()
            , new KeyValuePair<string, object>("aggCode", aggCode)
            , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType)
            , new KeyValuePair<string, object>("documentStatus", CurrentAggregate.CurrentAggStatus)
            )).Value;

        IsLoading = false;

        await OpenModalSuggestDetails();
    }

    public async Task OnAggregatableDocValidSubmit()
    {
        AddAggDoc();
    }

    public async Task OnAggregatableDocSelectClick(string aggCode)
    {
        CrudDocKey.AddDocKey = aggCode;

        AddAggDoc();

        await CloseModalAggDoc();
    }
    #endregion

    #region Remove Agg Suggest
    public async Task OnRemoveModalClick(string docKey)
    {
        CrudDocKey.RemoveDocKey = docKey;

        await ModalRemove.Open(new());
    }

    public async Task OnSuggestDocRemoveClick()
    {
        RemoveAggDoc();
    }
    #endregion

    #region Search Aggregatable Documents
    public async Task OnAggDocModalClick()
    {
        await OpenModalAggDoc();
    }

    public async Task OnAggDocRefreshClick(MouseEventArgs e)
    {
        CrudDocKey.AddDocKey = string.Empty;

        SearchDocText = string.Empty;

        AggregatableDocsSearch = AggregatableDocs.Where(p => p.IsChoosed == false).ToList();
    }

    public void OnAggDocSearchClick(MouseEventArgs e)
    {
        AggregatableDocsSearch = AggregatableDocs.Where(p => p.IsChoosed == false && p.DocumentData.Contains(SearchDocText)).ToList();
    }

    #endregion

    #region Revoke And Aggregated
    public async Task OnRevokeButtonClick(DocumentHeaderDto doc)
    {
        RevokeDocument.Key = doc.Key;

        RevokeDocument.DocumentType = doc.DocumentType;

        RevokeDocument.DocumentStatusId = doc.DocumentStatusId;

        RevokeAggregateDescription = string.Empty;

        await RevokeModal.Open(new());
    }

    public async Task OnRevokeClick()
    {
        IsLoading = true;

        RevokeDocumentAggregateCommand revoke = new()
        {
            DocumentKeyTypes = new()
            {
                new()
                {
                    Key = RevokeDocument.Key,
                    Type = RevokeDocument.DocumentType
                }
            },
            DocumentStatus = RevokeDocument.DocumentStatusId,
            Description = RevokeAggregateDescription
        };

        var result = (await Api.PostAsync<int>("SRevokeDocumentAgg",
                new KeyValuePair<string, object>("revoke", revoke))).Value;

        if (result > -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            AggregatedDocuments.RemoveAll(p => p.Key == RevokeDocument.Key && p.DocumentType == RevokeDocument.DocumentType);

            if (AggregatedDocuments.Any())
            {
                GridAggregatedDocs.Rebind();
            }

            AggregatableDocs.ForEach(p => p.IsChoosed = false);

            GetSuggest = false;

            AggregateSuggests = new();

            RevokeAggregateDescription = string.Empty;

            await OnGetSuggestButtonClick();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }


        IsLoading = false;
    }

    public async Task OnGetAggregatedButtonClick(MouseEventArgs e)
    {
        if (CurrentAggregate.CurrentAggType.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Document_Type, "error");

            return;
        }

        if (CurrentAggregate.CurrentAggStatus == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_DocumentStatus, "error");

            return;
        }

        IsLoading = true;

        AggregatedDocuments = (await Api.PostAsyncByUri<List<DocumentHeaderDto>>("wms/Document", "SGetAllAggregatedDocs"
                    , new KeyValuePair<string, object>("documentType", CurrentAggregate.CurrentAggType)
                    , new KeyValuePair<string, object>("documentStatus", CurrentAggregate.CurrentAggStatus)
                    )).Value;

        IsLoading = false;
    }

    public async Task OnAggregatedDetailsModalClick(string aggCode, string docType, int docStatus)
    {
        IsLoading = true;

        AggregatedDetails = (await Api.PostAsyncByUriAndContext<List<GetAggregatedDocDetailsVm>>("wms/Document"
            , "SGetAggregatedDocDetailsByAggCode"
            , new GetAggregatedDocDetailsVmContext()
            , new KeyValuePair<string, object>("aggCode", aggCode)
            , new KeyValuePair<string, object>("documentType", docType)
            , new KeyValuePair<string, object>("documentStatus", docStatus)
            )).Value;

        IsLoading = false;

        await ModalAggregatedDetails.Open(new());
    }

    public async Task OnSingleDocPrintClick(DocumentHeaderDto doc)
    {
        PrintableDoc = doc;

        CurrentAggregate.CurrentAggCode = doc.Key;

        await ModalPrint.Open(new());
    }

    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        DocumentHeaderDto item = (DocumentHeaderDto)args.Item;

        args.Class += " bg-success";
    }
    #endregion

    #region Private methods
    private async Task CloseModalAggDoc()
    {
        await ModalAggDoc.Close(new());

        CrudDocKey.AddDocKey = string.Empty;

        SearchDocText = string.Empty;

        AddDocError = string.Empty;

        await ModalSuggestDetails.Open(new());
    }

    private async Task OpenModalAggDoc()
    {
        await OnAggDocRefreshClick(new());

        await ModalSuggestDetails.Close(new());

        await ModalAggDoc.Open(new());
    }

    private async Task OpenModalSuggestDetails()
    {
        await ModalSuggestDetails.Open(new());

        AddDocError = string.Empty;

        CrudDocKey.AddDocKey = string.Empty;
    }

    private void AddAggDoc()
    {
        AddDocError = string.Empty;

        if (CrudDocKey.AddDocKey.HasValue())
        {
            GetAllAggDocVm doc = AggregatableDocs.FirstOrDefault(p => p.DocumentKey.Equals(CrudDocKey.AddDocKey) && p.IsChoosed == false);

            if (doc is not null)
            {
                var aggDoc = AggregateSuggests.FirstOrDefault(p => p.DocAggCode.Equals(CurrentAggregate.CurrentAggCode));

                if (aggDoc is not null)
                {
                    aggDoc.DocAggCode = CurrentAggregate.CurrentAggCode += ('+' + doc.DocumentKey);

                    aggDoc.DocumentType = doc.DocumentType;

                    foreach (var docKey in doc.DocumentKey.Split('+'))
                    {
                        aggDoc.DocumentCount++;
                    }

                    aggDoc.ItemSum += doc.ItemSum;
                }
                else
                {
                    GetAllDocAggSuggestVm newAggDoc = new()
                    {
                        DocAggCode = doc.DocumentKey,
                        DocumentType = doc.DocumentType,
                        ItemSum = doc.ItemSum
                    };

                    foreach (var docKey in doc.DocumentKey.Split('+'))
                    {
                        newAggDoc.DocumentCount++;
                    }

                    AggregateSuggests.Insert(0, newAggDoc);

                    AggregateSuggests = AggregateSuggests.ToList();

                    CurrentAggregate.CurrentAggCode = doc.DocumentKey;
                }

                SetDocAggStatus(new() { doc.DocumentKey }, true);

                AggregateSuggestDetails.Add(Mapper.Map<GetAllDocAggSuggestDetailVm>(doc));

                GridSuggestDetails.Rebind();

                CrudDocKey.AddDocKey = string.Empty;
            }
            else
            {
                AddDocError = TextResources.APP_StringKeys_Error_Notfound_DocKey;
            }
        }
    }

    private void RemoveAggDoc()
    {
        var doc = AggregateSuggestDetails.First(p => p.DocumentKey.Equals(CrudDocKey.RemoveDocKey));

        var aggDoc = AggregateSuggests.FirstOrDefault(p => p.DocAggCode.Equals(CurrentAggregate.CurrentAggCode));

        if (CurrentAggregate.CurrentAggCode.Split('+').ToList().Count > 0)
        {
            aggDoc.DocAggCode = RemoveAggCode(doc.DocumentKey);

            foreach (var docKey in doc.DocumentKey.Split('+'))
            {
                aggDoc.DocumentCount = aggDoc.DocumentCount - 1;
            }

            aggDoc.ItemSum = aggDoc.ItemSum - doc.ItemSum;

            SetDocAggStatus(new() { doc.DocumentKey }, false);

            AggregateSuggestDetails.RemoveAll(p => p.DocumentKey.Equals(doc.DocumentKey));

            GridSuggestDetails.Rebind();
        }
    }

    private string RemoveAggCode(string code)
    {
        List<string> docKeys = CurrentAggregate.CurrentAggCode.Split('+').ToList();

        docKeys = docKeys.Except(code.Split('+').ToList()).ToList();

        if (docKeys.Count > 0)
        {
            CurrentAggregate.CurrentAggCode = docKeys.Aggregate((p, q) => p + '+' + q);
        }
        else
        {
            RemoveAggSuggest(false);
        }

        return CurrentAggregate.CurrentAggCode;
    }

    /// <summary>
    /// If aggregate suggest approved to aggregate in database, all document status must set 1
    /// If aggregate suggest just removed from suggestion list, all document status must set 0 
    /// </summary>
    /// <param name="docAggStatus"></param>
    private void RemoveAggSuggest(bool isChoosed)
    {
        AggregateSuggests = AggregateSuggests.Where(p => p.DocAggCode.NotEquals(CurrentAggregate.CurrentAggCode)).ToList();

        List<string> aggregatedDocKeys = CurrentAggregate.CurrentAggCode.Split('+').ToList();

        SetDocAggStatus(aggregatedDocKeys, isChoosed);

        AggregatableDocsSearch = AggregatableDocs.Where(p => p.IsChoosed == false).ToList();
    }

    private async Task Print()
    {
        IsLoading = true;

        if (PrintableDoc.DocumentItems is null)
        {
            PrintableDoc.DocumentItems = (await Api.PostAsyncByUri<List<DocumentItemDto>>("wms/Document", "SGetAllDocItems",
                 new KeyValuePair<string, object>("docKey", PrintableDoc.Key),
                 new KeyValuePair<string, object>("docType", PrintableDoc.DocumentType))).Value;
        }

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration["Settings:Company"];
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(DocumentItemPrintDto), Mapper.Map<List<DocumentItemPrintDto>>(PrintableDoc.DocumentItems))
        };

        List<KeyValuePair<string, object>> variables = new()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("DocumentKey", $"{PrintableDoc.Key}")
            , new("DateTimeString", $"تاریخ و ساعت درج سند: {PersianCalendarTools.GregorianToPersian(PrintableDoc.ImportDateTime)}-{PrintableDoc.ImportDateTime.Value.ToShortTimeString()}")
            , new("DocumentType", $"نوع سند: {(DocumentTypes.FirstOrDefault(p=>p.Code.ToString().Equals(PrintableDoc.DocumentType))?.Title ?? string.Empty)}")
            , new("Description", $"توضیحات: {PrintableDoc.Description}")
            , new("Status", $"وضعیت: {DocumentStatuses.FirstOrDefault(p=>p.Id==PrintableDoc.DocumentStatusId)?.Title}")
            , new("CompanyName", CompanyName)
            , new("PageTitle", PageTitle)
        };

        var headerDatas = JToken.Parse(PrintableDoc.HeaderData);

        if (headerDatas is not null)
        {
            foreach (JProperty prop in headerDatas)
            {
                variables.Add(new(prop.Name.ToString().Trim().Replace(' ', '_'), prop.Value.ToString()));
            }
        }

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

        IsLoading = false;
    }

    private void SetDocAggStatus(List<string> docKeys, bool isChoosed)
    {
        foreach (var doc in AggregatableDocs)
        {
            foreach (var docKey in docKeys)
            {
                if (doc.DocumentKey.Contains(docKey))
                {
                    AggregatableDocs.FirstOrDefault(p => p.DocumentKey.Contains(docKey)).IsChoosed = isChoosed;
                }
            }
        }
    }
    #endregion
}
