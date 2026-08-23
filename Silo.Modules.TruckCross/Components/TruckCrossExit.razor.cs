using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Silo.Application;
using Silo.Application.Dto;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Components;
public partial class TruckCrossExit : IDisposable
{
    public bool IsExitWeightLoading = false;
    public bool IsExitPriceLoading = false;
    public List<TruckCrossItemDto> TruckCrossExitItems = new();
    public TruckCrossItemDto TruckCrossItemRequest = new();
    public string TruckCrossItemsModalTitle;
    public string TruckCrossItemsModalError;
    public List<TelerikDropDownItemGeneric<int>> PaymentTypes;
    public string CompanyName = string.Empty;
    public GetAllTruckCrossPresentCauseVm PresentCause;

    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IHostingEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }

    [Parameter] public string Username { get; set; }
    [Parameter] public string UserId { get; set; }
    [Parameter] public List<GetTruckCrossItemsByTruckCrossIdVm> Items { get; set; } = new();
    [Parameter][EditorRequired] public TruckCrossDataDto CrossRequest { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossPresentCauseVm> Causes { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossProductTypeVm> TruckCrossProductTypes { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }
    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public Gallery Gallery { get; set; }
    [CascadingParameter] public SiloComponentsContext SiloContext { get; set; }
    [CascadingParameter] public string PageTitle { get; set; }
    [CascadingParameter] TruckCrossComponentsContext TruckCrossContext { get; set; }

    public Modal ModalTruckCrossItems { get; set; }
    public SecurityGate SecurityGateComponent { get; set; }
    public int ActiveExitTabIndex { get; set; } = 0;
    public TelerikGrid<TruckCrossItemDto> TruckCrossExitItemsGrid { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PaymentTypes = new()
        {
            new ()
            {
                Value = 1,
                Name = TextResources.APP_StringKeys_Payment_BySender
            },
            new ()
            {
                Value = 2,
                Name = TextResources.APP_StringKeys_Payment_ByReciever
            },
            new ()
            {
                Value = 3,
                Name = TextResources.APP_StringKeys_Payment_ByCompany
            }
        };

        TruckCrossContext.TruckCrossDataHasChanged += LoadCross;

        SiloContext.NavbarTabChanged += OnTabStateChanged;

        IsLoading = false;
    }

    public async Task OnActiveTabbChanged(int activeTab)
    {
        await LoadCross(CrossRequest);
    }

    public async Task LoadCross(TruckCrossDataDto cross)
    {
        IsLoading = true;

        if (Items is not null)
        {
            TruckCrossExitItems = Mapper.Map<List<TruckCrossItemDto>>(Items.Where(p => p.Type.Equals(2)).ToList());
        }

        if (cross.PresentCause is null)
        {
            return;
        }

        PresentCause = Causes.FirstOrDefault(p => p.Id == cross.PresentCause);

        IsLoading = false;
    }

    public async Task OnExitValidSubmit(EditContext context)
    {
        if ((PresentCause is not null && PresentCause.ActionTypeId is not null)
            && CrossRequest.MovementActionId.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Update_Uhf_Statuses, "error");

            return;
        }

        CrossRequest.ExitUserId = UserId;

        CrossRequest.ExitDateTime = DateTime.Now;

        if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Exit)
        {
            CrossRequest.TruckCrossStatus = TruckCrossStatuses.Exit;
        }

        await TruckCrossContext.SetSaveHasFired();

        int crossResult = (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCross"
            , new KeyValuePair<string, object>("cross", CrossRequest))).Value;

        if (crossResult != 0)
        {
            CrossRequest.ExitIsSaved = true;

            CrossRequest.ExitUsername = Username;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                , "success");
        }
        else
        {
            if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Exit)
            {
                CrossRequest.TruckCrossStatus = TruckCrossStatuses.Enter;
            }

            Notification.Show(TextResources.APP_StringKeys_Alert_Fail
                , "error");
        }
    }

    public async Task OnInvalidSubmit(EditContext context)
    {
        {
            foreach (string validation in context.GetValidationMessages())
            {
                {
                    Notification.Show(validation, "error");
                }
            }
        }
    }

    public async Task OnGetExitWeightClick()
    {
        IsExitWeightLoading = true;

        var result = await GetLastWeighbridgeLog();

        CrossRequest.ExitWeightTonage = result.Weight.Value;

        CalculatePureWeight();

        IsExitWeightLoading = false;

        void CalculatePureWeight()
        {
            decimal diff = CrossRequest.ExitWeightTonage - CrossRequest.EnterWeightTonage;

            if (diff < 0)
            {
                CrossRequest.ExitPureWeightCargo = 0;
            }
            else
            {
                CrossRequest.ExitPureWeightCargo = diff;
            }
        }
    }

    public async Task OnModalTruckCrossItemsClick()
    {
        TruckCrossItemRequest = new();

        TruckCrossItemsModalTitle = TextResources.APP_StringKeys_View_Product_Add + " - " +
                                                                          TextResources.APP_StringKeys_TruckCross_Steps_Exit;

        await ModalTruckCrossItems.Open(new());
    }

    public async Task OnTruckCrossItemConfirmRemove(TruckCrossItemDto item)
    {
        if (item.Id == 0)
        {
            TruckCrossExitItems.Remove(item);

            TruckCrossExitItemsGrid.Rebind();

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossItem"
                                , new KeyValuePair<string, object>("id", TruckCrossItemRequest.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");


            TruckCrossExitItems.Remove(TruckCrossItemRequest);

            TruckCrossExitItemsGrid.Rebind();

            TruckCrossItemRequest = new();

            TruckCrossItemsModalError = string.Empty;
        }
        else
        {
            TruckCrossItemsModalError = TextResources.APP_StringKeys_Alert_Fail;
        }

        IsLoading = false;
    }

    public void OnTruckCrossItemChoose(TruckCrossItemDto item)
    {
        TruckCrossItemRequest = item;

        TruckCrossItemsModalError = string.Empty;
    }

    public async Task OnItemAddValidSubmit()
    {
        TruckCrossItemRequest.TruckCrossProductTypeTitle = TruckCrossProductTypes.FirstOrDefault(p => p.Id.Equals(TruckCrossItemRequest.TruckCrossProductTypeId))?.Title;

        if (TruckCrossItemRequest.Title.HasNoValue()) // Ghahari said!
        {
            TruckCrossItemRequest.Title = TruckCrossItemRequest.TruckCrossProductTypeTitle;
        }

        TruckCrossItemRequest.Type = (int)TruckCrossItemModes.Exit;

        TruckCrossItemRequest.TruckCrossId = CrossRequest.Id;

        if (TruckCrossItemRequest.Id.Equals(0))
        {
            TruckCrossExitItems.Add(TruckCrossItemRequest);

            TruckCrossExitItemsGrid.Rebind();
        }
        else
        {
            var listItem = TruckCrossExitItems.FirstOrDefault(p => p.Id == TruckCrossItemRequest.Id);

            listItem = TruckCrossItemRequest;
        }

        TruckCrossItemRequest = new();
    }

    public async Task OnItemsSaveClick(MouseEventArgs e)
    {
        IsLoading = true;

        List<TruckCrossItemDto> items = new();


        TruckCrossExitItems.ForEach(p => p.Id = 0);

        items = TruckCrossExitItems;

        var result = (await Api.PostAsyncByUriAndContext<List<GetTruckCrossItemsByTruckCrossIdVm>>("wms/TruckCross", "SSaveTruckCrossItem"
                            , new GetTruckCrossItemsByTruckCrossIdVmContext()
                            , new KeyValuePair<string, object>("items", items))).Value;

        IsLoading = false;

        if (result is not null)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");


            TruckCrossExitItems = Mapper.Map<List<TruckCrossItemDto>>(result);

            TruckCrossExitItemsGrid.Rebind();

            await ModalTruckCrossItems.Close(new());
        }
        else
        {
            TruckCrossItemsModalError = TextResources.APP_StringKeys_Alert_Fail;
        }
    }

    public void OnTruckCrossItemRefreshClick()
    {
        TruckCrossItemRequest = new();

        TruckCrossItemsModalError = string.Empty;
    }

    public void OnGetExitPureWeightCargoClick()
    {
        TruckCrossItemRequest.ProductCount = CrossRequest.ExitPureWeightCargo;
    }

    public async Task OnOpenExitGallery(MouseEventArgs e)
    {
        if (CrossRequest.Id.Equals(0))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(CrossRequest.NationalCode
            , GalleryUsageType.TruckCrossExit
            , CrossRequest.Id.ToString());
    }

    public async Task OnExitWeightChange(object e)
    {
        decimal value = (decimal)e;

        decimal diff = value - CrossRequest.EnterWeightTonage;

        if (diff < 0)
        {
            CrossRequest.ExitPureWeightCargo = 0;
        }
        else
        {
            CrossRequest.ExitPureWeightCargo = diff;
        }
    }

    public async Task OnPrintClick(MouseEventArgs e)
    {
        if (!CrossRequest.ExitIsSaved)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_TruckCross_SaveExit, "error");

            return;
        }

        List<GateProductPrintableVm> printableProducts = new();

        TruckCrossHeaderPrintDto printableTruckCrossHeader = new();

        IsLoading = true;

        printableProducts = (await Api.PostAsyncByContext<List<GateProductPrintableVm>>("SGetGateProductsByTruckCrossId"
            , new GateProductPrintableVmContext()
            , new KeyValuePair<string, object>("truckCrossId", CrossRequest.Id))).Value;

        var result = (await Api.PostAsyncByUri<List<TruckCrossHeaderPrintDto>>("wms/TruckCross", "SGetPrintableTruckCrossData"
            , new KeyValuePair<string, object>("truckCrossId", CrossRequest.Id))).Value;

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

        string path = Path.Combine(Environment.WebRootPath, "images", "Icons", "company.png");

        List<KeyValuePair<string, string>> images = new()
        {
            new("Image_Logo", path)
        };

        if (CompanyName.HasNoValue())
        {
            CompanyName = Configuration.GetSection("Settings")["Company"];
        }

        List<KeyValuePair<string, object>> variables = new()
        {
            new("CompanyName", CompanyName),
            new("PageTitle", PageTitle)
        };

        List<KeyValuePair<string, object>> dataSources = new()
        {
            new(nameof(GateProductPrintableVm), printableProducts),
            new(nameof(TruckCrossHeaderPrintDto), printableTruckCrossHeader)
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

        await Export.ExportAndDownloadUsingBypass(response.Value.Result);

        IsLoading = false;
    }

    public async Task OnCalculatePriceClick(MouseEventArgs e)
    {
        IsExitPriceLoading = true;

        var result = (await Api.PostAsyncByUri<GetTruckCrossPriceExitVm>("wms/TruckCross"
                    , "SCalculatePriceExit"
                    , new KeyValuePair<string, object>("cross", CrossRequest))).Value;

        CrossRequest.ExitTotalCost = result.FinalPrice.ToString();

        CrossRequest.ExitUnitPrice = result.Fee.ToString();

        IsExitPriceLoading = false;
    }

    private void OnTabStateChanged(bool isExpanded)
    {
        StateHasChanged();
    }

    private async Task<GetLastWeighbridgeLogVm> GetLastWeighbridgeLog()
    {
        return (await Api.PostAsyncByUriAndContext<GetLastWeighbridgeLogVm>("wms/TruckCross"
                                                                          , "SGetLastWeighbridgeLog"
                                                                          , new GetLastWeighbridgeLogVmContext())).Value;
    }

    void IDisposable.Dispose()
    {
        TruckCrossContext.TruckCrossDataHasChanged -= LoadCross;
        SiloContext.NavbarTabChanged -= OnTabStateChanged;
    }
}
