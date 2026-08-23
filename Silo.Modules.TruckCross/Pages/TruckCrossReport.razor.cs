using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Pages;
public partial class TruckCrossReport
{
    public bool IsLoading = true;
    public TruckCrossDataDto CrossRequest = new();
    public List<TruckCrossDataDto> Crosses;
    public GetTruckCrossReportQuery Search = new();
    public List<GetAllTruckTypesVm> TruckTypes = new();
    public List<GetAllTruckCrossPresentCauseVm> Causes = new();
    public List<GateProductDto> GateProducts = new();
    public List<GateProductDto> Docs = new();
    public List<TelerikDropDownItem> Destinations = new();
    public List<GetLoadedCargoProductsDto> LoadedProducts = new();
    public string UserId;
    public string Username;
    public GetAllTruckCrossPresentCauseVm CauseRequest = new();
    public string CompanyName = string.Empty;
    public List<TelerikDropDownItemGeneric<TruckCrossStatuses>> TruckCrossStatusList;
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes;
    public List<GetAllTruckCrossShipmentVm> Shipments;
    public List<GetAllTruckCrossCustomerVm> Customers;
    public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations;
    public List<GetAllDynamicFieldSectionsVm> DynamicFieldsSections;
    public List<DynamicFieldDto> DynamicFields;

    public Gallery Gallery { get; set; }
    public Modal ModalLoadedProducts { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IExport Exporter { get; set; }

    protected override async Task SiloInitializer()
    {
        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Username = (await AuthState.GetAuthenticationStateAsync()).User.GetUsername();

        Causes = (await Api.PostAsyncByUri<List<GetAllTruckCrossPresentCauseVm>>("wms/TruckCross", "SGetTruckPresentCause")).Value;

        TruckTypes = (await Api.PostAsyncByUri<List<GetAllTruckTypesVm>>("wms/TruckCross", "SGetTruckType")).Value;

        TruckCrossStatusList = new()
        {
            new()
            {
                Value =TruckCrossStatuses.Present,
                Name = TextResources.APP_StringKeys_TruckCross_Presented
            },
            new()
            {
                Value = TruckCrossStatuses.Enter,
                Name = TextResources.APP_StringKeys_TruckCross_Entered
            },
            new()
            {
                Value = TruckCrossStatuses.Exit,
                Name = TextResources.APP_StringKeys_TruckCross_Exited
            },
            new()
            {
                Value = TruckCrossStatuses.Revoke,
                Name = TextResources.APP_StringKeys_TruckCross_Revoked
            }
        };

        OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetAllTruckCrossOperationType")).Value;

        OperationDestinations = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationDestinationsVm>>("wms/TruckCross", "SGetAllTruckCrossOperationDestination")).Value;

        Shipments = (await Api.PostAsyncByUri<List<GetAllTruckCrossShipmentVm>>("wms/TruckCross", "SGetAllTruckCrossShipment")).Value;

        Customers = (await Api.PostAsyncByUri<List<GetAllTruckCrossCustomerVm>>("wms/TruckCross", "SGetAllTruckCrossCustomer")).Value;

        await LoadDynamicFields();

        IsLoading = false;
    }

    #region Valid Submits
    public async Task OnSearchValidSubmit(EditContext context)
    {
        IsLoading = true;

        Crosses = (await Api.PostAsyncByUri<List<TruckCrossDataDto>>("wms/TruckCross", "SReportTruckCrossForm"
                          , new KeyValuePair<string, object>("search", Search))).Value;

        IsLoading = false;

        IsFiltersShown = false;
    }
    #endregion

    #region Clear Button
    public async Task OnSearchClearClick(MouseEventArgs e)
    {
        Search = new();

        Crosses = null;
    }
    #endregion

    public async Task OnGetLoadedProductsClick(MouseEventArgs e, int Id)
    {
        IsLoading = true;

        LoadedProducts = (await Api.SendAsyncObjectByUri<GetLoadedCargoProductsVm>(HttpMethod.Get
        , "Crosses/GetLoadedCargoProductByCrossId"
        , new GetLoadedCargoProductsQuery()
        {
            TruckCrossId = Id,
        })).Value.LoadedPoducts;

        IsLoading = false;

        await ModalLoadedProducts.Open(e);
    }

    public async Task OnCrossPrintClick(TruckCrossDataDto data)
    {
        List<GateProductPrintableVm> printableProducts = new();

        TruckCrossHeaderPrintDto printableTruckCrossHeader = new();

        if (data.ExitUsername.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_TruckCross_SaveExit, "error");

            return;
        }

        IsLoading = true;

        printableProducts = (await Api.PostAsyncByContext<List<GateProductPrintableVm>>("SGetGateProductsByTruckCrossId"
        , new GateProductPrintableVmContext()
            , new KeyValuePair<string, object>("truckCrossId", data.Id))).Value;

        List<TruckCrossHeaderPrintDto> result = (await Api.PostAsyncByUri<List<TruckCrossHeaderPrintDto>>("wms/TruckCross", "SGetPrintableTruckCrossData"
            , new KeyValuePair<string, object>("truckCrossId", data.Id))).Value;

