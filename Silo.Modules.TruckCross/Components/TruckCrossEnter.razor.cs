using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Web;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Components;
public partial class TruckCrossEnter : IDisposable
{
    public bool IsEnterWeightLoading = false;
    public List<TruckCrossItemDto> TruckCrossEnterItems = new();
    public TruckCrossItemDto TruckCrossItemRequest = new();
    public string TruckCrossItemsModalTitle;
    public string TruckCrossItemsModalError;
    public List<GetAllAcceptPlacesVm> AcceptPlaces;

    [Inject] public IMapper Mapper { get; set; }

    [Parameter] public string Username { get; set; }
    [Parameter] public string UserId { get; set; }
    [Parameter] public List<GetTruckCrossItemsByTruckCrossIdVm> Items { get; set; }
    [Parameter][EditorRequired] public TruckCrossDataDto CrossRequest { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossProductTypeVm> TruckCrossProductTypes { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClearClick { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }
    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public Gallery Gallery { get; set; }
    [CascadingParameter] public SiloComponentsContext SiloContext { get; set; }
    [CascadingParameter] public TruckCrossComponentsContext TruckCrossContext { get; set; }

    public int ActiveEnterTabIndex { get; set; } = 0;
    public Modal ModalTruckCrossItems { get; set; }
    public TelerikGrid<TruckCrossItemDto> TruckCrossEnterItemsGrid { get; set; }


    protected override async Task OnInitializedAsync()
    {
        TruckCrossContext.TruckCrossDataHasChanged += LoadCross;

        SiloContext.NavbarTabChanged += OnTabStateChanged;

        IsLoading = false;
    }

    public async Task LoadCross(TruckCrossDataDto cross)
    {
        IsLoading = true;

        if (Items is not null)
        {
            TruckCrossEnterItems = Mapper.Map<List<TruckCrossItemDto>>(Items.Where(p => p.Type.Equals(1)).ToList());
        }

        AcceptPlaces = (await Api.PostAsyncByUri<List<GetAllAcceptPlacesVm>>("wms/truckcross", "SGetAllTruckCrossAcceptPlace")).Value;

        IsLoading = false;
    }

    public async Task OnActiveTabbChanged(int activeTab)
    {
        await LoadCross(CrossRequest);
    }

    public async Task OnGetEnterWeightClick()
    {
        IsEnterWeightLoading = true;

        var result = await GetLastWeighbridgeLog();

        CrossRequest.EnterWeightTonage = result.Weight.Value;

        IsEnterWeightLoading = false;
    }

    public async Task OnModalTruckCrossItemsClick()
    {
        TruckCrossItemRequest = new();

        TruckCrossItemsModalTitle = $"{TextResources.APP_StringKeys_View_Product_Add} - {TextResources.APP_StringKeys_TruckCross_Steps_Enter}";

        await ModalTruckCrossItems.Open(new());
    }

    public void OnTruckCrossItemRefreshClick()
    {
        TruckCrossItemRequest = new();

        TruckCrossItemsModalError = string.Empty;
    }

    public async Task OnItemsSaveClick(MouseEventArgs e)
    {
        IsLoading = true;

        List<TruckCrossItemDto> items = new();

        TruckCrossEnterItems.ForEach(p => p.Id = 0);

        items = TruckCrossEnterItems;

        var result = (await Api.PostAsyncByUriAndContext<List<GetTruckCrossItemsByTruckCrossIdVm>>("wms/TruckCross", "SSaveTruckCrossItem"
                            , new GetTruckCrossItemsByTruckCrossIdVmContext()
                            , new KeyValuePair<string, object>("items", items))).Value;

        IsLoading = false;

        if (result is not null)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            TruckCrossEnterItems = Mapper.Map<List<TruckCrossItemDto>>(result);

            TruckCrossEnterItemsGrid.Rebind();

            await ModalTruckCrossItems.Close(new());
        }
        else
        {
            TruckCrossItemsModalError = TextResources.APP_StringKeys_Alert_Fail;
        }
    }

    public async Task OnItemAddValidSubmit()
    {
        TruckCrossItemRequest.TruckCrossProductTypeTitle = TruckCrossProductTypes.FirstOrDefault(p => p.Id.Equals(TruckCrossItemRequest.TruckCrossProductTypeId))?.Title;

        if (TruckCrossItemRequest.Title.HasNoValue()) // Ghahari said!
        {
            TruckCrossItemRequest.Title = TruckCrossItemRequest.TruckCrossProductTypeTitle;
        }

        TruckCrossItemRequest.Type = (int)TruckCrossItemModes.Enter;

        TruckCrossItemRequest.TruckCrossId = CrossRequest.Id;

        if (TruckCrossItemRequest.Id.Equals(0))
        {
            TruckCrossEnterItems.Add(TruckCrossItemRequest);

            TruckCrossEnterItemsGrid.Rebind();
        }
        else
        {
            var listItem = TruckCrossEnterItems.FirstOrDefault(p => p.Id == TruckCrossItemRequest.Id);

            listItem = TruckCrossItemRequest;
        }


        TruckCrossItemRequest = new();
    }

    public async Task OnTruckCrossItemConfirmRemove(TruckCrossItemDto item)
    {
        if (item.Id == 0)
        {
            TruckCrossEnterItems.Remove(item);

            TruckCrossEnterItemsGrid.Rebind();

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsyncByUri<bool>("wms/TruckCross", "SDeleteTruckCrossItem"
                                , new KeyValuePair<string, object>("id", TruckCrossItemRequest.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");


            TruckCrossEnterItems.Remove(TruckCrossItemRequest);

            TruckCrossEnterItemsGrid.Rebind();

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

    public async Task OnOpenEnterGallery(MouseEventArgs e)
    {
        if (CrossRequest.Id.Equals(0))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(CrossRequest.NationalCode
            , GalleryUsageType.TruckCrossEnter
            , CrossRequest.Id.ToString());
    }

    public async Task OnEnterValidSubmit(EditContext context)
    {
        CrossRequest.EnterUserId = UserId;

        CrossRequest.EnterDateTime = DateTime.Now;

        if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Enter)
        {
            CrossRequest.TruckCrossStatus = TruckCrossStatuses.Enter;
        }

        await TruckCrossContext.SetSaveHasFired();

        int crossResult = (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCross"
            , new KeyValuePair<string, object>("cross", CrossRequest))).Value;

        if (crossResult != 0)
        {
            CrossRequest.EnterIsSaved = true;

            CrossRequest.EnterUsername = Username;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                , "success");
        }
        else
        {
            if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Enter)
            {
                CrossRequest.TruckCrossStatus = TruckCrossStatuses.Present;
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
    }
}
