using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Silo.Application.Dto;
using Silo.Domains.Entities;

namespace Silo.Modules.Document.Pages;
public partial class DocumentUpload
{
    public bool IsLoading = true;
    public string DocumentStatus;
    public JToken HeaderDatas;

    public string CompanyName;
    public DocumentHeaderDto PrintableDoc;

    public List<GetAllDocumentTypesVm> DocumentTypes = new();
    public List<TelerikDropDownItemGeneric<int>> DocumentTypeItems = new();
    public GetDocumentByKeyQuery DocumentRequest = new();

    public List<GetAllDocumentItemVm> DocumentItems;

    public List<string> DynamicFieldColumns = new();
    public List<GetAllDynamicFieldVm> DynamicFields = new();
    public List<TelerikDropDownItemGeneric<int?>> DocumentCheckTypes = new()
    {
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_Exact,
            Value = (int?)DocumentCheckType.Exact
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_ProductCodeRemain,
            Value = (int?)DocumentCheckType.ProductCodeAndDocCodeRemain
        },
        new()
        {
            Name = TextResources.APP_StringKeys_Document_CheckType_DocumentRemain,
            Value = (int?)DocumentCheckType.DocCodeRemain
        }
    };

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task SiloInitializer()
    {
        var result = (await Api.SendAsyncObjectByUri<GetAllActionTypesVm>(HttpMethod.Get
       , "ActionType/ReadAll")).Value.List;


        DocumentTypes = Mapper.Map<List<GetAllDocumentTypesVm>>(result);

        DocumentTypeItems = DocumentTypes.Select(p => new TelerikDropDownItemGeneric<int>()
        {
            Name = p.Title,
            Value = int.Parse(p.Code)
        }).ToList();

        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir").GetAwaiter();

        IsLoading = false;
    }

    public async Task OnValidSubmit(EditContext context)
    {
        IsLoading = true;

        DynamicFields = (await Api.PostAsyncByUri<List<GetAllDynamicFieldVm>>("wms/Document","SGetDynamicFieldsByActionTypeId",
            new KeyValuePair<string, object>("actionTypeId", DocumentRequest.DocumentType))).Value;

        if (DynamicFields.Any())
        {
            DynamicFieldColumns = DynamicFields.Select(p => p.Title).ToList();
        }
        else
        {
            DynamicFieldColumns = new();
        }

        var result = await Api.PostAsyncByUriAndContext<GetAllDocumentHeaderVm>("wms/Document","SGetDocumentHeaderAndItems"
            , new GetAllDocumentHeaderVmContext()
            , new KeyValuePair<string, object>("documentKey", DocumentRequest.DocumentKey)
            , new KeyValuePair<string, object>("documentType", DocumentRequest.DocumentType));

        if (result.Successful)
        {
            if (result.Value is not null)
            {
                await ClearResult();

                DocumentStatus = result.Value.Status.Equals(0) ? TextResources.APP_StringKeys_Aggregatable : TextResources.APP_StringKeys_Aggregated;

                PrintableDoc = Mapper.Map<DocumentHeaderDto>(result.Value);

                HeaderDatas = JToken.Parse(result.Value?.HeaderData);

                DocumentItems = result.Value.DocumentItems.OrderByDescending(item => item.Id).ToList();

                Notification.Show(TextResources.APP_StringKeys_Alert_Success
               , "success");
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_NotFound
               , "warning");

                await ClearResult();
            }
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail
           , "error");
        }

        IsLoading = false;
    }

    public async Task OnInvalidExcelUploadClick(MouseEventArgs e)
    {
        Notification.Show(TextResources.APP_StringKeys_Validation_Document_Type
            , "error");
    }

    public async Task OnCompleteUploadExcel(string json)
    {
        IsLoading = true;

        if (DocumentRequest.DocumentType.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Document_Type
                , "error");

            IsLoading = false;

            return;
        }

        JToken token = JToken.Parse(json);

        string originalFileName = token["FileName"].ToString();

        var isExist = (await Api.PostAsync<bool>("SCheckUniqueFileName"
    , new KeyValuePair<string, object>("originalFileName", originalFileName)
    , new KeyValuePair<string, object>("type", "dynamicExcel"))).Value;

        if (isExist)
        {
            Notification.Show(TextResources.APP_StringKeys_FileName_IsExist
                , "error");

            IsLoading = false;

            return;
        }

        string path = token["Path"].ToString();

        var result = await Api.PostFileAsync<bool>("UploadExcelDynamic", path
            , new("type", "dynamicExcel")
            , new("documentType", DocumentRequest.DocumentType.ToString())
            , new("fileName", originalFileName)
            , new("documentCheckType", DocumentRequest.DocumentCheckType == null ? "0" :
                                        DocumentRequest.DocumentCheckType.ToString()));

        if (result.Successful)
        {
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

    public async Task OnClearClick(MouseEventArgs e)
    {
        await ClearResult();

        DocumentRequest = new()
        {
            DocumentCheckType = 0
        };
    }

    public async Task OnPrintDocumentClick(MouseEventArgs e)
    {
        IsLoading = true;

        if (PrintableDoc is not null)
        {
            await Print();
        }

        IsLoading = false;
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
            , new("Status", $"وضعیت: {(PrintableDoc.DocumentStatusId.Equals(0) ? TextResources.APP_StringKeys_Aggregatable : TextResources.APP_StringKeys_Aggregated)}")
            , new("CompanyName", CompanyName)
        };

        var headerDatas = JToken.Parse(PrintableDoc.HeaderData);

        if (headerDatas is not null)
        {
            foreach (var column in DynamicFieldColumns)
            {
                if (headerDatas[column] is not null)
                {
                    variables.Add(new(column.Trim().Replace(' ', '_'), (headerDatas[column]).ToString()));
                }
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

    private async Task ClearResult()
    {
        HeaderDatas = null;

        DocumentItems = new();

        PrintableDoc = null;
    }
}
