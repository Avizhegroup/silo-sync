using Silo.Infrastructure.Web;

namespace Silo.Components.PlacementOrder;
public partial class PlacementOrderDetailsGridHierarchy
{
    public bool IsLoading = true;
    public List<GetAllPlacementOrderByProductCodeVm> PlacementOrderDetails = new();

    [EditorRequired][Parameter] public int OperationCode { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;

        PlacementOrderDetails = (await Api.PostAsync<List<GetAllPlacementOrderByProductCodeVm>>("SGetPlacementOrdersByOperationCode",
            new KeyValuePair<string, object>("operationCode", OperationCode))).Value;

        IsLoading = false;
    }
}
