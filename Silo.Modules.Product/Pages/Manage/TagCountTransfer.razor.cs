using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;
using Silo.Components.DynamicField;
using Silo.Shared.Components.Modals;
using Silo.Shared.Components.Print;

namespace Silo.Modules.Product.Pages;
public partial class TagCountTransfer
{
    private bool _shouldScrollToBottom;
    public bool IsLoading = true;
    public string UserId;
    public List<GetProductInfosBySerialVm> SourceProductInfos = new();
    public JToken SourceProductProperties;
    public List<DynamicFieldWithValueDto> DynamicFieldDtos = new();
    public TransferCountTagModes TransferMode = TransferCountTagModes.NewSerial;
    public string DestinationSerial = string.Empty;
    public List<GetProductInfosBySerialVm> DestinationProductInfos = new();
    public string TransferQuantityText = string.Empty;
    public TransferCountTagVm TransferResult;

    public DynamicFieldFillValue DynamicFieldRef { get; set; }
    public ProductSerialModal DestinationModal { get; set; }
    public SelectPrintFormat SelectPrintFormatRef { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_shouldScrollToBottom)
        {
            _shouldScrollToBottom = false;

            await JSRuntime.InvokeVoidAsync("scrollToBottom");
        }
    }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        await LoadDynamicFields();

        IsLoading = false;
    }

    public async Task OnSourceSerialSelected(string serial)
    {
        IsLoading = true;

        SourceProductInfos = new();

        SourceProductProperties = null;

        TransferResult = null;

        foreach (var field in DynamicFieldDtos)
        {
            field.Value = string.Empty;
        }

        await GetSourceProductInfos(serial);

        IsFiltersShown = false;

        if (SourceProductInfos.Neither())
        {
            IsFiltersShown = true;

            Notification.Show(TextResources.APP_StringKeys_Message_TagNotFound, "error");
        }

        IsLoading = false;
    }

    public async Task OnFilterClear()
    {
        await OnClearClick();
    }

    public async Task OnClearClick()
    {
        SourceProductInfos = new();

        SourceProductProperties = null;

        DestinationSerial = string.Empty;

        DestinationProductInfos = new();

        TransferQuantityText = string.Empty;

        TransferResult = null;

        TransferMode = TransferCountTagModes.NewSerial;

        foreach (var field in DynamicFieldDtos)
        {
            field.Value = field.DefaultValue ?? string.Empty;
        }

        IsFiltersShown = true;
    }

    public void OnTransferModeChanged(TransferCountTagModes mode)
    {
        TransferMode = mode;

        DestinationSerial = string.Empty;

        DestinationProductInfos = new();
    }

    public async Task OnSearchDestinationClick()
    {
        if (string.IsNullOrWhiteSpace(DestinationSerial))
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_ProductSerial), "error");
         
            return;
        }

        IsLoading = true;
       
        await LoadDestinationProductInfos(DestinationSerial);
        
        IsLoading = false;
    }

    public async Task OnDestinationSerialsSelected(List<GetAllProductBySerialVm> products)
    {
        if (products is null || products.Neither())
        { 
            return; 
        }

        DestinationSerial = products[0].ProductSerial;

        IsLoading = true;
    
        await LoadDestinationProductInfos(DestinationSerial);
      
        IsLoading = false;
    }

    public async Task OnExecuteTransferClick()
    {
        if (SourceProductInfos.Neither())
        {
            return;
        }

        if (!decimal.TryParse(TransferQuantityText, out decimal quantity) || quantity <= 0)
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Count), "error");
            
            return;
        }

        if (quantity > SourceProductInfos[0].ProductCount)
        {
            Notification.Show(TextResources.APP_StringKeys_You_Have_Permission_To_Transfer_Value_To_Registred_Tag, "error");
         
            return;
        }

        if (TransferMode == TransferCountTagModes.ExistingSerial)
        {
            if (DestinationSerial.HasNoValue() || DestinationProductInfos.Neither())
            {
                Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Destination), "error");
               
                return;
            }
        }

        IsLoading = true;
      
        TransferResult = null;

        var sourceSerial = SourceProductInfos[0].ProductSerial;
        
        var toEpc = TransferMode == TransferCountTagModes.ExistingSerial
            ? DestinationProductInfos[0].TagEpc
            : $"esv{sourceSerial}";

        var result = (await Api.PostAsync<string>("STransferProductCountToAnotherTag"
            , new KeyValuePair<string, object>("FromSerial", sourceSerial)
            , new KeyValuePair<string, object>("ToEPC", toEpc)
            , new KeyValuePair<string, object>("Count", quantity.ToString())
            , new KeyValuePair<string, object>("DestinationCode", "-1")
            , new KeyValuePair<string, object>("userToken", UserId))).Value;

        if (result.HasValue() || TransferMode == TransferCountTagModes.ExistingSerial)
        {
            string destinationSerial = TransferMode == TransferCountTagModes.ExistingSerial
                ? DestinationProductInfos[0].ProductSerial
                : result ?? string.Empty;

            TransferResult = new TransferCountTagVm
            {
                SourceSerial = sourceSerial,
                DestinationSerial = destinationSerial,
                ProductCode = SourceProductInfos[0].ProductCode,
                ProductName = SourceProductInfos[0].ProductName,
                TransferredQuantity = quantity
            };

            _shouldScrollToBottom = true;

            await GetSourceProductInfos(sourceSerial);

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "warning");
        }

        IsLoading = false;
    }

    public async Task OnPrintActionClick(GetPrintFormatsByPageTitleDto format)
    {
        await PrintTransferResult(format.Path);
    }

    private async Task PrintTransferResult(string reportFileName)
    {
        IsLoading = true;

        var freshData = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
            , new KeyValuePair<string, object>("serial", TransferResult.DestinationSerial))).Value;

        if (freshData is null || freshData.Neither())
        {
            Notification.Show(TextResources.APP_StringKeys_Message_TagNotFound, "error");
            IsLoading = false;
            return;
        }

        var info = freshData[0];

        string logoPath = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", logoPath)
        };

        string companyName = Configuration["Settings:Company"];

        var variables = new List<KeyValuePair<string, object>>()
        {
              new("DateString", $"تاریخ و ساعت گزارش: {PersianCalendarTools.GregorianToPersian(DateTime.Now)}-{DateTime.Now.ToShortTimeString()}")
            , new("CompanyName", companyName)
            , new("ProductName", info.ProductName)
            , new("ProductCode", info.ProductCode)
            , new("ProductCount", info.ProductCount)
            , new("ProductValue", info.ProductValue)
            , new("ProductCountInPack", info.ProductCountInPack)
            , new("ProductPackWeight", info.ProductPackWeight)
            , new("ProductPackVolume", info.ProductPackVolume)
            , new("TagEpc", info.TagEpc)
            , new("TagStatusTitle", info.TagStatusTitle)
            , new("Warehouse", info.Warehouse)
            , new("TagZone", info.TagZone)
            , new("RegisterUserName", info.RegisterUserName)
            , new("ProductSerial", info.ProductSerial)
            , new("SourceSerial", TransferResult.SourceSerial)
            , new("TransferredQuantity", TransferResult.TransferredQuantity)
        };


        if (info.ProductProperties.HasValue())
        {
            var productProperties = JObject.Parse(info.ProductProperties);

            foreach (var property in productProperties.Properties())
            {
                variables.Add(new KeyValuePair<string, object>(
                    property.Name.Trim().Replace(' ', '_'),
                    property.Value?.ToString() ?? string.Empty));
            }
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new("Serials", new List<TelerikDropDownItem>()
            {
                new() { Value = info.ProductSerial }
            })
        };

        CreatePreparedReportCommand command = new()
        {
            Title = PageTitle,
            ReportFileName = reportFileName,
            Variables = variables,
            DataSources = dataSources,
            Images = images
        };

        var response = await Api.SendAsyncObjectByUri<CreatePreparedReportVm>(HttpMethod.Post
            , "PreparedReport/Create"
            , command);

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    

    private async Task GetSourceProductInfos(string serial)
    {
        SourceProductInfos = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
            , new KeyValuePair<string, object>("serial", serial))).Value ?? new();

        if (SourceProductInfos.Any() && SourceProductInfos[0].ProductProperties.HasValue())
        {
            SourceProductProperties = JToken.Parse(SourceProductInfos[0].ProductProperties);

            foreach (var field in DynamicFieldDtos)
            {
                string value = SourceProductProperties.Value<string>(field.Title) ?? string.Empty;
               
                field.Value = value.HasValue() ? SourceProductProperties[field.Title].ToString() : string.Empty;
            }
        }
        else
        {
            foreach (var field in DynamicFieldDtos)
            {
                field.Value = string.Empty;
            }
        }

        foreach (var field in DynamicFieldDtos)
        {
            field.IsReadOnly = true;
        }
    }

    private async Task LoadDestinationProductInfos(string serial)
    {
        DestinationProductInfos = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
            , new KeyValuePair<string, object>("serial", serial))).Value ?? new();

        if (!DestinationProductInfos.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Message_TagNotFound, "error");
        }
    }

    private async Task LoadDynamicFields()
    {
        var fields = (await Api.PostAsync<List<GetAllDynamicFieldVm>>("SGetDynamicFieldsByActionTypeId",
            new KeyValuePair<string, object>("actionTypeId", 0))).Value;

        if (fields is null)
        { 
            return;
        }

        DynamicFieldDtos = fields.OrderBy(f => f.Id).Select(f => new DynamicFieldWithValueDto
        {
            Title = f.Title,
            Value = string.Empty,
            ValueType = f.ValueType,
            DefaultValue = f.DefaultValue ?? string.Empty,
            IsRequired = f.IsRequired ?? false,
            ValueOptions = f.ValueOptionList,
            IsReadOnly = true
        }).ToList();
    }
}
