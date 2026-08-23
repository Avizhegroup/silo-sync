using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Application.Dto.DynamicField;
using Silo.Components.DynamicField;
using Silo.Application.Features;
using Silo.Shared.Components;
using Silo.Shared.Components.Print;

namespace Silo.Modules.Product.Pages;
public partial class TagEdit
{
    public bool IsLoading = true;
    public string UserId;
    public string ActiveProductSerial;
    public List<GetProductInfosBySerialVm> ProductInfos = new();
    public JToken ProductProperties;
    public List<DynamicFieldWithValueDto> DynamicFieldDtos = new();

    public DynamicFieldFillValue DynamicFieldRef { get; set; }
    public SelectPrintFormat SelectPrintFormatRef { get; set; }
    public Gallery GalleryRef { get; set; }


    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        await LoadDynamicFields();

        IsLoading = false;
    }

    public async Task OnSerialSelected(string serial)
    {
        IsLoading = true;

        ActiveProductSerial = serial;

        ProductInfos = new();
    
        ProductProperties = null;

        foreach (var field in DynamicFieldDtos)
        {
            field.Value = string.Empty;
        }

        await GetProductInfos();

        IsFiltersShown = false;

        if (!ProductInfos.Any())
        {
            IsFiltersShown = true;
            Notification.Show(TextResources.APP_StringKeys_Message_TagNotFound, "error");
        }



        IsLoading = false;
    }

    public async Task OnSaveClick()
    {
        IsLoading = true;

        string dynamicJson = DynamicFieldRef is not null
           ? await DynamicFieldRef.GetJsonData()
           : "{}";

        var result = (await Api.PostAsync<bool>("SEditProductInformation"
            , new ("serial", ProductInfos[0].ProductSerial)
            , new ("productCode", "-1")
            , new("line", "-1")
            , new("shift", "-1")
            , new("tagZone", "-1")
            , new("refCode", "-1")
            , new("desc", "-1")
            , new("properties", dynamicJson)
            , new("userToken", UserId))).Value;

        if (result)
        {
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
        await PrintSelectedFormat(format.Path);
    }

    public async Task PrintSelectedFormat(string reportFileName)
    {
        if (!ProductInfos.Any()) return;

        await OnSerialSelected(ActiveProductSerial);

        var info = ProductInfos[0];

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
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
        };

        foreach (var field in DynamicFieldDtos)
        {
            variables.Add(new(field.Title.ToString().Trim().Replace(' ', '_'), field.Value.ToString()));
        }

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new("Serials", new List<TelerikDropDownItem>()
            {
                new()
                {
                    Value = info.ProductSerial
                }
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
    }

    public async Task OnFilterClear()
    {
        await OnClearClick();
    }

    public async Task OnClearClick()
    {
        ActiveProductSerial = string.Empty;

        ProductInfos = new();

        ProductProperties = null;


        foreach (var field in DynamicFieldDtos)
        {
            field.Value = field.DefaultValue ?? string.Empty;
        }

        IsFiltersShown = true;

    }


    private async Task GetProductInfos()
    {
        ProductInfos = (await Api.PostAsync<List<GetProductInfosBySerialVm>>("SGetTagHistoryProductInfo"
                        , new KeyValuePair<string, object>("serial", ActiveProductSerial))).Value;

        if (ProductInfos.Any() && ProductInfos[0].ProductProperties.HasValue())
        {
            ProductProperties = JToken.Parse(ProductInfos[0].ProductProperties);

            foreach (var field in DynamicFieldDtos)
            {
                string value = ProductProperties.Value<string>(field.Title) ?? string.Empty;

                if (value.HasValue())
                {
                    field.Value = ProductProperties[field.Title].ToString();
                }
                else
                {
                    field.Value = string.Empty;
                }
            }
        }
    }

    public async Task OnOpenGallery()
    {
        if (string.IsNullOrEmpty(ActiveProductSerial))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await GalleryRef.Show(UserId, GalleryUsageType.Tag, ActiveProductSerial);
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
            IsReadOnly = f.IsReadOnly ?? false
        }).ToList();
    }
}

