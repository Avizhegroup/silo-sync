using Silo.Application;

namespace Silo.Pages.Settings;
public partial class ConvertData
{
    public bool IsLoading = true;
    public string ConvertJsonDocumentType = string.Empty;
    public List<SalesShopExcelDto> ExcelSalesShops = new();
    public List<SalesInstallerExcelDto> ExcelSalesInstallers = new();
    public List<GetAllProvinceVm> Provinces = new();
    public List<GetCitiesVm> Cities;

    public Modal ModalSalesShopExcel { get; set; }
    public Modal ModalSalesInstallerExcel { get; set; }
    public TelerikGrid<SalesShopExcelDto> ExcelSalesShopGridRef { get; set; }
    public TelerikGrid<SalesInstallerExcelDto> ExcelSalesInstallerGridRef { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IExport Export { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }

    protected override async Task SiloInitializer()
    {
        try
        {
            Provinces = (await Api.PostAsyncByContext<List<GetAllProvinceVm>>("GetAllProvinces"
            , new GetAllProvinceVmContext())).Value;

            Cities = (await Api.PostAsyncByContext<List<GetCitiesVm>>("GetAllCities"
                , new GetCitiesVmContext())).Value;
        }
        catch (Exception)
        {
            Cities = new();
        }

        IsLoading = false;
    }

    public async Task OnConvertJsonDocumentClick(MouseEventArgs e)
    {
        if (ConvertJsonDocumentType.HasNoValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Empty_Completable, TextResources.APP_StringKeys_Settings_ConvertJsonDocument), "error");

            return;
        }

        IsLoading = true;

