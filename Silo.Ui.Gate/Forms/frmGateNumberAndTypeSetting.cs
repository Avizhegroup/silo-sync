namespace Silo.Ui.Gate;

public partial class frmGateNumberAndTypeSetting : Form
{
    List<WarehouseDto> _listAllWareHousesFrom = new List<WarehouseDto>();
    List<WarehouseDto> _listAllWareHousesTo = new List<WarehouseDto>();
    DAL.ApiBusiness _api = new DAL.ApiBusiness();

    public frmGateNumberAndTypeSetting()
    {
        InitializeComponent();
    }

    private void frmGateNumberAndTypeSetting_Load(object sender, EventArgs e)
    {
        FillData();

    }

    private void ShowSetting()
    {

        txtGateNumber.Text = Properties.Settings.Default.GateNumber;
        txtGateTitle.Text = Properties.Settings.Default.GateTitle;
        chbAtentionForNotRegisteredTag.Checked = (Properties.Settings.Default.AtentionForNotRegisteredTag == "1") ? true : false;
        chbIgnoringNonExistentGoods.Checked = (Properties.Settings.Default.IgnoringNonExistentGoods == "1") ? true : false;
        foreach (string FromStoreCodeSelected in Properties.Settings.Default.FromStore.Split(','))
        {
            for (int i = 0; i < cmbFromStore.CheckBoxItems.Count; i++)
            {
                if (cmbFromStore.CheckBoxItems[i].Text.Split('_')[0] == FromStoreCodeSelected)
                {
                    cmbFromStore.CheckBoxItems[i].Checked = true;
                }
            }
        }

        cmbToStore.SelectedValue = Properties.Settings.Default.ToStore;
        cmbKindGateSave.SelectedIndex = Properties.Settings.Default.KindGateSave;

        chbUserAccessForDeleteTrueSerial.Checked = (Properties.Settings.Default.UserAccessForDeleteTrueSerial == "1") ? true : false;
        chbUserAccessForDeleteErrorSerial.Checked = (Properties.Settings.Default.UserAccessForDeleteErrorSerial == "1") ? true : false;


        chbControlByDocument.Checked = (Properties.Settings.Default.ControlByDocument == "1") ? true : false;
        chbGetDocumentFromProductInfo.Checked = (Properties.Settings.Default.GetDocumentFromProductInfo == "1") ? true : false;
        chbRequirmentDocument.Checked = (Properties.Settings.Default.RequirmentDocument == "1") ? true : false;
        chbAtentionForNotQCTag.Checked = (Properties.Settings.Default.AtentionForNotQCTag == "1") ? true : false;
        chbAtentionForFreezedTag.Checked = (Properties.Settings.Default.AtentionForFreezedTag == "1") ? true : false;
        chbProductValueImportant.Checked = (Properties.Settings.Default.ProductValueImportant == "1") ? true : false;
        chbIgnoringTagAgeThenLess.Checked = (Properties.Settings.Default.IgnoringTagAgeThenLess == "1") ? true : false;
        txtIgnoringTagAgeThenLessDetails.Text = Properties.Settings.Default.IgnoringTagAgeThenLessDetails;

        chbAtentionForWithOutQCTag.Checked = (Properties.Settings.Default.AtentionForWithOutQCTag == "1") ? true : false;


    }


    private async void FillData()
    {

        try
        {
            _listAllWareHousesFrom = await _api.GetAllWarehouses();
            _listAllWareHousesTo = await _api.GetAllWarehouses();
            if (_listAllWareHousesFrom != null)
            {
                foreach (WarehouseDto _wms in _listAllWareHousesFrom)
                {
                    cmbFromStore.Items.Add(_wms.DestinationCode + "_" + _wms.DestinationTitle);
                }
            }
            //cmbFromStore.DataSource=  _listAllWareHousesFrom;
            //cmbFromStore.DisplayMember="DestinationTitle";
            //cmbFromStore.ValueMember="DestinationCode";


            cmbToStore.DataSource = _listAllWareHousesTo;
            cmbToStore.DisplayMember = "DestinationTitle";
            cmbToStore.ValueMember = "DestinationCode";

            Thread.Sleep(4000);
            ShowSetting();

        }
        catch
        {

        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.GateNumber = txtGateNumber.Text;
        Properties.Settings.Default.GateTitle = txtGateTitle.Text;
        Properties.Settings.Default.ProductValueImportant = (chbProductValueImportant.Checked == true) ? "1" : "0";
        Properties.Settings.Default.AtentionForNotRegisteredTag = (chbAtentionForNotRegisteredTag.Checked == true) ? "1" : "0";
        Properties.Settings.Default.IgnoringNonExistentGoods = (chbIgnoringNonExistentGoods.Checked == true) ? "1" : "0";
        Properties.Settings.Default.AtentionForWithOutQCTag = (chbAtentionForWithOutQCTag.Checked == true) ? "1" : "0";
        Properties.Settings.Default.IgnoringTagAgeThenLess = (chbIgnoringTagAgeThenLess.Checked == true) ? "1" : "0";
        Properties.Settings.Default.IgnoringTagAgeThenLessDetails = txtIgnoringTagAgeThenLessDetails.Text;
        var FromStoreCodeList = string.Empty;
        var FromStoreTitleList = string.Empty;

        for (int i = 0; i < cmbFromStore.CheckBoxItems.Count; i++)
        {
            if (cmbFromStore.CheckBoxItems[i].Checked)
            {
                FromStoreCodeList += cmbFromStore.CheckBoxItems[i].Text.Split('_')[0] + ",";
                FromStoreTitleList += cmbFromStore.CheckBoxItems[i].Text.Split('_')[1] + ",";
            }
        }
        Properties.Settings.Default.FromStore = FromStoreCodeList;
        Properties.Settings.Default.ToStore = (cmbToStore.SelectedValue != null) ? cmbToStore.SelectedValue.ToString() : "-1";
        Properties.Settings.Default.FromStoreTitle = FromStoreTitleList;
        Properties.Settings.Default.ToStoreTitle = cmbToStore.Text.ToString();


        Properties.Settings.Default.UserAccessForDeleteErrorSerial = (chbAtentionForNotQCTag.Checked) ? "1" : "0";
        Properties.Settings.Default.UserAccessForDeleteTrueSerial = (chbUserAccessForDeleteTrueSerial.Checked) ? "1" : "0";
        Properties.Settings.Default.AtentionForNotQCTag = (chbAtentionForNotQCTag.Checked) ? "1" : "0";
        Properties.Settings.Default.AtentionForFreezedTag = (chbAtentionForFreezedTag.Checked) ? "1" : "0";

        Properties.Settings.Default.ControlByDocument = (chbControlByDocument.Checked) ? "1" : "0";
        Properties.Settings.Default.GetDocumentFromProductInfo = (chbGetDocumentFromProductInfo.Checked) ? "1" : "0";
        Properties.Settings.Default.RequirmentDocument = (chbRequirmentDocument.Checked) ? "1" : "0";

        Properties.Settings.Default.KindGateSave = cmbKindGateSave.SelectedIndex;
        Properties.Settings.Default.Save();
        MessageBox.Show("ثبت تنظیمات با موفقیت انجام شد");

    }
}
