using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Silo.Shared.Components;

namespace Silo.Modules.TruckCross.Components;
public partial class SecurityGate : IDisposable
{
    public SecurityPageModes Mode;
    public bool IsLoading = false;
    public string PageTitle = string.Empty;
    public List<GetPlaceByTruckCrossIdDto> Products;
    public List<GetPlaceProductDetailsByCrossIdDto> ProductDetails;
    public GetExitActionByUhfIdVm Operation = new();
    public List<GetAllStationsVm> Stations;
    public GetAllTruckCrossPresentCauseVm PresentCause;
    public string Url = string.Empty;

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    [Parameter][EditorRequired] public TruckCrossDataDto CrossRequest { get; set; }
    [Parameter][EditorRequired] public List<GetAllTruckCrossPresentCauseVm> Causes { get; set; }

    [CascadingParameter] TelerikNotification Notification { get; set; }
    [CascadingParameter] TruckCrossComponentsContext TruckCrossContext { get; set; }

    public Modal ModalDetails { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = TextResources.APP_StringKeys_View_Security_Index_Doc_required;

        Mode = SecurityPageModes.GateAndDoc;

        TruckCrossContext.TruckCrossDataHasChanged += LoadCross;

        await LoadCross(CrossRequest);
    }

    public async Task LoadCross(TruckCrossDataDto cross)
    {
        IsLoading = true;

        if (CrossRequest.PresentCause is not null)
        {
            PresentCause = Causes.FirstOrDefault(p => p.Id == CrossRequest.PresentCause);

            var result = (await Api.PostAsyncByContext<List<GetAllStationsVm>>("SGetAllStations"
           , new GetAllStationsVmContext())).Value;

            Stations = result.Where(p => p.Type == StationTypeEnum.Gate
            && p.StationActionType == PresentCause.ActionTypeId.ToString()).ToList();

            await OnGetMovementActionDataClick(new());
        }

        RebuildUrl();

        IsLoading = false;
    }

    public async Task OnProductDetailsClick(GetPlaceByTruckCrossIdDto product)
    {
        IsLoading = true;

        ProductDetails = (await Api.SendAsyncObjectByUri<GetPlaceProductDetailsByCrossIdVm>(HttpMethod.Get
                , "Crosses/GetPlaceProductDetailsByCrossId"
                , new GetPlaceProductDetailsByCrossIdQuery()
                {
                    TruckCrossId = CrossRequest.Id,
                    ProductCode = product.ProductCode,
                    DocumentCode = product.DocumentCode
                })).Value.List;

        await ModalDetails.Open(new());

        IsLoading = false;
    }

    public async Task OnGetMovementActionDataClick(MouseEventArgs e)
    {
        IsLoading = true;

        var res = await Api.SendAsyncObjectByUri<GetPlaceByTruckCrossIdVm>(HttpMethod.Get, "Crosses/GetPlaceByTruckCrossId"
                , new GetPlaceByTruckCrossIdQuery()
                {
                    TruckCrossId = CrossRequest.Id
                });

        
        if (res.Value.PlaceProducts is not null)
        {
            Products = res.Value.PlaceProducts;

            CrossRequest.MovementActionId = res.Value.ActionIds.FirstOrDefault(0).ToString();
        }
        else
        {
            Products = null;
        }

        IsLoading = false;
    }

    public void RebuildUrl()
    {
        var presentCauseActionType = PresentCause is not null ? PresentCause.ActionTypeId.ToString() : "";
       
        Url = $"/location/place/{(int)Mode}/{Operation.Gate}/{presentCauseActionType}/{CrossRequest.Id}";
    }

    public void Dispose()
    {
        TruckCrossContext.TruckCrossDataHasChanged -= LoadCross;
    }
}