        int result = (await Api.PostAsync<int>("SConvertDataFromJArrayToJToken"
            , new KeyValuePair<string, object>("documentType", ConvertJsonDocumentType))).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }

    public async Task OnConvertTruckCrossStatusClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsyncByUri<int>("wms/truckcross", "SConvertTruckCrossStatus")).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }

    public async Task OnCopyTagsToGuaranteeClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsyncByUri<int>("wms/Product", "SCopyTagsToGuarantee")).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }

    public async Task OnCreateUHFReaderLogHeadersFromUHFReaderLogClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SCreateUHFReaderLogHeadersFromUHFReaderLog")).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }

    #region SalesShop
    public void OnRemoveSalesShopClick(SalesShopExcelDto product)
    {
        ExcelSalesShops.Remove(product);

        ExcelSalesShopGridRef.Rebind();

        StateHasChanged();
    }

    public async Task OnSampleSalesShopClick(MouseEventArgs e)
    {
        string directory = $"{Environment.WebRootPath}\\templates\\salesshop.xlsx";

        await Export.ExportAndDownload(directory, $"{TextResources.APP_StringKeys_ExcelSample_Shop}.xlsx");
    }

    public async Task OnCompleteUploadSalesShopExcelAdd(string path)
    {
        var data = DataTableTools.ReadExcelDataExportInDataTable(path);

        ExcelSalesShops = new();

        int index = 0;

        foreach (DataRow row in data.Tables[0].Rows)
        {
            if (index == 0)
            {
                index++;

                continue;
            }

            if (row.ItemArray.Length < 9)
            {
                await ModalSalesShopExcel.Close(new());

                Notification.Show(TextResources.APP_StringKeys_Validation_Excel_Format
                    , "error");

                IsLoading = false;

                return;
            }

            ExcelSalesShops.Add(new()
            {
                Code = row.ItemArray[0].ToString(),
                Title = row.ItemArray[1].ToString(),
                ManagerName = row.ItemArray[2].ToString(),
                CityId = Cities.FirstOrDefault(p => p.Title.Equals(row.ItemArray[3].ToString().Trim()))?.Id,
                ProvinceId = Provinces.FirstOrDefault(p => p.Title.Equals(row.ItemArray[4].ToString().Trim()))?.Id,
                Phone = row.ItemArray[5].ToString(),
                Mobile = row.ItemArray[6].ToString(),
                Address = row.ItemArray[7].ToString(),
                Password = row.ItemArray[8].ToString(),
                ErrorMessage = string.IsNullOrEmpty(row.ItemArray[0].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_SalesShop_Code) :
                string.IsNullOrEmpty(row.ItemArray[1].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_SalesShop_Title) :
                string.IsNullOrEmpty(row.ItemArray[8].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Password) : ""
            });
        }

        var duplicatedStoreCodes = (await Api.PostAsync<List<string>>("SGetDuplicatedSalesStoreCodes"
            , new KeyValuePair<string, object>("salesStoreCodes", ExcelSalesShops.Select(p => p.Code).ToList()))).Value;

        ExcelSalesShops.Where(p => duplicatedStoreCodes.Any(q => q == p.Code)).ToList().ForEach(p => p.ErrorMessage = TextResources.APP_StringKeys_Validation_Code_Uniqueness);

        IsLoading = false;
    }

    public async Task OnYesSalesShopModalClick(MouseEventArgs e)
    {
        if (!ExcelSalesShops.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_AnyData, "error");

            return;
        }

        if (ExcelSalesShops.Any(p => p.ErrorMessage.HasValue()))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Prevent_On_Errors, "error");

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsync<int>("SSaveSalesShop"
            , new KeyValuePair<string, object>("shops", ExcelSalesShops))).Value;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        ExcelSalesShops = new();

        IsLoading = false;
    }

    public void OnRowSalesShopRenderHandler(GridRowRenderEventArgs args)
    {
        string classStr = string.Empty;

        SalesShopExcelDto item = args.Item as SalesShopExcelDto;

        if (item.ErrorMessage.HasValue())
        {
            classStr += "bg-danger";
        }

        args.Class += classStr;
    }
    #endregion

    #region SalesInstaller
    public void OnRemoveSalesInstallerClick(SalesInstallerExcelDto product)
    {
        ExcelSalesInstallers.Remove(product);

        ExcelSalesInstallerGridRef.Rebind();

        StateHasChanged();
    }

    public async Task OnSampleSalesInstallerClick(MouseEventArgs e)
    {
        string directory = $"{Environment.WebRootPath}\\templates\\salesinstaller.xlsx";

        await Export.ExportAndDownload(directory, $"{TextResources.APP_StringKeys_ExcelSample_Install}.xlsx");
    }

    public async Task OnCompleteUploadSalesInstallerExcelAdd(string path)
    {
        var data = DataTableTools.ReadExcelDataExportInDataTable(path);

        ExcelSalesInstallers = new();

        int index = 0;

        foreach (DataRow row in data.Tables[0].Rows)
        {
            if (index == 0)
            {
                index++;

                continue;
            }

            if (row.ItemArray.Length < 3)
            {
                await ModalSalesInstallerExcel.Close(new());

                Notification.Show(TextResources.APP_StringKeys_Validation_Excel_Format
                    , "error");

                IsLoading = false;

                return;
            }

            ExcelSalesInstallers.Add(new()
            {
                Code = row.ItemArray[0].ToString(),
                Name = row.ItemArray[1].ToString(),
                Password = row.ItemArray[2].ToString(),
                ErrorMessage = string.IsNullOrEmpty(row.ItemArray[0].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_SalesShop_Code) :
                string.IsNullOrEmpty(row.ItemArray[1].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Field_Name) :
                string.IsNullOrEmpty(row.ItemArray[2].ToString()) ?
                string.Format(TextResources.APP_StringKeys_Validation_Required, TextResources.APP_StringKeys_Password) : ""
            });
        }

        var duplicatedStoreCodes = (await Api.PostAsync<List<string>>("SGetDuplicatedSalesInstallerCodes"
            , new KeyValuePair<string, object>("salesStoreCodes", ExcelSalesInstallers.Select(p => p.Code).ToList()))).Value;

        ExcelSalesInstallers.Where(p => duplicatedStoreCodes.Any(q => q == p.Code)).ToList().ForEach(p => p.ErrorMessage = TextResources.APP_StringKeys_Validation_Code_Uniqueness);

        IsLoading = false;
    }

    public async Task OnYesSalesInstallerModalClick(MouseEventArgs e)
    {
        if (!ExcelSalesInstallers.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_AnyData, "error");

            return;
        }

        if (ExcelSalesInstallers.Any(p => p.ErrorMessage.HasValue()))
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Prevent_On_Errors, "error");

            return;
        }

        IsLoading = true;

        var result = (await Api.PostAsync<int>("SSaveSalesInstallers"
            , new KeyValuePair<string, object>("salesInstallers", ExcelSalesInstallers))).Value;

        if (result > 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        ExcelSalesInstallers = new();

        IsLoading = false;
    }

    public void OnRowSalesInstallerRenderHandler(GridRowRenderEventArgs args)
    {
        string classStr = string.Empty;

        SalesInstallerExcelDto item = args.Item as SalesInstallerExcelDto;

        if (item.ErrorMessage.HasValue())
        {
            classStr += "bg-danger";
        }

        args.Class += classStr;
    }
    #endregion

    #region Add Serial into UhfReaderLog
    public async Task OnFillUhfProductSerialClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SInsertProductSerialFromTagsToUhf")).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }
    #endregion

    #region Update Uhf used statuses
    public async Task OnUpdateUhfStatusesClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SUpdateUhfStatuses")).Value;

        if (result < 0)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }

        IsLoading = false;
    }
    #endregion
}