        if (result.Any())
        {
            printableTruckCrossHeader = result.First();

            printableTruckCrossHeader.EnterDate =
                printableTruckCrossHeader.EnterDateTime == null ? "" :
                PersianCalendarTools.GregorianToPersian(printableTruckCrossHeader.EnterDateTime);

            printableTruckCrossHeader.PresentDate =
                printableTruckCrossHeader.PresentDateTime == null ? "" :
                PersianCalendarTools.GregorianToPersian(printableTruckCrossHeader.PresentDateTime);

            printableTruckCrossHeader.ExitDate =
                printableTruckCrossHeader.ExitDateTime == null ? "" :
                PersianCalendarTools.GregorianToPersian(printableTruckCrossHeader.ExitDateTime);
        }

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
            new("CompanyName", CompanyName)
        };

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GateProductPrintableVm), printableProducts),
            new(nameof(TruckCrossHeaderPrintDto), new List<TruckCrossHeaderPrintDto> { printableTruckCrossHeader })
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ExitTruckCross",
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

        await Exporter.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnPrintAllReportsClick()
    {
        List<GetCargoByTruckCrossIdVm> printableProducts = new();
      
        List<TruckCrossHeaderPrintDto> printableTruckCrossHeader = new();

        IsLoading = true;

        printableProducts = (await Api.PostAsyncByContext<List<GetCargoByTruckCrossIdVm>>("SGetCargoByTruckCrossId"
            , new GetCargoByTruckCrossIdVmContext()
            , new KeyValuePair<string, object>("truckCrossIds", Crosses.Select(p => p.Id).ToList()))).Value;

        printableTruckCrossHeader = (await Api.PostAsyncByUri<List<TruckCrossHeaderPrintDto>>("wms/TruckCross"
            , "SGetAllPrintableTruckCrossDatas"
            , new KeyValuePair<string, object>("truckCrossIds", Crosses.Select(p => p.Id).ToList()))).Value;

        foreach (var header in printableTruckCrossHeader)
        {
            foreach (var item in printableProducts.Where(p => p.TruckCrossId == header.Id))
            {
                header.RelatedCargos += $"{item.DocumentCode}, {item.ProductCode}, {item.ProductName}, {item.SumCount}" + System.Environment.NewLine;
            }
        }

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        var allFieldIds = printableTruckCrossHeader.Where(h => h.DynamicDataDict is not null)
                                                            .SelectMany(h => h.DynamicDataDict.Keys)
                                                            .Distinct()
                                                            .OrderBy(id => id)
                                                            .ToList();

        Dictionary<int, int>? fieldMapping = new ();

        for (int i = 0; i < allFieldIds.Count && i < DynamicFields.Count; i++) 
        {
            fieldMapping[allFieldIds[i]] = i + 1;
        }

        foreach (TruckCrossHeaderPrintDto header in printableTruckCrossHeader)
        {
            if (header.DynamicDataDict is null)
            {
                continue;
            }

            foreach (var kvp in header.DynamicDataDict)
            {
                if (fieldMapping.TryGetValue(kvp.Key, out int index))
                {
                    var propertyName = $"DynamicField{index}";

                    var property = typeof(TruckCrossHeaderPrintDto).GetProperty(propertyName);
                   
                    property?.SetValue(header, kvp.Value);
                }
            }
        }

        var variables = new List<KeyValuePair<string, object>>()
        {
            new("CompanyName", CompanyName),
            new("DateString", PersianCalendarTools.GregorianToPersian(DateTime.Now)),
            new("PageTitle", PageTitle)
        };

        for (int i = 0; i < allFieldIds.Count && i < DynamicFields.Count; i++)
        {
            var field = DynamicFields?.FirstOrDefault(f => f.Id == allFieldIds[i]);
           
            if (field is null)
            {
                continue;
            }
              
            variables.Add(new($"DynamicFieldTitle{i + 1}", field.Title));
        }

        variables.Add(new("DynamicFieldCount", Math.Min(allFieldIds.Count, 20)));

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
           new("Image_Logo", path)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GetCargoByTruckCrossIdVm), printableProducts),
            new(nameof(TruckCrossHeaderPrintDto), printableTruckCrossHeader)
        };

        var command = new CreatePreparedReportCommand
        {
            Title = PageTitle,
            ReportFileName = "ExitTruckCrossAll",
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

    #region Gallery Openers
    public async Task OnOpenDriverGallery(MouseEventArgs e, string NationalCode)
    {
        if (NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(NationalCode
            , GalleryUsageType.TruckCrossDriver);
    }

    public async Task OnOpenPresentGallery(MouseEventArgs e, string NationalCode, int Id)
    {
        if (Id.Equals(0) || NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(NationalCode
            , GalleryUsageType.TruckCrossPresent
            , Id.ToString());
    }

    public async Task OnOpenEnterGallery(MouseEventArgs e, string NationalCode, int Id)
    {
        if (Id.Equals(0) || NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(NationalCode
            , GalleryUsageType.TruckCrossEnter
            , Id.ToString());
    }

    public async Task OnOpenExitGallery(MouseEventArgs e, string NationalCode, int Id)
    {
        if (Id.Equals(0) || NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(NationalCode
            , GalleryUsageType.TruckCrossExit
            , Id.ToString());
    }
    #endregion

    private async Task LoadDynamicFields()
    {
        DynamicFieldsSections = (await Api.PostAsyncByUri<List<GetAllDynamicFieldSectionsVm>>(
            "wms/Document",
            "GetAllDynamicFieldSections")).Value;

        DynamicFieldsSections = DynamicFieldsSections.Where(p => p.DynamicFieldType >= (int)DynamicFieldType.TruckCrossPresent)
                                                     .ToList();

        DynamicFields = (await Api.PostAsyncByUri<List<DynamicFieldDto>>(
            "wms/Document",
            "SGetAllDynamicFields")).Value;

        DynamicFields = DynamicFields.Where(p => p.FieldType >= DynamicFieldType.TruckCrossPresent)
                                     .OrderBy(p => p.Id)
                                     .ToList();
    }
}
