using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Silo.Application;
using Silo.Shared.Components;
using Silo.Shared.Components.Modals;
using Silo.Shared.Tools;

namespace Silo.Modules.Product.Pages;
public partial class AddProduct
{
    public bool IsLoading = true;
    public SaveProductCommand Product = new();
    public List<GetAllProductTypeVm> Types;
    public List<GetAllProductQcsVm> Qcs;
    public List<GetAllProductSizeTitleAndCodeVm> Sizes;
    public List<GetAllProductVm> Products;
    public List<GetCusProductBySearchTextVm> CusProducts;
    public List<ExcelProductDto> ExcelProducts = new();
    public GetAllProductQuery SearchRequest = new();
    public string SearchAccounting = string.Empty;
    public string ResultString = string.Empty;
    public string UserToken = string.Empty;
    public List<GetAllProductBrandVm> ProductBrands;
    public List<GetAllProductGroupVm> ProductGroups;
    public List<GetAllProductClassVm> ProductClasses;
    public List<GetAllProductSubGroupVm> ProductSubGroups;
    public List<GetAllProductSubGroupVm> FilteredProductSubGroups;
    public List<TechnicalDataItemDto> TechnicalDataItems = new();
    public List<string> ProductRequiredFields = new();
    public SaveProductCommandEnabilityCheck EnabilityChecker = new();
    public string ImageBase64;

