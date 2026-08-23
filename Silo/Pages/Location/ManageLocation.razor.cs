using Silo.Application;

namespace Silo.Pages.Location;
public partial class ManageLocation
{
    public bool IsLoading = true;
    public bool IsShownTreeview = false;
    public string UserId;
    public GetAllZonesVm Request = new();
    public GetAllZonesVm RequestParent;
    public List<GetAllZonesVm> Locations;
    public List<TreeviewNode> Nodes = new();
    public List<TelerikContextMenuItem> ContextMenuItems = new()
    {
        new()
        {
            Text = "انتخاب به عنوان والد",
            Icon = "parent",
            Items = new List<TelerikContextMenuItem>()
        }
    };
    public List<GetAllWarehousesVm> Warehouses;
    public List<ZoneExcelDto> ZoneExcels = new();
    public List<SaveZoneCommand> SaveZoneExcel = new();

    public Modal ModalDelete { get; set; }
    public Modal ModalExcel { get; set; }
    public TelerikContextMenu<TelerikContextMenuItem> ContextMenu { get; set; }
    public TelerikGrid<ZoneExcelDto> ZoneExcelGridRef { get; set; }

    [Inject] public IExport Export { get; set; }
    [Inject] public IWebHostEnvironment Environment { get; set; }
    [Inject] public IFormalDataCache FormalCache { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public ILogger<ManageLocation> Logger { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Warehouses = await FormalCache.GetWarehouses();

        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        await RefreshTree();

        IsLoading = false;
    }

    public async Task OnSaveClick(MouseEventArgs e)
    {
        if (await CheckEmptiness())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_EmptinessCheck, "error");

            return;
        }

        IsLoading = true;

        Request.Title = Request.ZoneCode;

        bool result = (await Api.PostAsync<int>("SPDefineZone",
                new("ZoneCode", Request.ZoneCode),
                new("ZoneTitle", Request.Title),
                new("ZoneDimention", "0"),
                new("ZoneParentCode", Request.ParentCode),
                new("ZoneParentLayer", Request.ParentLayer),
                new("ZoneStoreCode", Request.StoreCode),
                new("ZoneCountPixle", "0"),
                new("MinZoneCapacity", Request.MinCapacity),
                new("MaxZoneCapacity", Request.MaxCapacity),
                new("ZoneRowIndex", Request.RowIndex),
                new("UserCode", UserId))).Value > 0;

        IsLoading = false;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");

