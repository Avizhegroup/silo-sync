using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Components;
public partial class TruckCrossPresent : IDisposable
{
    public bool IsPresentTurnLoading = false;
    private string PresentCauseLastOnChangeValue = string.Empty;
    public List<TruckCrossDataDto> ReviewPlaqueCrosses;
    public List<GetAllTruckCrossOperationTypesVm> OperationTypes;
    public List<GetAllTruckCrossProductTypeVm> TruckCrossProductTypes;

    public int ActivePresentTabIndex { get; set; } = 0;
    public Modal ModalReviewPlaque { get; set; }
    public Modal ModalRevoke { get; set; }

    [Parameter] public string Username { get; set; }
    [Parameter] public string UserId { get; set; }
    [Parameter][EditorRequired] public TruckCrossDataDto CrossRequest { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCompaniesVm> TruckCompanies { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckTypesVm> TruckTypes { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossPresentCauseVm> Causes { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossShipmentVm> Shipments { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossCustomerVm> Customers { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossOperationDestinationsVm> OperationDestinations { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClearClick { get; set; }
    [Parameter] public EventCallback OnRevokeSuccess { get; set; }
    [Parameter] public EventCallback<TruckCrossDataDto> OnReviewTruckCrossChoosen { get; set; }

    [CascadingParameter] public TelerikNotification Notification { get; set; }
    [CascadingParameter] public bool IsLoading { get; set; }
    [CascadingParameter] public RfidConnectApi Api { get; set; }
    [CascadingParameter] public Gallery Gallery { get; set; }
    [CascadingParameter] SiloComponentsContext SiloContext { get; set; }
    [CascadingParameter] TruckCrossComponentsContext TruckCrossContext { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
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

        if (CrossRequest.PresentCause is not null)
        {
            OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetTruckCrossOperationTypesByCause",
                            new KeyValuePair<string, object>("presentCauseId", CrossRequest.PresentCause))).Value;
        }

        IsLoading = false;
    }

    public async Task OnPresentTurnClick(MouseEventArgs e)
    {
        IsPresentTurnLoading = true;

        CrossRequest.PresentTurn = (await Api.PostAsyncByUri<int>("wms/TruckCross", "SGetNextTruckCrossTurn")).Value;

        IsPresentTurnLoading = false;
    }

    public async Task OnSearchNationalCode(MouseEventArgs e)
    {
        if (CrossRequest.NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Empty, "error");

            return;
        }

        IsLoading = true;

        ReviewPlaqueCrosses =
            (await Api.PostAsyncByUri<List<TruckCrossDataDto>>("wms/TruckCross", "SGetTruckCrossByNc"
            , new KeyValuePair<string, object>("nationalCode", CrossRequest.NationalCode)))
            .Value;

        if (ReviewPlaqueCrosses.Select(p => p.Plaque).Distinct().Count() > 1)
        {
            await ModalReviewPlaque.Open(new());
        }
        else if (ReviewPlaqueCrosses.Any())
        {
            await OnReviewTruckCrossChoosen.InvokeAsync(ReviewPlaqueCrosses.First());
        }

        IsLoading = false;
    }

    public async Task OnClearButtonClick(MouseEventArgs e)
    {
        await OnClearClick.InvokeAsync(e);
    }

    public async Task OnRevokeTruckCrossModalOpen(MouseEventArgs e)
    {
        if (CrossRequest.TruckCrossStatus != TruckCrossStatuses.Present)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Revoke_TruckCross_Level, "error");

            return;
        }

        await ModalRevoke.Open(e);
    }

    public async Task OnRevokeConfirmClick(MouseEventArgs e)
    {
        IsLoading = true;

        bool result = (await Api.PostAsyncByUri<bool>("wms/TruckCross"
            , "SRevokePresentTruckCross"
            , new KeyValuePair<string, object>("id", CrossRequest.Id))).Value;

        if (result)
        {
            CrossRequest.PresentRevokeUserId = UserId;
            CrossRequest.PresentRevokeUsername = Username;
            CrossRequest.PresentRevokeDateTime = DateTime.Now;
            CrossRequest.TruckCrossStatus = TruckCrossStatuses.Revoke;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            await OnRevokeSuccess.InvokeAsync();
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "error");
        }

        IsLoading = false;
    }

    public async Task OnChooseReviewTruckCross(TruckCrossDataDto cross)
    {
        await ModalReviewPlaque.Close(new());

        await OnReviewTruckCrossChoosen.InvokeAsync(cross);
    }

    public async Task OnOpenPresentGallery(MouseEventArgs e)
    {
        if (CrossRequest.Id.Equals(0))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(CrossRequest.NationalCode
            , GalleryUsageType.TruckCrossPresent
            , CrossRequest.Id.ToString()
            , GalleryOcrTypes.Plaque);
    }

    public async Task OnOpenDriverGallery(MouseEventArgs e)
    {
        if (CrossRequest.NationalCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await Gallery.Show(CrossRequest.NationalCode
            , GalleryUsageType.TruckCrossDriver);
    }

    public async Task OnPresentCauseChange(object value)
    {
        if (value is null)
        {
            return;
        }

        var currentValue = value.ToString();

        if (PresentCauseLastOnChangeValue.NotEquals(currentValue) && CrossRequest.PresentCause > 0)
        {
            IsLoading = true;

            OperationTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossOperationTypesVm>>("wms/TruckCross", "SGetTruckCrossOperationTypesByCause",
                new KeyValuePair<string, object>("presentCauseId", CrossRequest.PresentCause))).Value;

            TruckCrossProductTypes = (await Api.PostAsyncByUri<List<GetAllTruckCrossProductTypeVm>>("wms/TruckCross", "SGetTruckCrossProductTypesByCause",
                new KeyValuePair<string, object>("presentCauseId", CrossRequest.PresentCause))).Value;

            PresentCauseLastOnChangeValue = currentValue;

            IsLoading = false;
        }
    }

    public async Task OnPresentValidSubmit(EditContext context)
    {
        CrossRequest.PresentUserId = UserId;

        CrossRequest.PresentDateTime = DateTime.Now;

        if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Present)
        {
            CrossRequest.TruckCrossStatus = TruckCrossStatuses.Present;
        }

        CrossRequest.Plaque = $"{CrossRequest.FirstPart}{CrossRequest.Character}{CrossRequest.SecondPart}{CrossRequest.CityPart}";

        await TruckCrossContext.SetSaveHasFired();

        int crossId = (await Api.PostAsyncByUri<int>("wms/TruckCross", "SSaveTruckCross"
            , new KeyValuePair<string, object>("cross", CrossRequest))).Value;

        if (crossId != 0)
        {
            CrossRequest.PresentIsSaved = true;

            if (CrossRequest.Id == 0)
            {
                CrossRequest.Id = crossId;
            }
            CrossRequest.PresentUsername = Username;

            Notification.Show(TextResources.APP_StringKeys_Alert_Success
                , "success");
        }
        else
        {
            if (CrossRequest.TruckCrossStatus <= TruckCrossStatuses.Present)
            {
                CrossRequest.TruckCrossStatus = 0;
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

    public void OnTabRender(object e)
    {
        JSRuntime.InvokeVoidAsync("removeAttr", ".text-dir-left .k-input-inner", "dir").GetAwaiter();
    }

    private void OnTabStateChanged(bool isExpanded)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        TruckCrossContext.TruckCrossDataHasChanged -= LoadCross;
        SiloContext.NavbarTabChanged += OnTabStateChanged;
    }
}
