using Microsoft.Win32;
namespace Silo.Ui.Gate;


public partial class frmSetting : Form
{ 
    DAL.ApiBusiness _api = new DAL.ApiBusiness();
    public frmSetting()
    {
        InitializeComponent();
    }
    List<WarehouseDto> _listAllWareHousesFrom = new List<WarehouseDto>();
    List<WarehouseDto> _listAllWareHousesTo = new List<WarehouseDto>();

    
    private void frmSetting_Load(object sender, EventArgs e)
    {

        ShowSetting();





    }
    private void ShowSetting()
    {

        txtTrueRelayDetails1.Text=Properties.Settings.Default.TrueRelayDetails1;
        txtTrueRelayDetails2.Text=Properties.Settings.Default.TrueRelayDetails2;
        txtTrueRelayDetails3.Text = Properties.Settings.Default.TrueRelayDetails3;
        txtTrueRelayDetails4.Text = Properties.Settings.Default.TrueRelayDetails4;

        chbTrueRelay.Checked = (Properties.Settings.Default.TrueRelay == "1") ? true : false;
        chbAppReset.Checked = (Properties.Settings.Default.AppReset == "1") ? true : false;
        

        txtAlarmRelayDetails1.Text = Properties.Settings.Default.AlarmRelayDetails1;
        txtAlarmRelayDetails2.Text = Properties.Settings.Default.AlarmRelayDetails2;
        txtAlarmRelayDetails3.Text = Properties.Settings.Default.AlarmRelayDetails3;
        txtAlarmRelayDetails4.Text = Properties.Settings.Default.AlarmRelayDetails4;

        chbAlarmRelay.Checked = (Properties.Settings.Default.AlarmRelay == "1") ? true : false;

        chbChangeReaderPowerTools.Checked = (Properties.Settings.Default.ChangeReaderPowerTools == "1") ? true : false;

         chbGetTagFromHandheld.Checked = (Properties.Settings.Default.GetTagFromHandheld == "1") ? true : false;
        chbTagDetectInMultiPart.Checked=(Properties.Settings.Default.TagDetectInMultiPart=="1") ? true : false;

        
        
         txtReaderConnectDetails.Text=Properties.Settings.Default.ReaderConnectDetails;
        txtReaderPower.Text=Properties.Settings.Default.ReaderPower.ToString();
        cmbReaderType.SelectedItem =Properties.Settings.Default.ReaderType;
        cmbReaderConnectionType.SelectedItem=Properties.Settings.Default.ReaderConnectionType;
        cmbKindShowProduct.SelectedIndex=Properties.Settings.Default.KindShowProduct;
         cmbStartActionKind.SelectedIndex=Properties.Settings.Default.StartActionKind!="" ? int.Parse(Properties.Settings.Default.StartActionKind): -1;
       cmbEndActionKind.SelectedIndex= Properties.Settings.Default.EndActionKind!="" ? int.Parse(Properties.Settings.Default.EndActionKind) : -1;

         chbTrueRelay.Checked=(Properties.Settings.Default.TrueRelay=="1") ? true : false;
        chbDetectMachin.Checked=(Properties.Settings.Default.DetectANDSaveMachin=="1") ? true : false;
        chbSearchMachin.Checked=(Properties.Settings.Default.SearchMachin=="1") ? true : false;
        cmbMachinType.SelectedIndex=Properties.Settings.Default.MachinType;


        chbPlaySoundForConfirm.Checked = (Properties.Settings.Default.PlaySoundForConfirm == "1") ? true : false;
        chbPlaySoundForAlarm.Checked = (Properties.Settings.Default.PlaySoundForAlarm == "1") ? true : false;



        chbGetInventorySummaryByStoreCode.Checked = (Properties.Settings.Default.GetInventorySummaryByStoreCode == "1") ? true : false;
         



        txtStartActionDetails.Text=Properties.Settings.Default.StartActionDetails;
        txtEndActionDetails.Text=Properties.Settings.Default.EndActionDetails;

    }

