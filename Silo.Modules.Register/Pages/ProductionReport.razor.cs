using Silo.Application.Dto;
using Silo.Application.Features;

namespace Silo.Modules.Register.Pages;

public partial class ProductionReport
{
    private readonly GetRegisterRequestFilter Filter = new()
    {
        ProductCode = "-1",
        ProductSerial = "-1",
        Line = "-1",
        FromDate = "-1",
        ToDate = "-1",
        User = "-1",
        Shift = "-1",
        Qc = "-1",
        TechnicalCode = "-1",
        ContractStatus = "-1",
        ProductGroup = "-1",
        ProductBrand = "-1",
        InspectStatus = "-1",
        RegisterDevice = "0"
    };
    private string FromDate = string.Empty;
    private string ToDate = string.Empty;
    private string Message = string.Empty;
    private bool IsLoading;
    private List<GetAllRegisterDetailsVm>? Rows;

    [Inject] public RfidConnectApi Api { get; set; } = default!;

    private async Task Search()
    {
        IsLoading = true;
        Message = string.Empty;
        Filter.ProductCode = string.IsNullOrWhiteSpace(Filter.ProductCode) ? "-1" : Filter.ProductCode.Trim();
        Filter.ProductSerial = string.IsNullOrWhiteSpace(Filter.ProductSerial) ? "-1" : Filter.ProductSerial.Trim();
        Filter.FromDate = string.IsNullOrWhiteSpace(FromDate) ? "-1" : FromDate.Replace("/", string.Empty, StringComparison.Ordinal);
        Filter.ToDate = string.IsNullOrWhiteSpace(ToDate) ? "-1" : ToDate.Replace("/", string.Empty, StringComparison.Ordinal);

        try
        {
            Rows = (await Api.PostAsyncByContext<List<GetAllRegisterDetailsVm>>(
                "SRepRegisterTagDetailsForWinApp", new GetAllRegisterDetailsVmContext(),
                new("filter", Filter))).Value ?? new();
        }
        catch
        {
            Rows = new();
            Message = TextResources.APP_StringKeys_Alert_Fail;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
