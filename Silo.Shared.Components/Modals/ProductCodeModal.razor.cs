using DocumentFormat.OpenXml.Spreadsheet;
using Silo.Application.Features;
using Telerik.Blazor.Components;

namespace Silo.Shared.Components.Modals;
public partial class ProductCodeModal
{
    public bool IsLoading = true;
    public string ProductTitle = string.Empty;
    public string ProductCode = string.Empty;
    public string RegCode = string.Empty;
    public string Size = string.Empty;
    public string Quality = string.Empty;
    public List<PositionProductResponse> Products = new();
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductQcsVm> Qcs;

    [Parameter] public EventCallback<string> OnClickProductCode { get; set; }
    [Parameter] public EventCallback<string> OnClickProductTitle { get; set; }
    [Parameter] public EventCallback<PositionProductResponse> OnClickProduct { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }

    public Modal Modal { get; set; }
    public TelerikGrid<PositionProductResponse> GridProducts { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Qcs = (await Api.PostAsync<List<GetAllProductQcsVm>>("SGetAllQcs")).Value;

        Sizes = (await Api.PostAsync<List<GetAllProductSizeTitleAndCodeVm>>("SGetAllProductSize"
            , new("userToken", "Modal product code")
            , new("haveNotSelect", false))).Value;

        IsLoading = false;
    }

    public async Task OnSearchClick(MouseEventArgs e)
    {
        IsLoading = true;

        PositionProductRequest request = FixEmptiness();
        Products = (await Api.PostAsync<List<PositionProductResponse>>("SPSearchProductWeb",
            new KeyValuePair<string, object>[] { new("search", request) })).Value;

        IsLoading = false;

        GridProducts.Data = Products;

        GridProducts.Rebind();

        PositionProductRequest FixEmptiness()
        {
            PositionProductRequest request = new();

            if (string.IsNullOrEmpty(ProductCode))
            {
                request.MProductCode = "-1";
            }
            else
            {
                request.MProductCode = ProductCode;
            }

            if (string.IsNullOrEmpty(ProductTitle))
            {
                request.MProductTitle = "-1";
            }
            else
            {
                request.MProductTitle = ProductTitle;
            }

            if (string.IsNullOrEmpty(Quality))
            {
                request.MQuality = "-1";
            }
            else
            {
                request.MQuality = Quality;
            }

            if (string.IsNullOrEmpty(Size))
            {
                request.MSize = "-1";
            }
            else
            {
                request.MSize = Size;
            }

            if (string.IsNullOrEmpty(RegCode))
            {
                request.MTechCode = "-1";
            }
            else
            {
                request.MTechCode = RegCode;
            }

            return request;
        }
    }

    public async Task OnChooseProductCode(string productCode, string productTitle,PositionProductResponse product)
    {
        await OnClickProductCode.InvokeAsync(productCode);

        await OnClickProductTitle.InvokeAsync(productTitle);

        await OnClickProduct.InvokeAsync(product);

        await Modal.Close(new());
    }

    public async Task Show()
    {
        ProductTitle = string.Empty;
        ProductCode = string.Empty;
        RegCode = string.Empty;
        Size = string.Empty;
        Quality = string.Empty;
        Products = null;
        await Modal.Open(new());
    }

    public async Task Show(string productCode)
    {
        ProductTitle = string.Empty;
        ProductCode = productCode;
        RegCode = string.Empty;
        Size = string.Empty;
        Quality = string.Empty;
        Products = null;
        await Modal.Open(new());

        if (productCode.HasValue())
        {
            await OnSearchClick(new());
        }
    }
}

public class PositionProductRequest
{
    public string MProductTitle { get; set; }
    public string MProductCode { get; set; }
    public string MTechCode { get; set; }
    public string MSize { get; set; }
    public string MQuality { get; set; }
}

public class PositionProductResponse
{
    public string ProductName { get; set; }
    public string ProductCode { get; set; }
    public string TechnicalCode { get; set; }
    public string ProductSize { get; set; }
    public string ProductENTitle { get; set; }
    public string ProductPackValue { get; set; }
    public string ProductPackWeight { get; set; }
    public string ProductPackVolume { get; set; }
    public string ProductCountInPack { get; set; }
    public string ProductValue { get; set; }
    public string ProductProperties { get; set; }
    public string ProductType { get; set; }
    public string ProductUnit { get; set; }
    public string ProductStatus { get; set; }
    public string ProductStatusTitle { get; set; }
    public string ProductTypeTitle { get; set; }
    public string DocumentId { get; set; }
   public string ProductTechnicalData { get; set; }

}

public class Size
{
    public string Code { get; set; }
    public string Title { get; set; }
}