    private void SaveSetting()
    {


        Properties.Settings.Default.TrueRelayDetails1=txtTrueRelayDetails1.Text;
        Properties.Settings.Default.TrueRelayDetails2=txtTrueRelayDetails2.Text;
        Properties.Settings.Default.TrueRelayDetails3 = txtTrueRelayDetails3.Text;
        Properties.Settings.Default.TrueRelayDetails4 = txtTrueRelayDetails4.Text;

        
        Properties.Settings.Default.AppReset = (chbAppReset.Checked == true) ? "1" : "0";
        Properties.Settings.Default.TrueRelay = (chbTrueRelay.Checked == true) ? "1" : "0";



        Properties.Settings.Default.AlarmRelayDetails1 = txtAlarmRelayDetails1.Text;
        Properties.Settings.Default.AlarmRelayDetails2 = txtAlarmRelayDetails2.Text;
        Properties.Settings.Default.AlarmRelayDetails3 = txtAlarmRelayDetails3.Text;
        Properties.Settings.Default.AlarmRelayDetails4 = txtAlarmRelayDetails4.Text;
     
        
        Properties.Settings.Default.AlarmRelay = (chbAlarmRelay.Checked) ? "1" : "0";



         Properties.Settings.Default.GetTagFromHandheld = (chbGetTagFromHandheld.Checked == true) ? "1" : "0";
        Properties.Settings.Default.TagDetectInMultiPart = (chbTagDetectInMultiPart.Checked == true) ? "1" : "0";


        Properties.Settings.Default.PlaySoundForAlarm = (chbPlaySoundForAlarm.Checked == true) ? "1" : "0";

        Properties.Settings.Default.PlaySoundForConfirm = (chbPlaySoundForConfirm.Checked == true) ? "1" : "0";


         Properties.Settings.Default.ReaderConnectDetails=txtReaderConnectDetails.Text;
        Properties.Settings.Default.ReaderPower=txtReaderPower.Text;
        Properties.Settings.Default.ReaderType=(cmbReaderType.SelectedItem!=null)? cmbReaderType.SelectedItem.ToString():"";
        Properties.Settings.Default.ReaderConnectionType= (cmbReaderConnectionType.SelectedItem != null) ? cmbReaderConnectionType.SelectedItem.ToString():"";
        Properties.Settings.Default.KindShowProduct=cmbKindShowProduct.SelectedIndex;
        Properties.Settings.Default.StartActionKind=cmbStartActionKind.SelectedIndex.ToString();
        Properties.Settings.Default.EndActionKind=cmbEndActionKind.SelectedIndex.ToString();
         Properties.Settings.Default.DetectANDSaveMachin=(chbDetectMachin.Checked)?"1":"0" ;
        Properties.Settings.Default.SearchMachin =(chbSearchMachin.Checked) ? "1" : "0";
        Properties.Settings.Default.MachinType=cmbMachinType.SelectedIndex;

        Properties.Settings.Default.ChangeReaderPowerTools = (chbChangeReaderPowerTools.Checked) ? "1" : "0";



        Properties.Settings.Default.StartActionDetails= txtStartActionDetails.Text;
        Properties.Settings.Default.EndActionDetails= txtEndActionDetails.Text;
        if (Properties.Settings.Default.StartUpFlag != 1)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rkApp.SetValue("TDA_RFIDConnect_AntennaConnector.exe", Application.ExecutablePath.ToString());
            Properties.Settings.Default.StartUpFlag = 1;
        }



        Properties.Settings.Default.Save();
        MessageBox.Show("ثبت تنظیمات با موفقیت انجام شد");

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        SaveSetting();
    }

    private void groupBox9_Enter(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
        new frmConnectionSetting().Show();
    }

    private void button2_Click(object sender, EventArgs e)
    {
        new frmGateNumberAndTypeSetting().Show();

    }
}