    public Gallery Gallery { get; set; }
    public Modal ModalProducts { get; set; }
    public Modal ModalCusProducts { get; set; }
    public Modal ModalRemove { get; set; }
    public Modal ModalExcel { get; set; }
    public ProductCodeModal? ProductCodeModalRef;
    public TelerikGrid<ExcelProductDto> ExcelProductGridRef { get; set; }


    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }
    [Inject] public IConfiguration Configuration { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        EnabilityChecker = (await Api.PostAsync<SaveProductCommandEnabilityCheck?>("SGetSettingsAddProduct")).Value;

        UserToken = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        Types = await FormalCache.GetTypes();

        Qcs = await FormalCache.GetQcs();

        Sizes = await FormalCache.GetSizes();

        ProductBrands = await FormalCache.GetBrands();

        ProductGroups = await FormalCache.GetGroups();

        ProductClasses = await FormalCache.GetProductClass();

        ProductSubGroups = await FormalCache.GetSubGroups();

        ProductRequiredFields = (await Api.PostAsyncByUri<List<string>>("wms/Product", "SGetProductCreationRequiredFields")).Value;

        IsLoading = false;
    }

    public async Task OnSelectProduct(GetAllProductVm product)
    {
        IsLoading = true;

        Product = Mapper.Map<SaveProductCommand>(product);

        if (Product.ProductGalleryId.NotEquals(0))
        {
            ImageBase64 = await SetImage(new GetGalleryImageFileQuery()
            {
                Id = Product.ProductGalleryId,
                UsageType = GalleryUsageType.Product
            });
        }
        else
        {
            ImageBase64 = null;
        }

        FillTechnicalItemsFromJson(Product.ProductTechnicalData);

        FilteredProductSubGroups = ProductSubGroups.Where(p => p.ProductGroupCode == Product.ProductGroup.ToString()).ToList();

        await ModalProducts.Close(new());

        IsLoading = false;
    }

    public void OnRemoveExcelZoneClick(ExcelProductDto product)
    {
        ExcelProducts.Remove(product);

        ExcelProductGridRef.Rebind();

        StateHasChanged();
    }

    public async Task OnSelectProductAccounting(GetCusProductBySearchTextVm product)
    {
        IsLoading = true;

        Product = Mapper.Map<SaveProductCommand>(product);

        if (Product.ProductGalleryId.NotEquals(0))
        {
            ImageBase64 = await SetImage(new()
            {
                Id = Product.ProductGalleryId,
                UsageType = GalleryUsageType.Product
            });
        }
        else
        {
            ImageBase64 = null;
        }

        await ModalCusProducts.Close(new());

        IsLoading = false;
    }

    public async Task OnSearchProduct(MouseEventArgs e)
    {
        IsLoading = true;

        GetAllProductQuery request = FixEmptinessSearchProducts();

        Products = (await Api.PostAsync<List<GetAllProductVm>>("SPSearchProductWeb",
            new KeyValuePair<string, object>[] { new("search", request) })).Value;

        IsLoading = false;
    }

    public async Task OnRefreshSearchProduct(MouseEventArgs e)
    {
        SearchRequest = new();

        Products = null;
    }

    public async Task OnSearchAccounting(MouseEventArgs e)
    {
        IsLoading = true;

        CusProducts = (await Api.PostAsync<List<GetCusProductBySearchTextVm>>("SGetCusProductsForWeb",
            new KeyValuePair<string, object>[] { new("SearchText", SearchAccounting) })).Value;

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        Product = new();

        ImageBase64 = null;

        Products = null;

        SearchRequest = new();

        CusProducts = null;

        SearchAccounting = string.Empty;

        TechnicalDataItems = new List<TechnicalDataItemDto>();

        FilteredProductSubGroups = new();
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        bool result = await CheckProductEmptiness();

        if (!result)
        {
            await ModalRemove.Open(new());
        }
    }

    public async Task OnSubmitClick(EditContext editContext)
    {
        foreach (var requiredField in ProductRequiredFields)
        {
            switch (requiredField)
            {
                case "ProductCode":
                    if (Product.ProductCode.HasNoValue())
                    {
                        Notification.Show(
                            string.Format(TextResources.APP_StringKeys_Validation_Required,
                                          TextResources.APP_StringKeys_ProductCode),
                            "error");
                        return;
                    }
                    break;

                case "ProductTitle":
                    if (Product.ProductTitle.HasNoValue())
                    {
                        Notification.Show(
                            string.Format(TextResources.APP_StringKeys_Validation_Required,
                                          TextResources.APP_StringKeys_ProductTitle),
                            "error");
                        return;
                    }
                    if (Product.ProductENTitle.HasNoValue())
                    {
                        Notification.Show(
                            string.Format(TextResources.APP_StringKeys_Validation_Required,
                                          TextResources.APP_StringKeys_ProductENTitle),
                            "error");
                        return;
                    }
                    break;

                default:
                    break;
            }
        }

        IsLoading = true;

        FixEmptinessSaveProduct();

        PrepareTechnicalDataForSave();

        bool result = await SaveProducts(new List<SaveProductCommand>()
        {
            Product
        });

        if (result)
        {
            if (Product.ProductId == 0)
            {
                Product.ProductId = int.Parse((await Api.PostAsync<string>("GetLatestIdOfIdentityTable"
                , new KeyValuePair<string, object>("tableName", "tbl_Products"))).Value);
            }
        }

        IsLoading = false;
    }

    public async Task OnQcBatchSubmitClick(MouseEventArgs e)
    {
        IsLoading = true;

        if (Product.ProductId == 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            IsLoading = false;

            return;
        }

        if (string.IsNullOrEmpty(Product.ProductStatus))
        {
            Notification.Show(TextResources.APP_StringKeys_Qc_Validation_Choose, "error");

            IsLoading = false;

            return;
        }

        FixEmptinessSaveProduct();

        var result = (await Api.PostAsync<int>("SSaveProductByQc",
            new KeyValuePair<string, object>[] { new("product", Product) })).Value;

        if (result == 1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else if (result == -2)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Product_ApiUnique, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnCompleteUpload(GalleryFileUploadedDto galleryFileUploaded)
    {
        Product.ProductGalleryId = galleryFileUploaded.Id;

        ImageBase64 = galleryFileUploaded.Base64Image;
        
        IsLoading = false;
    }

    public async Task OnCompleteUploadExcelAdd(string path)
    {
        var data = DataTableTools.ReadExcelDataExportInDataTable(path);

        ExcelProducts = new();

        int index = 0;

        foreach (DataRow row in data.Tables[0].Rows)
        {
            if (index == 0)
            {
                index++;

                continue;
            }

            if (row.ItemArray.Length < 14)
            {
                await ModalExcel.Close(new());

                Notification.Show(TextResources.APP_StringKeys_Validation_Excel_Format
                    , "error");

                IsLoading = false;

                return;
            }

            if (string.IsNullOrEmpty(row.ItemArray[0].ToString()))
            {
                continue;
            }

            ExcelProducts.Add(new()
            {
                ProductCode = row.ItemArray[0].ToString(),
                ProductTitle = row.ItemArray[1].ToString(),
                ProductENTitle = row.ItemArray[2].ToString(),
                ProductTechnicalCode = row.ItemArray[3].ToString(),
                ProductType = Types.FirstOrDefault(p => p.Title.Equals(row.ItemArray[4].ToString().Trim()) || p.Code.Equals(row.ItemArray[4].ToString().Trim()))?.Code,
                ProductSize = Sizes.FirstOrDefault(p => p.Title.Equals(row.ItemArray[5].ToString().Trim()) || p.Code.Equals(row.ItemArray[5].ToString().Trim()))?.Code,
                ProductStatus = Qcs.FirstOrDefault(p => p.Title.Equals(row.ItemArray[6].ToString().Trim()) || p.Code.Equals(row.ItemArray[6].ToString().Trim()))?.Code,
                ProductUnit = row.ItemArray[7].ToString(),
                ProductGroup = ProductGroups.FirstOrDefault(p => p.Title.Equals(row.ItemArray[8].ToString().Trim()) || p.Code.Equals(row.ItemArray[8].ToString().Trim()))?.Code,
                ProductBrand = ProductBrands.FirstOrDefault(p => p.Title.Equals(row.ItemArray[9].ToString().Trim()) || p.Code.Equals(row.ItemArray[9].ToString().Trim()))?.Code,
                ProductPackValue = row.ItemArray[10].ToString(),
                ProductValue = row.ItemArray[11].ToString(),
                ProductCountInPack = row.ItemArray[12].ToString(),
                ProductPackWeight = row.ItemArray[13].ToString(),
                ProductPackVolume = row.ItemArray[14].ToString(),
                ProductProperties = "",
                ProductGalleryId = 0,
                ProductRegUser = UserToken
            });
        }

        IsLoading = false;
    }

    public async Task OnCompleteUploadExcelTechnicalData(string path)
    {
        var result = await Api.PostFileAsync<bool>("InputExcelFile", path
                , new("type", "techdata")
                , new("userToken", UserToken));

        if (Product.ProductCode.HasValue())
        {
            await ReloadCurrentProduct();
        }

        ResultString = TextResources.APP_StringKeys_Alert_Success;

        if (!(result.Successful || result.Value))
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;

        async Task ReloadCurrentProduct()
        {
            GetAllProductQuery request = FixEmptinessSearchProducts("-1");

            request.MProductCode = Product.ProductCode;

            List<GetAllProductVm> searchList = (await Api.PostAsync<List<GetAllProductVm>>("SPSearchProductWeb",
                new KeyValuePair<string, object>[] { new("search", request) })).Value;

            if (searchList.Any())
            {
                GetAllProductVm positionProduct = searchList.First();

                await OnSelectProduct(positionProduct);
            }
        }
    }

    public async Task OnChangeSize(object value)
    {
        if (IsLoading)
        {
            return;
        }

        if (value is null)
        {
            return;
        }

        if (value is string castedValue)
        {
            if (castedValue.HasNoValue())
            {
                return;
            }
        }

        var size = Sizes.FirstOrDefault(p => p.Code == value.ToString());

        if (size is not null)
        {
            if (size.Data.HasValue())
            {
                var data = JsonSerializer.Deserialize<GetProductSizeDataVm>(size.Data);

                Product.ProductValue = decimal.Parse(data.ProductValue);
                Product.ProductCountInPack = decimal.Parse(data.ProductCountInPack);
                Product.ProductPackValue = decimal.Parse(data.ProductPackValue);
                Product.ProductPackVolume = decimal.Parse(data.ProductPackVolume);
                Product.ProductPackWeight = decimal.Parse(data.ProductPackWeight);
                Product.ProductUnit = data.ProductUnit;
            }
        }
    }

    public async Task OnRemoveModalClick(MouseEventArgs e)
    {
        int result = (await Api.PostAsync<int>("SRemoveProduct"
             , new("productCode", Product.ProductCode)
             , new("userToken", UserToken))).Value;

        if (result == -2)
        {
            Notification.Show(TextResources.APP_StringKeys_Error_RegisteredProductCode, "error");
        }
        else
        {
            if (result <= 0)
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
            }
            else
            {
                Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
            }

            await OnClearClick(e);
        }
    }

    public async Task<bool> CheckProductEmptiness()
    {
        if (string.IsNullOrEmpty(Product.ProductCode))
        {
            Notification.Show(TextResources.APP_StringKeys_Error_SelectProduct, "error");

            return true;
        }

        return false;
    }

    public async Task OnSampleClick(MouseEventArgs e)
    {
        string directory = Environment.WebRootPath
            + "\\templates\\addproduct.xlsx";

        await Export.ExportAndDownload(directory, "نمونه اکسل ثبت محصول.xlsx");
    }

    public async Task OnYesModal(MouseEventArgs e)
    {
        if (!ExcelProducts.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_AnyData, "error");

            return;
        }

        IsLoading = true;

        await SaveProducts(Mapper.Map<List<SaveProductCommand>>(ExcelProducts));

        ExcelProducts = new();

        IsLoading = false;
    }

    public async Task OnChangePackValue(object value)
    {
        if (IsLoading)
        {
            return;
        }

        Product.ProductPackValue = Product.ProductValue * Product.ProductCountInPack;
    }

    public async Task OnChangeProductGroup(object productGroup)
    {
        if (productGroup is not null)
        {
            Product.ProductGroup = productGroup.ToString();

            FilteredProductSubGroups = ProductSubGroups.Where(p => p.ProductGroupCode == productGroup.ToString()).ToList();

            if (FilteredProductSubGroups.Count() == 1)
            {
                Product.ProductSubGroup = FilteredProductSubGroups.First().Code;
            }
        }
    }

    public async Task OnOpenGallery(MouseEventArgs e)
    {
        await Gallery.ShowFileUploader( GalleryUsageType.Product
            , Product.ProductCode);
    }

    public async Task OpenProductCodeModal()
    {
        if (ProductCodeModalRef is null)
            return;

        await ProductCodeModalRef.Show(Product.ProductCode);
    }

    public Task OnProductSelected(PositionProductResponse product)
    {
        if (product is null)
            return Task.CompletedTask;

        Product.ProductTechnicalData = product.ProductTechnicalData;

        FillTechnicalItemsFromJson(Product.ProductTechnicalData);

        StateHasChanged();
        return Task.CompletedTask;
    }

    private GetAllProductQuery FixEmptinessSearchProducts(string defaultValue = "")
    {
        GetAllProductQuery request = new();

        if (defaultValue.HasValue())
        {
            request.MProductCode = defaultValue;

            request.MProductTitle = defaultValue;

            request.MQuality = defaultValue;

            request.MSize = defaultValue;

            request.MTechCode = defaultValue;

            request.Brand = defaultValue;

            request.Group = defaultValue;
        }
        else
        {
            if (SearchRequest.MProductCode.HasNoValue())
            {
                request.MProductCode = "-1";
            }
            else
            {
                request.MProductCode = SearchRequest.MProductCode;
            }

            if (SearchRequest.MProductTitle.HasNoValue())
            {
                request.MProductTitle = "-1";
            }
            else
            {
                request.MProductTitle = SearchRequest.MProductTitle;
            }

            if (SearchRequest.MQuality.HasNoValue())
            {
                request.MQuality = "-1";
            }
            else
            {
                request.MQuality = SearchRequest.MQuality;
            }

            if (SearchRequest.MSize.HasNoValue())
            {
                request.MSize = "-1";
            }
            else
            {
                request.MSize = SearchRequest.MSize;
            }

            if (SearchRequest.MTechCode.HasNoValue())
            {
                request.MTechCode = "-1";
            }
            else
            {
                request.MTechCode = SearchRequest.MTechCode;
            }

            if (SearchRequest.Group.HasNoValue())
            {
                request.Group = "-1";
            }
            else
            {
                request.Group = SearchRequest.Group;
            }

            if (SearchRequest.Brand.HasNoValue())
            {
                request.Brand = "-1";
            }
            else
            {
                request.Brand = SearchRequest.Brand;
            }

            if (SearchRequest.Class.HasNoValue())
            {
                request.Class = "-1";
            }
            else
            {
                request.Class = SearchRequest.Class;
            }

            if (SearchRequest.SubGroup.HasNoValue())
            {
                request.SubGroup = "-1";
            }
            else
            {
                request.SubGroup = SearchRequest.SubGroup;
            }
        }



        request.IsActive = SearchRequest.IsActive;

        return request;
    }

    private void FixEmptinessSaveProduct()
    {
        if (Product.ProductProperties.HasNoValue())
        {
            Product.ProductProperties = "";
        }

        if (Product.ProductType.HasNoValue())
        {
            Product.ProductType = "";
        }

        if (Product.ProductTechnicalData.HasNoValue())
        {
            Product.ProductTechnicalData = "";
        }
    }

    private async Task<bool> SaveProducts(List<SaveProductCommand> products)
    {
        int result = (await Api.PostAsync<int>("SSaveProductBatch"
             , new KeyValuePair<string, object>("products", products))).Value;

        if (result == 1)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            return true;
        }
        else if (result == -2)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Product_ApiUnique, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        return false;
    }

    private async Task<string> SetImage(GetGalleryImageFileQuery request)
    {
        var imageBytes = await Api.PostAsync("Gallery/GetGalleryImageFile", request);

        if (imageBytes is not null)
        {
            return ImageTools.ConvertImageByteToBase64String(imageBytes);
        }

        return null;
    }

    private void PrepareTechnicalDataForSave()
    {
        if (TechnicalDataItems == null || !TechnicalDataItems.Any())
        {
            Product.ProductTechnicalData = string.Empty;
            return;
        }

        var dictionary = TechnicalDataItems
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .GroupBy(x => x.Key.Trim())
            .ToDictionary(
                g => g.Key,
                g => g.Last().Value?.Trim() ?? string.Empty);

        Product.ProductTechnicalData = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
    }

    private void FillTechnicalItemsFromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            TechnicalDataItems = new List<TechnicalDataItemDto>();
            return;
        }

        try
        {
            var technicalObject = JObject.Parse(json);

            TechnicalDataItems = technicalObject.Properties()
                .Select(p => new TechnicalDataItemDto
                {
                    Key = p.Name,
                    Value = p.Value?.ToString() ?? string.Empty
                })
                .ToList();
        }
        catch
        {
            TechnicalDataItems = new List<TechnicalDataItemDto>();
        }
    }

}
