using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Shared.Components;

namespace Silo.Modules.Document.Pages;
public partial class DocumentDivision
{
    public bool IsLoading = true;

    public GetDividableDocumentQuery SearchDividableDoc = new();
    public List<GetDivisionSuggestItemsVm> DivisionSuggestItems = new();
    public List<GetRemainDividableDocumentItemsVm> RemainDividableItems = new();
    public List<GetAllDividedDocumentHeaderVM> DividedDocuments = new();
    public List<GetAllDividedDocumentItemVm> DividedDocumentDetails = new();
    public SaveDocumentDivideCommand SaveDivision = new()
    {
        NewDivisionDocItems = new()
    };
    public GetDividableDocumentQuery CurrentRevokeKeyType = new();
    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public string CompanyName;
    public DocumentHeaderDto PrintableDoc = new();
    public List<GetAllDocumentStatusVm> DocumentStatuses = new();
    public string DivideDescription = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Parameter] public string RedirectedDocumentCode { get; set; }
    [Parameter] public string RedirectedDocumentType { get; set; }

    public Modal RevokeModal { get; set; }
    public Modal DetailsModal { get; set; }

    public TelerikGrid<GetRemainDividableDocumentItemsVm> GridNewDivisionItems { get; set; }

    #region Events
    protected override async Task SiloInitializer()
    {
        DocumentTypes = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentTypesVm>>("wms/Document", "SGetAllDocumentType"
                , new GetAllDocumentTypesVmContext())).Value;

        if (DocumentTypes.Count == 1)
        {
            SearchDividableDoc.DocumentType = DocumentTypes.First().Code;
        }

        DocumentStatuses = (await Api.PostAsyncByUriAndContext<List<GetAllDocumentStatusVm>>("wms/Document", "SGetAllDocumentStatus"
                    , new GetAllDocumentStatusVmContext())).Value;

        if (RedirectedDocumentCode.HasValue()&& RedirectedDocumentType.HasValue())
        {
            SearchDividableDoc.DocumentKey = RedirectedDocumentCode;

            SearchDividableDoc.DocumentType = RedirectedDocumentType;

            await OnSearchValidSubmit();
        }

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        RemainDividableItems = new();

        SaveDivision = new()
        {
            NewDivisionDocItems = new()
        };

        SearchDividableDoc = new();

        if (DocumentTypes.Count == 1)
        {
            SearchDividableDoc.DocumentType = DocumentTypes.FirstOrDefault().Code;
        }

        DivideDescription = string.Empty;
        
        DividedDocuments = new();
    }
    #endregion

    #region Get Remain and Divided Docs
    public async Task OnSearchValidSubmit()
    {
        IsLoading = true;

        SaveDivision = new()
        {
            NewDivisionDocItems = new()
        };

        RemainDividableItems = (await Api.PostAsyncByContext<List<GetRemainDividableDocumentItemsVm>>("SGetRemainDividableDocumentItem"
                        , new GetRemainDividableDocumentItemsVmContext()
                        , new KeyValuePair<string, object>("request", SearchDividableDoc)
                        )).Value;

        DividedDocuments = (await Api.PostAsync<List<GetAllDividedDocumentHeaderVM>>("SGetDividedDocuments"
            , new KeyValuePair<string, object>("request", SearchDividableDoc)
            )).Value;

        if (RemainDividableItems.Count == 0 && DividedDocuments.Count == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_Notfound_DocKey, "error");
        }

        DivideDescription = string.Empty;

        IsLoading = false;
    }
    #endregion

    #region Suggest
    public async Task OnGetSuggestClick()
    {
        if (SearchDividableDoc.DocumentType.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_DocKey), "error");

            return;
        }
        if (SearchDividableDoc.DocumentKey.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Document_Type), "error");

            return;
        }

        await OnSearchValidSubmit();

        IsLoading = true;

        DivisionSuggestItems = (await Api.PostAsyncByContext<List<GetDivisionSuggestItemsVm>>("SGetDivisionSuggestItems"
                , new GetDivisionSuggestItemsVmContext()
                , new KeyValuePair<string, object>("request", SearchDividableDoc)
                )).Value;

        foreach (var suggest in DivisionSuggestItems)
        {
            var remain = RemainDividableItems.FirstOrDefault(p => p.ProductCode == suggest.ProductCode);

            if (remain is not null)
            {
                remain.Count = 0;
            }

            SaveDivision.NewDivisionDocItems.Add(new()
            {
                ProductCode = suggest.ProductCode,
                ProductTitle = suggest.ProductTitle,
                ProductUnit = suggest.ProductUnit,
                Count = suggest.Count
            });
        }

        IsLoading = false;
    }
    #endregion

    #region Client CRUD
    public async Task OnAddNewItemClick(GetRemainDividableDocumentItemsVm Item)
    {
        if (Item.Count - Item.DivisionCount < 0)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Max_Value, Item.Count), "error");

            return;
        }

        if (Item.DivisionCount == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }

        if (RemainDividableItems.Count == 1 && Item.Count - Item.DivisionCount == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Remain_Document, "error");

            return;
        }

        if (SaveDivision.NewDivisionDocItems.Exists(p => p.ProductCode == Item.ProductCode))
        {
            SaveDivision.NewDivisionDocItems.FirstOrDefault(p => p.ProductCode == Item.ProductCode).Count += Item.DivisionCount;
        }
        else
        {
            SaveDivision.NewDivisionDocItems.Add(new()
            {
                ProductCode = Item.ProductCode,
                ProductTitle = Item.ProductTitle,
                ProductUnit = Item.ProductUnit,
                Count = Item.DivisionCount
            });
        }

        Item.Count = Item.Count - Item.DivisionCount;

        Item.DivisionCount = 0;

        if (GridNewDivisionItems is not null)
        {
            GridNewDivisionItems.Rebind();
        }
    }

    public async Task OnDividedItemRemove(GetRemainDividableDocumentItemsVm Item)
    {
        SaveDivision.NewDivisionDocItems.Remove(Item);

        RemainDividableItems.FirstOrDefault(p => p.ProductCode == Item.ProductCode).Count += Item.Count;

        if (GridNewDivisionItems is not null)
        {
            GridNewDivisionItems.Rebind();
        }
    }
    #endregion

    #region Approve Division
    public async Task OnApproveDivisionClick()
    {
        if (SaveDivision.NewDivisionDocItems.Count() == 0)
        {
            Notification.Show(string.Concat(TextResources.APP_StringKeys_Document_Division, " ",
                                            TextResources.APP_StringKeys_Validation_EmptyError), "error");

            return;
        }

        SaveDivision.RemainDocItems = RemainDividableItems.Where(p => p.Count != 0).ToList();

        SaveDivision.CurrentDocKey = SearchDividableDoc.DocumentKey;

        SaveDivision.CurrentDocType = SearchDividableDoc.DocumentType;

        SaveDivision.Description = DivideDescription;

        var result = (await Api.PostAsync<int>("SDivideDocument"
            , new KeyValuePair<string, object>("division", SaveDivision)
            )).Value;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            DivideDescription = string.Empty;

            await OnSearchValidSubmit();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }
    }
    #endregion

    #region Revoke And Divided
    public async Task OnDividedDocDetailsClick(GetAllDividedDocumentHeaderVM doc)
    {
        IsLoading = true;

        var reuest = new GetDividableDocumentQuery()
        {
            DocumentKey = doc.Key,
            DocumentType = doc.DocumentType
        };

        DividedDocumentDetails = (await Api.PostAsync<List<GetAllDividedDocumentItemVm>>("SGetDividedDocumentDetails"
            , new KeyValuePair<string, object>("request", reuest)
            )).Value;

        await DetailsModal.Open(new());

        IsLoading = false;
    }

    public async Task OnRevokeModalClick(GetAllDividedDocumentHeaderVM doc)
    {
        CurrentRevokeKeyType = new()
        {
            DocumentKey = doc.Key,
            DocumentType = doc.DocumentType,
            DocumentStatus = doc.DocumentStatus
        };

        DivideDescription = string.Empty;

        await RevokeModal.Open(new());
    }

    public async Task OnRevokeDivisionClick()
    {
        IsLoading = true;

        CurrentRevokeKeyType.Description = DivideDescription;

        var result = (await Api.PostAsync<int>("SRevokeDocumentDivision"
            , new KeyValuePair<string, object>("request", CurrentRevokeKeyType)
            )).Value;

        await RevokeModal.Close(new());

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Success, "success");

            await OnSearchValidSubmit();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Message_Failure, "error");
        }

        IsLoading = false;
    }

    public void OnRowDocRenderHandler(GridRowRenderEventArgs args)
    {
        GetAllDividedDocumentHeaderVM item = (GetAllDividedDocumentHeaderVM)args.Item;

        args.Class += " bg-success";
    }
    #endregion

    #region Print
    public async Task OnPrintDocumentClick(string documentKey, string documentType)
    {
        IsLoading = true;

        PrintableDoc = (await Api.PostAsyncByUri<DocumentHeaderDto>("wms/Document", "SGetDocumentHeaderAndItems",
             new KeyValuePair<string, object>("documentKey", documentKey),
             new KeyValuePair<string, object>("documentType", documentType))).Value;


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

        IsLoading = false;
    }

    public async Task OnPreventPrintDocumentClick()
    {
        Notification.Show(TextResources.APP_StringKeys_Divide_Document_Required, "error");

        Notification.Show(TextResources.APP_StringKeys_RemainDocument_Prevent_Print, "error");
    }
    #endregion
}