            await RefreshTree(true);
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }
    }

    public async Task OnNodeSelect(TreeViewItemClickEventArgs e)
    {
        var node = (TreeviewNode)e.Item;

        GetAllZonesVm location = Locations.FirstOrDefault(p => p.ZoneCode == node.thisnodeid);

        Request = location ?? new();
    }

    public async Task OnNodeRightClick(MouseEventArgs mouseE, TreeviewNode node)
    {
        RequestParent = Locations.FirstOrDefault(p => p.ZoneCode == node.thisnodeid);

        await ContextMenu.ShowAsync(mouseE.PageX, mouseE.PageY);
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Request.ZoneCode.HasNoValue())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "error");

            return;
        }

        await ModalDelete.Open(e);
    }

    public async Task OnRemoveChoosedItemClick(MouseEventArgs e)
    {
        IsLoading = true;

        int result = (await Api.PostAsync<int>("SPDeleteZone"
        , new("ZoneCode", Request.ZoneCode)
        , new("UserCode", UserId))).Value;

        string messageText = string.Empty;

        switch (result)
        {
            case -3:
                messageText = TextResources.APP_StringKeys_Validation_HaveChild;
                break;
            case -2:
                messageText = TextResources.APP_StringKeys_Validation_Capacity;
                break;
            case -1 or 0:
                messageText = TextResources.APP_StringKeys_Alert_Fail;
                break;
            default:
                messageText = TextResources.APP_StringKeys_Alert_Success;

                await RefreshTree(true);

                break;
        }

        IsLoading = false;

        if (result <= 0)
        {
            Notification.Show(messageText, "error");
        }
        else
        {
            Notification.Show(messageText, "success");
        }
    }

    public async Task OnContextMenuItemClick(TelerikContextMenuItem item)
    {
        if (item.Text == "انتخاب به عنوان والد")
        {
            Request.ParentCode = RequestParent is null ? "-1" : RequestParent.ZoneCode;

            Request.ParentLayer = RequestParent is null ? "1" : (int.Parse(RequestParent.ParentLayer) + 1).ToString();
        }
    }

    public async Task OnComboWarehouseChange(object sender)
    {
        IsLoading = true;

        await RefreshTree();

        IsLoading = false;
    }

    #region Import Excel

    

    public async Task OnSampleClick(MouseEventArgs e)
    {
        string directory = Environment.WebRootPath
            + "\\templates\\addzone.xlsx";

        await Export.ExportAndDownload(directory, "نمونه اکسل ثبت لوکیشن.xlsx");
    }

    public async Task OnCompleteUploadExcelAdd(string path)
    {
        try
        {
            var data = DataTableTools.ReadExcelDataExportInDataTable(path);

            ZoneExcels.Clear();

            foreach (DataRow row in data.Tables[0].Rows)
            {
                if (row.ItemArray.Length < 7)
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

                ZoneExcels.Add(new()
                {
                    Code = row.ItemArray[0].ToString(),
                    Title = row.ItemArray[1].ToString(),
                    Dimention = "0",
                    ParentCode = row.ItemArray[2].ToString(),
                    ParentLayer = int.Parse(row.ItemArray[3].ToString()),
                    WarehouseCode = row.ItemArray[4].ToString(),
                    MinCapacity = decimal.Parse(row.ItemArray[5].ToString()),
                    MaxCapacity = decimal.Parse(row.ItemArray[6].ToString()),
                    RowIndex = int.Parse(row.ItemArray[7].ToString()),
                    CountPixle = 0,
                    OccupiedCapacity = 0
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, ex.Message);

            Notification.Show(TextResources.APP_StringKeys_Validation_Excel_Format, "error");

            ZoneExcels.Clear();

            IsLoading = false;

            return;
        }

        await BatchInsertData();

        IsLoading = false;
    }
    #endregion

    private async Task RefreshTree(bool isRefresh = false)
    {
        IsShownTreeview = false;

        if (Locations is not null)
        {
            Locations.Clear();
        }

        Locations = (await Api.PostAsync<List<GetAllZonesVm>>("SPGetZonesByWarehouse"
                      , new KeyValuePair<string, object>("code", Request.StoreCode))).Value
                    .OrderBy(p => p.RowIndex)
                    .ToList();

        TreeviewNode allParent = new()
        {
            thisnodeid = "-1",
            selectable = false,
            text = TextResources.APP_StringKeys_Treeview,
            value = "-1",
            index = -2,
        };

        GetChildrenLocation(allParent);

        Nodes.Clear();

        Nodes.Add(allParent);

        IsShownTreeview = true;
    }

    private async Task<bool> CheckEmptiness()
    {
        if (Request.ZoneCode.HasNoValue())
        {
            return true;
        }

        if (Request.ParentCode.HasNoValue())
        {
            return true;
        }

        if (Request.RowIndex.HasNoValue())
        {
            return true;
        }

        if (Request.ParentLayer.HasNoValue())
        {
            return true;
        }

        if (Request.MaxCapacity.HasNoValue())
        {
            return true;
        }

        return false;
    }

    private void GetChildrenLocation(TreeviewNode node)
    {
        Logger.LogInformation(node.thisnodeid);

        List<GetAllZonesVm> children = Locations
                 .Where(p => p.ParentCode == node.thisnodeid && p.ZoneCode != node.thisnodeid)
                 .ToList();

        List<TreeviewNode> nodes = new();

        foreach (GetAllZonesVm child in children)
        {
            TreeviewNode childNode = new()
            {
                thisnodeid = child.ZoneCode,
                text = child.Title,
                value = child.ZoneCode,
                index = int.Parse(child.RowIndex),
                selectable = true
            };

            GetChildrenLocation(childNode);

            nodes.Add(childNode);
        }

        node.nodes = nodes.OrderBy(p => p.index).ToArray();
    }

    private async Task BatchInsertData()
    {
        if (!ZoneExcels.Any())
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_AnyData, "error");

            return;
        }

        IsLoading = true;

        SaveZoneCommand saveZoneCommand = new SaveZoneCommand();

        saveZoneCommand.Zones = ZoneExcels;

        bool result = (await Api.PostAsync<int>("SInsertZoneExcel",
            new KeyValuePair<string, object>("saveZoneCommand", saveZoneCommand))).Value > 0;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await ModalExcel.Close(new());

        IsLoading = false;
    }
}
