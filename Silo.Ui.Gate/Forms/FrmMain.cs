using System.IO.Ports;
using Newtonsoft.Json.Linq;
using Silo.Ui.Gate.BLL;
using Silo.Ui.Gate.DAL;
using Silo.Ui.Gate.DML;
using Silo.Ui.Gate.Models;
using TimaxDev;

namespace Silo.Ui.Gate;
public partial class FrmMain : Form
{
    CCommands Commander;

    bool VerifyRelayConnectionStatus = false;
    bool AlarmRelayConnectionStatus = false;
    int ActionType = 0;
    List<DynamicFieldDto> _dynamicFieldDtos = new();
    bool FlagControlsDocBySumValue = false;
    string _InSaveActionProgressActionId = "";
    ActionStatus _actionStatus;
    string _actionDescription = "";
    int KindShowProduct = 0;
    bool GetDocumentDataFlag = false;
    bool _FlagResetApp = false;
    bool _DocumentCheck = true;
    bool _detectWM = false;
    bool _ErrorDetected = false;
    bool _RequireClear = false;
    bool _RequireWmClear = false;
    bool _InSaveActionProgress = false;
    bool _FlagActionStart = false;
    bool _FlagActionEnd = false;

    DateTime _RequireWMClearDetectedDateTime;
    DateTime _RequireClearDetectedDateTime;
    DateTime _ErrorDetectedDateTime;
    DateTime _WMDetectedDateTime;
    DateTime _LatestTagReadDateTime;
    DateTime _StartActionDateTime;
    DateTime _EndActionDateTime;
    DateTime _LatestTrueTagReadDateTime;

    UhfReaders _UhfReaders = new UhfReaders();
    MotionDetectionSensor _MotionDetectionSensor = new ();
    string _ActionId = "";

    string WM_ReadEPC = "";

    List<DocumentItem> _documentItem = new ();
    List<Tags> _DetectedTagListWhenGateInSaveAction = new ();

    List<Tags> _DetectedTagList = new ();
    List<GateResult> _GateResultList = new ();
    List<GateResult> _GateResultListTajamoee = new ();
    List<GateResult> _GateResultListFiltered = new();
    List<WarehouseMachines> WarehouseMachineslist = new ();

    ApiBusiness _apiBusiness = new ();
    int MultiPartRowIndex = 1;

    CancellationTokenSource _cts = new CancellationTokenSource();


    public FrmMain()
    {
        InitializeComponent();

        _UhfReaders.Connect();


        _UhfReaders._setTextCallback += _UhfReaders__setTextCallback;


        KindShowProduct = Properties.Settings.Default.KindShowProduct;

        if (Properties.Settings.Default.TrueRelay == "1")
        {
            try
            {
                Commander = new CCommands(CUDP.ProtocolType.TCP_CLIENT,
                               0,
                               null,
                               null,
                               null);
            }
            catch
            {

            }
        }





    }

    private void _UhfReaders__setTextCallback(List<Tags> _detectedTagList)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => _UhfReaders__setTextCallback(_detectedTagList)));
            return;
        }

        try
        {
            foreach (Tags _tag in _detectedTagList)
            {
                lblConnectToReaderStatus.BackColor = Color.Green;

                if (!string.IsNullOrEmpty(lblActionId.Text) && lblActionId.Text != "0" && !string.IsNullOrEmpty(_ActionId) && _ActionId != "0")
                {
                    _tag.TagPackageId = MultiPartRowIndex;
                    if (!_tag.TagEPC.Contains(Properties.Settings.Default.WM_EPC_Pattern))
                    {
                        _LatestTagReadDateTime = DateTime.Now;
                        if (_DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == _tag.TagEPC.ToLower()) == null)
                        {
                            _tag.TagReedTime = DateTime.Now;
                            _tag.TagActionStatus = 0;
                            _tag.TagReedSaveStatus = 0;
                            _tag.TagReedUpdateStatus = 0;
                            _tag.TagPackageId = MultiPartRowIndex;
                            _tag.DocumentId = lblPnlPart_DocumentId.Text;
                            _tag.WMUsertId = lblWMUserId.Text;
                            _tag.TagBeforSendStatus = 0;

                            _DetectedTagList.Add(_tag);
                        }
                        else if (_DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == _tag.TagEPC.ToLower()).TagActionStatus == 3)
                        {
                            var existingTag = _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == _tag.TagEPC.ToLower());
                            if (existingTag != null)
                            {
                                existingTag.TagActionStatus = 0;
                                existingTag.TagReedSaveStatus = 0;
                            }
                        }
                    }
                    else
                    {
                        WM_ReadEPC = _tag.TagEPC;
                        ShowWMInfo(WM_ReadEPC);
                    }
                }
                else
                {
                    if (Properties.Settings.Default.EndActionKind != "4")
                    {
                        if (_DetectedTagListWhenGateInSaveAction.FirstOrDefault(p => p.TagEPC.ToLower() == _tag.TagEPC.ToLower()) == null)
                        {
                            _tag.TagReedTime = DateTime.Now;
                            _tag.TagActionStatus = 0;
                            _tag.TagReedSaveStatus = 0;
                            _tag.TagReedUpdateStatus = 0;
                            _tag.TagPackageId = MultiPartRowIndex;
                            _tag.DocumentId = lblPnlPart_DocumentId.Text;
                            _tag.WMUsertId = lblWMUserId.Text;

                            _DetectedTagListWhenGateInSaveAction.Add(_tag);
                        }
                    }
                }
            }

            lblCountTagWhenInSaveAction.Text = _DetectedTagListWhenGateInSaveAction.Count.ToString();
            lblDetectionTagsCount.Text = $"{_DetectedTagList.Count}/{_detectedTagList.Count}";
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }
    private async void SelectWarehouseMachines(CancellationToken token)
    {
        try
        {
            WarehouseMachineslist = await _apiBusiness.GetAllWarehouseMachines();

            token.ThrowIfCancellationRequested();
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);

            token.ThrowIfCancellationRequested();
        }
    }


    private async void GetInventorySummaryByStoreCode(CancellationToken token)
    {
        try
        {
            if (Properties.Settings.Default.GetInventorySummaryByStoreCode == "1")
            {
                var CountInventory = await _apiBusiness.GetInventorySummaryByStoreCode(Properties.Settings.Default.ToStore);

                this.Invoke(new Action(() => lblCountInventory.Text = CountInventory));
            }

            token.ThrowIfCancellationRequested();
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }


    private void PlaySound(string TypeSound)
    {
        try
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            switch (TypeSound)
            {

                case "ErrorWithOutLockForm":
                    if (Properties.Settings.Default.PlaySoundForAlarm == "1")
                    {
                        player.SoundLocation = Application.StartupPath + @"\\sounds\\Alarm_2.wav";
                        this.Invoke(new Action(() => player.Play()));
                        player.Play();
                    }
                    break;
                case "Error":
                    if (Properties.Settings.Default.PlaySoundForAlarm == "1")
                    {
                        player.SoundLocation = Application.StartupPath + @"\\sounds\\Alarm.wav";
                        this.Invoke(new Action(() => player.Play()));
                        player.Play();
                    }
                    break;
                case "ActionVerify":
                    if (Properties.Settings.Default.PlaySoundForConfirm == "1")
                    {

                        player.SoundLocation = Application.StartupPath + @"\\sounds\\Verify.wav";
                        this.Invoke(new Action(() => player.Play()));

                    }
                    break;
                case "TwoActionVerify":
                    if (Properties.Settings.Default.PlaySoundForConfirm == "1")
                    {
                        player.SoundLocation = Application.StartupPath + @"\\sounds\\Verify.wav";
                        this.Invoke(new Action(() => player.Play()));
                        Thread.Sleep(2200);
                        this.Invoke(new Action(() => player.Play()));

                    }
                    break;
            }
        }
        catch
        {

        }
    }


    private void MainForm_Load(object sender, EventArgs e)
    {
        _actionStatus = ActionStatus.NoCargo;
        try
        {


            GetGateSetting();




            if (Properties.Settings.Default.DetectANDSaveMachin == "1")
            {
                grpMachin.Visible = true;
                if (Properties.Settings.Default.MachinType == 1)
                {
                    pcbMachin.Image = Resources._2251137;
                    lableMachinId.Text = "پلاک:";

                }
            }
            else
            {
                // grpMachin.Visible=false;
                //grpPartDetected.Location=new Point(grpPartDetected.Location.X, grpPartDetected.Location.Y-96);
                //grpPartDetected.Size=new Size(grpPartDetected.Size.Width, grpPartDetected.Size.Height+96);
            }

            if (Properties.Settings.Default.TagDetectInMultiPart == "1")
            {
                pnlPartDetected.Visible = true;
            }
            else
            {
                pnlPartDetected.Visible = false;
            }




            timer_Clock.Enabled = true;
            timer_GetProductDatList.Enabled = true;


            if (Properties.Settings.Default.GetTagFromHandheld == "1")
                timer_GetFromHandHeld.Enabled = true;
            else
                timer_GetFromHandHeld.Enabled = false;


            ConnectToRelay();


            GetANDSetDynamicFieldDtos();

            RefreshAction();

             try
            {



                _UhfReaders.Stop();
                lblConnectToReaderStatus.BackColor = Color.Red;
                Thread.Sleep(5000);

                _UhfReaders.SetConfig(Convert.ToInt32(lblReaderPowerSet.Text));
                Thread.Sleep(5000);

                lblConnectToReaderStatus.Text = _UhfReaders.GetPower().ToString();
                Thread.Sleep(5000);

                if (_UhfReaders.Start())
                {
                     timer_RefreshReaderBuffer.Enabled = true;
                    Thread.Sleep(5000);

                    lblConnectToReaderStatus.BackColor = Color.Yellow;
                    if (lblConnectToReaderStatus.Text == "0")
                    {
                        Thread.Sleep(5000);

                        //  timer_appReset_Tick(sender, e);

                    }

                }
                else
                {
                    lblConnectToReaderStatus.BackColor = Color.Red;
                    lblConnectToReaderStatus.Text = "0";
                }


            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
                _UhfReaders.Stop();
                lblConnectToReaderStatus.BackColor = Color.Red;
                lblConnectToReaderStatus.Text = "0";
            }




            if (Properties.Settings.Default.StartActionKind == "3")
            {


                StartAction(0);
            }

        }
        catch
        {

        }


    }

    private void ConnectToRelay()
    {
        try
        {
            if (Properties.Settings.Default.TrueRelay == "1")
            {

                if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "timax" && !VerifyRelayConnectionStatus)
                {
                    VerifyRelayConnectionStatus = Commander.Connect(Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt32(Properties.Settings.Default.TrueRelayDetails2));

                    if (VerifyRelayConnectionStatus == false)
                        MessageBox.Show("Can not Connect to: " + Properties.Settings.Default.TrueRelayDetails1 + " RelayPort: " + Properties.Settings.Default.TrueRelayDetails2);
                }
                else if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "arduino" && !VerifyRelayConnectionStatus)
                {
                    try
                    {
                        if (serialPort1.IsOpen)
                            serialPort1.Close();
                        serialPort1.PortName = Properties.Settings.Default.TrueRelayDetails2;
                        if (!(serialPort1.IsOpen))
                        {
                            serialPort1.Open();
                            serialPort1.BaudRate = 9600;
                            serialPort1.Parity = Parity.Even;
                            serialPort1.StopBits = StopBits.One;
                            serialPort1.DataBits = 8;
                            serialPort1.Handshake = Handshake.None;
                            serialPort1.RtsEnable = true;
                            //if ((serialPort1.IsOpen))
                            //    serialPort1.DataReceived += serialPort1_DataReceived;

                        }
                        else
                        {

                        }
                    }
                    catch (UnauthorizedAccessException exUnauthorizedAccess)
                    {
                    }
                }
            }
            else
                VerifyRelayConnectionStatus = false;



            if (Properties.Settings.Default.AlarmRelay == "1")
            {
                if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "timax" && !AlarmRelayConnectionStatus)
                {

                    if (Properties.Settings.Default.TrueRelayDetails1 == Properties.Settings.Default.AlarmRelayDetails1 && Properties.Settings.Default.TrueRelayDetails2 == Properties.Settings.Default.AlarmRelayDetails2 && AlarmRelayConnectionStatus)
                    {
                    }
                    else
                    {
                        AlarmRelayConnectionStatus = Commander.Connect(Properties.Settings.Default.AlarmRelayDetails1, Convert.ToInt32(Properties.Settings.Default.AlarmRelayDetails2));

                        if (AlarmRelayConnectionStatus == false)
                            MessageBox.Show("Can not Connect to: " + Properties.Settings.Default.AlarmRelayDetails1 + " RelayPort: " + Properties.Settings.Default.AlarmRelayDetails2);
                    }
                }
                else if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "arduino" && !AlarmRelayConnectionStatus)
                {
                    try
                    {

                        if (Properties.Settings.Default.AlarmRelayDetails2 == Properties.Settings.Default.TrueRelayDetails2 && serialPort1.IsOpen)
                        {


                        }

                        else
                        {

                            if (serialPort1.IsOpen)
                                serialPort1.Close();
                            serialPort1.PortName = Properties.Settings.Default.AlarmRelayPort;
                            if (!(serialPort1.IsOpen))
                            {
                                serialPort1.Open();
                                serialPort1.BaudRate = 9600;
                                serialPort1.Parity = Parity.Even;
                                serialPort1.StopBits = StopBits.One;
                                serialPort1.DataBits = 8;
                                serialPort1.Handshake = Handshake.None;
                                serialPort1.RtsEnable = true;
                                //if ((serialPort1.IsOpen))
                                //    serialPort1.DataReceived += serialPort1_DataReceived;

                            }
                            else
                            {

                            }
                        }
                    }
                    catch (UnauthorizedAccessException exUnauthorizedAccess)
                    {
                    }
                }
            }
            else
                AlarmRelayConnectionStatus = false;

        }
        catch (Exception ex)
        {
            AlarmRelayConnectionStatus = false;
            VerifyRelayConnectionStatus = false;
            if (AlarmRelayConnectionStatus == false)
                MessageBox.Show("Can not Connect to: " + Properties.Settings.Default.AlarmRelayDetails1 + " RelayPort: " + Properties.Settings.Default.AlarmRelayDetails2 + "-ex: " + ex.ToString());

        }
    }


    private void GetGateSetting()
    {

        try
        {
            switch (KindShowProduct)
            {
                case 0:
                    pnl_show_TwoSeial.Visible = true;
                    pnl_show_OneProduct.Visible = false;
                    pnl_show_MultiProduct.Visible = false;
                    break;
                case 1:
                    pnl_show_TwoSeial.Visible = false;
                    pnl_show_OneProduct.Visible = true;
                    pnl_show_MultiProduct.Visible = false;
                    break;
                case 2:
                    pnl_show_TwoSeial.Visible = false;
                    pnl_show_OneProduct.Visible = false;
                    pnl_show_MultiProduct.Visible = true;
                    break;

            }

            lblGateNumber.Text = Properties.Settings.Default.GateNumber;
            lblGateTitle.Text = Properties.Settings.Default.GateTitle;
            lblFromStore.Text = Properties.Settings.Default.FromStoreTitle;
            lblToStore.Text = Properties.Settings.Default.ToStoreTitle;

            if (Properties.Settings.Default.AppReset == "1")
                timer_appReset.Enabled = true;

            lblReaderPowerSet.Text = Properties.Settings.Default.ReaderPower;
        }
        catch
        {

        }
    }


    private void StartAction(int AddSeconds)
    {
        try
        {
            this.Invoke(new Action(() => _StartActionDateTime = DateTime.Now.AddSeconds(AddSeconds)));
            this.Invoke(new Action(() => _FlagActionStart = true));

            _StartActionDateTime = DateTime.Now.AddSeconds(AddSeconds);
            _FlagActionStart = true;


            AddDetectedTagListWhenGateInSaveActionIntoMainTagList();

        }
        catch
        {

        }
    }

    private async void RefreshAction()
    {
        try
        {
            timer_checkSave.Enabled = false;
            Clear();

            // Cancel any ongoing operations and create a new token
            _cts.Cancel();
            _cts = new CancellationTokenSource();

            var token = _cts.Token;

            try
            {
                await Task.WhenAll(
                    Task.Run(() => SelectWarehouseMachines(token), token),
                    Task.Run(() => GetInventorySummaryByStoreCode(token), token)
                );
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
            }

            GetMaxActionId();
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }

    private void Clear()
    {
        try
        {


            FormLockANDInlock(false);
            _DocumentCheck = false;
            txtDocumentId.Text = "";
            _InSaveActionProgressActionId = "";
            _documentItem.Clear();
            GetDocumentDataFlag = false;
            dgvDocsList.Rows.Clear();
            dgvDocumentHeader.Rows.Clear();
            label13.Text = "اطلاعات و اقلام سند عطف عملیات: ";
            dgvDocumentItems.Rows.Clear();
            _FlagActionEnd = false;
            _RequireWmClear = false;
            GetDocumentDataFlag = false;
            _RequireClear = false;
            _FlagActionStart = false;
            pnlActionIdChange.Visible = false;
            pnlDocumentInfo.Visible = false;
            pnlShowSerials.Visible = false;
            pnlMassage.Visible = false;
            lblPlaque.Text = "";
            _actionStatus = ActionStatus.NoCargo;
            _actionDescription = "";
            lblPnlPart_DocumentId.Text = "";
            lblPnlPart_ProductTitle.Text = "";
            dgvSerialList.Rows.Clear();
            _InSaveActionProgress = false;
            _ErrorDetected = false;
            _RequireClear = false;
            this.BackColor = Color.White;
            lblLocationStore.Text = "";
            lblLocationCode.Text = "";
            _detectWM = false;
            lblMassage.Text = "";
            lblMassage.BackColor = Color.White;
            pnlMassage.Visible = false;
            this.BackColor = Color.White;
            lblActionId.Text = "";
            _ActionId = "";
            lblLocationCode.Text = "";
            txtDocumentId.Text = "";
            lblOneProduct_ProductCode.Text = lblOneProduct_ProductStatus.Text = lblOneProduct_ProductTitle.Text = lblOneProduct_TechnicalCode.Text = "";
            lblOneProduct_AllCount.Text = "0";
            lblOneProduct_AllSumValue.Text = "0";
            lblPnlPart_Count.Text = "0";
            lblPnlPart_DocumentId.Text = ""; lblOneProduct_ProductDocumentId.Text = "";
            dgvPnlPart.Rows.Clear();
            lblWMId.Text = lblWMTitle.Text = lblWMUserId.Text = "";
            dgvMutiProduct.Rows.Clear();
            lblMultiProductAllCount.Text = lblMultiProductAllSumValue.Text = "0";

            _GateResultList.Clear();
            _GateResultListFiltered.Clear();
            _GateResultListTajamoee.Clear();
            _UhfReaders.Refresh();
            MultiPartRowIndex = 1;
            lblTimerRemainToEnd.Text = "00:00";
            lblTimerActionDuration.Text = "00:00";
            RefreshDetectedTagList();

            foreach (Control cnt in pnlDynamicFeilds.Controls)
            {
                if (cnt.Name.Contains("dynamic"))
                    cnt.Text = "";
            }

            foreach (DynamicFieldDto _dto in _dynamicFieldDtos)
            {
                _dto.Value = "";
            }

            lblPnlTwoSerial_ProductCode1.Text = lblPnlTwoSerial_ProductCode2.Text = lblPnlTwoSerial_ProductCount1.Text = lblPnlTwoSerial_ProductCount2.Text = lblPnlTwoSerial_ProductInspectStatus1.Text = lblPnlTwoSerial_ProductInspectStatus2.Text = lblPnlTwoSerial_ProductRegCode1.Text = lblPnlTwoSerial_ProductRegCode2.Text = lblPnlTwoSerial_ProductSerial1.Text = lblPnlTwoSerial_ProductSerial2.Text = lblPnlTwoSerial_ProductStatus1.Text = lblPnlTwoSerial_ProductStatus2.Text = lblPnlTwoSerial_ProductTitle1.Text = lblPnlTwoSerial_ProductTitle2.Text = "";
            if (Properties.Settings.Default.StartActionKind == "3")
            {
                btnStart.Visible = true;
            }

            if (Properties.Settings.Default.EndActionKind == "4")
            {
                btnSave.Visible = true;
                //btnPrint.Visible=true;
                btnMultiPartSave.Visible = true;
                btnMultiPartDelete.Visible = true;

            }

            if (Properties.Settings.Default.KindShowProduct == 0)
            {
                btnShowSerials.Visible = false;
                btnRefreshDetectedTagList.Visible = false;
            }


            if (Properties.Settings.Default.ProductValueImportant == "1")
            {
                FlagControlsDocBySumValue = true;
                label38.Visible = true;
                lblOneProduct_AllSumValue.Visible = true;
                dgvMutiProductValue.Visible = true;
                label41.Visible = true;
                lblMultiProductAllSumValue.Visible = true;
            }
            else
            {
                label38.Visible = false;
                lblOneProduct_AllSumValue.Visible = false;
                dgvMutiProductValue.Visible = false;
                label41.Visible = false;
                lblMultiProductAllSumValue.Visible = false;

            }
        }
        catch
        {

        }
    }

    private void timer_Clock_Tick(object sender, EventArgs e)
    {
        try
        {

            if (_FlagActionStart && !_FlagActionEnd && Properties.Settings.Default.EndActionKind == "1" && _GateResultListFiltered.Count > 0)
            {
                var DifferenceSecond = Convert.ToInt32(Properties.Settings.Default.EndActionDetails) - (DateTime.Now - _LatestTrueTagReadDateTime).Seconds;

                if (DifferenceSecond > 0)
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + DifferenceSecond.ToString().PadLeft(2, '0')));
                }
                else
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + Convert.ToInt32(Properties.Settings.Default.EndActionDetails).ToString().PadLeft(2, '0')));

                }
            }
            else if (_FlagActionStart && !_FlagActionEnd && Properties.Settings.Default.EndActionKind == "5" && _GateResultListFiltered.Count > 0)
            {
                var DifferenceSecond = Convert.ToInt32(Properties.Settings.Default.EndActionDetails) - (DateTime.Now - _LatestTagReadDateTime).Seconds;

                if (DifferenceSecond > 0)
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + DifferenceSecond.ToString().PadLeft(2, '0')));
                }
                else
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + Convert.ToInt32(Properties.Settings.Default.EndActionDetails).ToString().PadLeft(2, '0')));

                }
            }
            else if (_FlagActionStart && !_FlagActionEnd && (Properties.Settings.Default.EndActionKind == "0"))
            {
                var DifferenceSecond = Convert.ToInt32(Properties.Settings.Default.EndActionDetails) - (DateTime.Now - _StartActionDateTime).Seconds;

                if (DifferenceSecond > 0)
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + DifferenceSecond.ToString().PadLeft(2, '0')));
                }
                else
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + Convert.ToInt32(Properties.Settings.Default.EndActionDetails).ToString().PadLeft(2, '0')));
                }

            }
            else if (_FlagActionStart && !_FlagActionEnd && (Properties.Settings.Default.EndActionKind == "3"))
            {
                var DifferenceSecond = Convert.ToInt32(Properties.Settings.Default.StartActionDetails) + Convert.ToInt32(Properties.Settings.Default.EndActionDetails) - (DateTime.Now - _StartActionDateTime).Seconds;

                if (DifferenceSecond > 0)
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + DifferenceSecond.ToString().PadLeft(2, '0')));
                }
                else
                {
                    this.Invoke(new Action(() => lblTimerRemainToEnd.Text = "00:" + Convert.ToInt32(Properties.Settings.Default.EndActionDetails).ToString().PadLeft(2, '0')));
                }

            }

            if (_FlagActionStart && !_FlagActionEnd && Properties.Settings.Default.EndActionKind != "3")
            {
                var DifferenceSecond = ((DateTime.Now - _StartActionDateTime).Minutes * 60) + (DateTime.Now - _StartActionDateTime).Seconds;


                this.Invoke(new Action(() => lblTimerActionDuration.Text = Tools.TimeTools.Time.ConvertMinToHour(DifferenceSecond)));

            }


            if (_ErrorDetected && DateTime.Now >= _ErrorDetectedDateTime.AddSeconds(12))
            {
                if (Properties.Settings.Default.EndActionKind != "4")
                {
                    RefreshAction();
                }
            }

            if (_RequireClear && !_ErrorDetected && DateTime.Now >= _RequireClearDetectedDateTime.AddSeconds(10))
            {
                if (Properties.Settings.Default.EndActionKind != "4")
                {

                    RefreshAction();
                }
            }

            if (_RequireWmClear && !_ErrorDetected && DateTime.Now >= _RequireWMClearDetectedDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.StartActionDetails)) && _GateResultListFiltered.Count == 0)
            {
                if (Properties.Settings.Default.EndActionKind != "4")
                {


                    RefreshAction();
                }
            }

            if (Properties.Settings.Default.StartActionKind == "0" && !_ErrorDetected && _FlagActionEnd && DateTime.Now >= _EndActionDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.StartActionDetails)))
            {


                RefreshAction();

            }

            if (!_InSaveActionProgress && Properties.Settings.Default.EndActionKind == "0" && _FlagActionStart && _GateResultListFiltered.Count > 0 && !_ErrorDetected && DateTime.Now >= _StartActionDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.EndActionDetails)))
            {

                FinishANDSave();
            }

            if (!_InSaveActionProgress && Properties.Settings.Default.EndActionKind == "1" && _FlagActionStart && _GateResultListFiltered.Count > 0 && !_ErrorDetected && DateTime.Now >= _LatestTrueTagReadDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.EndActionDetails)))
            {
                FinishANDSave();
            }


            if (!_InSaveActionProgress && Properties.Settings.Default.EndActionKind == "5" && _FlagActionStart && _GateResultListFiltered.Count > 0 && !_ErrorDetected && DateTime.Now >= _LatestTagReadDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.EndActionDetails)))
            {
                FinishANDSave();
            }

            if (!_InSaveActionProgress && Properties.Settings.Default.EndActionKind == "3" && !_InSaveActionProgress && _GateResultListFiltered.Count > 0 && _detectWM && _FlagActionStart && !_ErrorDetected && DateTime.Now >= _WMDetectedDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.EndActionDetails)))
            {
                FinishANDSave();
            }
            if (_FlagActionStart && (((DateTime.Now - _StartActionDateTime).Minutes * 60) + (DateTime.Now - _StartActionDateTime).Seconds) > 360)
            {
                if (Properties.Settings.Default.EndActionKind != "4")
                {

                    RefreshAction();
                }
            }


            lblTime.Text = DateTime.Now.ToLongTimeString();
            lblDate.Text = Tools.TimeTools.Time.PersinaNowDate();
            lblDayName.Text = Tools.TimeTools.Time.GetDayNameFromDateTime(DateTime.Now);
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private void AddDetectedTagListWhenGateInSaveActionIntoMainTagList()
    {
        if (Properties.Settings.Default.EndActionKind != "4" && _DetectedTagListWhenGateInSaveAction.Count>0)
        {
            try
            {
                foreach (Tags _tag in _DetectedTagListWhenGateInSaveAction)
                {
                    if (lblActionId.Text != "" && _ActionId != "")
                    {
                        lblConnectToReaderStatus.BackColor = Color.Green;
                        _tag.TagPackageId = MultiPartRowIndex;
                        this.Invoke(new Action(() => _LatestTagReadDateTime = DateTime.Now));

                        _LatestTagReadDateTime = DateTime.Now;
                        if (_DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == _tag.TagEPC.ToLower()) == null)
                        {
                            _tag.TagReedTime = DateTime.Now;
                            _tag.TagActionStatus = 0;
                            _tag.TagReedSaveStatus = 0;
                            _tag.TagReedUpdateStatus = 0;
                            _tag.TagPackageId = MultiPartRowIndex;
                            _tag.DocumentId = lblPnlPart_DocumentId.Text;
                            _tag.WMUsertId = lblWMUserId.Text;


                            _DetectedTagList.Add(_tag);
                        }

                    }
                }


                lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/" + _DetectedTagListWhenGateInSaveAction.Count.ToString();
                this.Invoke(new Action(() => lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/" + _DetectedTagListWhenGateInSaveAction.Count.ToString()));

                _DetectedTagListWhenGateInSaveAction.Clear();
                this.Invoke(new Action(() => _DetectedTagListWhenGateInSaveAction.Clear()));
                lblCountTagWhenInSaveAction.Text = "0";

            }
            catch
            {

            }
        }
    }


    private async Task GetMaxActionId()
    {
        try
        {
            // _hubconnecton.InvokeAsync("GetActionId", GateNumber).GetAwaiter().GetResult();
            var api = new ApiBusiness();
            _ActionId = (await api.GetNextIdByGateCode());
            if (_ActionId == "0" || _ActionId == "")
            {
                Thread.Sleep(6000);
                _ActionId = (await api.GetNextIdByGateCode());
                if (_ActionId == "0" || _ActionId == "")
                {
                    Thread.Sleep(6000);
                    _ActionId = (await api.GetNextIdByGateCode());

                    if (_ActionId == "0" || _ActionId == "")
                    {
                        Thread.Sleep(6000);
                        _ActionId = (await api.GetNextIdByGateCode());
                        if ((_ActionId == "0" || _ActionId == "" ) )
                        {

                            Thread.Sleep(6000);
                            _FlagResetApp = true;
                            try
                            {
                                _UhfReaders.Stop();
                                _UhfReaders.DisConnect();
                            }
                            catch
                            {

                            }
                            //Application.Restart();
                            //Environment.Exit(0);
                        }
                    }
                }
            }
            if (_ActionId != "0" && _ActionId != "")
            {
                this.Invoke(new Action(() => lblActionId.Text = _ActionId));
                this.Invoke(new Action(() => lblDtGetMaxActionId.Text = DateTime.Now.ToString()));

                AddDetectedTagListWhenGateInSaveActionIntoMainTagList();
            }

        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        RefreshAction();
        StartAction(0);
    }

    private void label23_Click(object sender, EventArgs e)
    {

    }




    private void timer_GetProductDatList_Tick(object sender, EventArgs e)
    {
        GetProductDataList(_ActionId);
    }


    private async Task GetProductDataList(string _actionId)
    {
        try
        {
            if (_actionId == "")
                return;
            if (Properties.Settings.Default.EndActionKind == "3")
            {
                if (!_detectWM)
                    return;
            }
            if (_InSaveActionProgress)
                return;
            if (_DetectedTagList.Count == 0)
                return;
            
             _DetectedTagList.ForEach(p => { p.TagBeforSendStatus = 1; });
            var _TempGateResultList = await _apiBusiness.SSaveGateLogAndShowResult(_DetectedTagList, _actionId, ActionType.ToString());
            foreach (var tgd in _DetectedTagList)
            {
                if (tgd.TagBeforSendStatus == 1)
                    tgd.TagReedSaveStatus = 1;
            }


            if (_TempGateResultList != null && !_InSaveActionProgress)
            {
                foreach (GateResult _tag in _TempGateResultList)
                {
                     if (_GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial) == null)
                    {
                        _tag.TagGateResultStatus = "";
                        _tag.TagPackageId = (Properties.Settings.Default.TagDetectInMultiPart == "0") ? 0 : MultiPartRowIndex;
                        _tag.TagPackageStatus = (_tag.TagPackageStatus == 1) ? 1 : ((Properties.Settings.Default.TagDetectInMultiPart == "0") ? 1 : 0);
                        _tag.TagGateReadTime = DateTime.Now;
                        _GateResultList.Add(_tag);
                    }
                    else
                    {
                        if (_GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial).ProductSerial != _tag.ProductSerial || _GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial).TagInDestinationId != _tag.TagInDestinationId)// تگ  بعد از شناسایی گیت رجیستر شده
                        {
                            _GateResultList = _GateResultList.Where(p => p.TagSerial != _tag.TagSerial).ToList();
                            _GateResultListFiltered = _GateResultListFiltered.Where(p => p.TagSerial != _tag.TagSerial).ToList();
                            _tag.TagGateResultStatus = "";
                            _tag.TagPackageId = (Properties.Settings.Default.TagDetectInMultiPart == "0") ? 0 : MultiPartRowIndex;
                            _tag.TagPackageStatus = (_tag.TagPackageStatus == 1) ? 1 : ((Properties.Settings.Default.TagDetectInMultiPart == "0") ? 1 : 0);
                            _tag.TagGateReadTime = DateTime.Now;
                            _GateResultList.Add(_tag);
                        }

                    }
                }
            }
            // صرفا حذف این تگ در حالت ذخیره نشده کافیست که چندباره ارسال بشه

            foreach (var tgd in _DetectedTagList)
            {
                if (tgd.TagReedSaveStatus == 1 && tgd.TagActionStatus == 0 && _GateResultList.FirstOrDefault(p => p.TagSerial.ToLower() == tgd.TagEPC.ToLower()) == null)
                {
                    tgd.TagActionStatus = 3;
                    tgd.TagReedSaveStatus = 0;
                }


                //if (_DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == tgd.TagEPC.ToLower()) != null)
                //{
                //    _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == tgd.TagEPC.ToLower()).TagReedSaveStatus = 1;

                //    if (tgd.TagActionStatus == 3 && _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == tgd.TagEPC.ToLower()).TagActionStatus!=3)
                //    {
                //        _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == tgd.TagEPC.ToLower()).TagActionStatus = 3;
                //        _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == tgd.TagEPC.ToLower()).TagReedSaveStatus = 0;

                //    }
                //}

            }
            // :  حلقه بنداز و فقط TagReedSaveStatus و   TagActionStatus  را ویرایش کن
            //_DetectedTagList = _TempDetectedTagList;




            ShowProduct();
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }


    }



    private string TagFiltering(GateResult _tag)
    {

        string _TagFilterDesc = "";
        bool TagFilterStatus = true;
        string AlarmDesc = "";
        string AlarmTypeDesc = "";


        if (_tag.ProductSerial != "")// کنترل عبور تگ رجیستر نشده
        {
            if (Properties.Settings.Default.IgnoringNonExistentGoods == "1")//چک کنترل موجود کالا در انبار مبدأ
            {
                try
                {
                    bool FlagCheckFromStore = false;
                    foreach (string FromStoreCode in Properties.Settings.Default.FromStore.Split(','))
                    {
                        if (_tag.TagInDestinationId == FromStoreCode)
                        {
                            FlagCheckFromStore = true;
                            break;
                        }
                    }

                    if (FlagCheckFromStore)
                    {
                        TagFilterStatus = true;
                    }
                    else
                    {
                        if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                        {
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 2;
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                        }
                        TagFilterStatus = false;
                        _TagFilterDesc = "Ignoring";
                    }
                }
                catch
                {
                    TagFilterStatus = false;
                    _TagFilterDesc = "Ignoring";

                }
            }


            if (Properties.Settings.Default.IgnoreSerials.Contains(_tag.ProductSerial))
            {
                TagFilterStatus = false;
                _TagFilterDesc = "Ignoring";
            }


            if (Properties.Settings.Default.IgnoringTagAgeThenLess == "1")//کنترل عبور تگ ها با عمر کمتر از یک مقدار مشخص
            {
                try
                {
                    if (_tag.TagRegisterDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.IgnoringTagAgeThenLessDetails)) >= DateTime.Now)
                    {
                        if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                        {
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 7;
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                        }
                        TagFilterStatus = false;
                        _TagFilterDesc = "Ignoring";
                    }
                }
                catch
                {
                    TagFilterStatus = false;
                    _TagFilterDesc = "Ignoring";

                }
            }




            if (_tag.ProductSerial != "" && Properties.Settings.Default.AtentionForFreezedTag == "1" && _TagFilterDesc != "Ignoring")// کنترل تگ فریز شده
            {
                try
                {
                    if (_tag.Freeze == "1")
                    {
                        TagFilterStatus = false;
                        if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                        {
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 4;
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                        }
                        AlarmDesc = "شناسایی کالای فریز شده - سریال کالا : " + _tag.ProductSerial + " عنوان کالا : " + _tag.ProductName;
                        AlarmTypeDesc = "شناسایی کالای فریز شده";

                        //     Alarm("Error",, , _tag.TagSerial, _tag.ProductSerial);
                        _TagFilterDesc = "تگ فریز شده";

                    }

                }
                catch
                {
                    TagFilterStatus = false;
                }
            }


            if (_tag.ProductSerial != "" && Properties.Settings.Default.AtentionForNotQCTag == "1" && _TagFilterDesc != "Ignoring")// کنترل تگ با بازرسی مردود
            {
                try
                {
                    if (_tag.Lock == "True")
                    {
                        TagFilterStatus = false;
                        if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                        {
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 5;
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                        }

                        AlarmDesc = "شناسایی کالای با وضعیت بازرسی مردود - سریال کالا : " + _tag.ProductSerial + " عنوان کالا : " + _tag.ProductName;
                        AlarmTypeDesc = "شناسایی کالای با وضعیت بازرسی مردود";

                        //     Alarm("Error", , , _tag.TagSerial, _tag.ProductSerial);
                        _TagFilterDesc = "تگ بازرسی مردود";

                    }

                }
                catch
                {
                    TagFilterStatus = false;
                }
            }

            if (_tag.ProductSerial != "" && Properties.Settings.Default.AtentionForWithOutQCTag == "1" && _TagFilterDesc != "Ignoring")// کنترل تگ بدون بازرسی
            {
                try
                {
                    if (_tag.LastInspectResult == "[]")
                    {
                        TagFilterStatus = false;
                        if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                        {
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 6;
                            _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                        }
                        AlarmDesc = "شناسایی کالای با وضعیت بازرسی نشده - سریال کالا : " + _tag.ProductSerial + " عنوان کالا : " + _tag.ProductName;
                        AlarmTypeDesc = "شناسایی کالای با وضعیت بازرسی نشده";

                        //  Alarm("Error", "شناسایی کالای با وضعیت بازرسی نشده - سریال کالا : " + _tag.ProductSerial + " عنوان کالا : " + _tag.ProductName, "شناسایی کالای با وضعیت بازرسی نشده", _tag.TagSerial, _tag.ProductSerial);
                        _TagFilterDesc = "تگ بازرسی نشده";

                    }

                }
                catch
                {
                    TagFilterStatus = false;
                }
            }

        }
        else
        {
            if (Properties.Settings.Default.AtentionForNotRegisteredTag == "1")//هشدار برای تگ رجیستر نشده
            {
                try
                {
                    TagFilterStatus = false;
                    if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                    {
                        _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 3;
                        _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                    }
                    AlarmDesc = "تگ رجیستر نشده شناسایی شده است";
                    AlarmTypeDesc = "شناسایی تگ رجیستر نشده";

                    //Alarm("Error", "تگ رجیستر نشده شناسایی شده است", "شناسایی تگ رجیستر نشده", _tag.TagSerial, "");
                    _TagFilterDesc = "تگ رجیستر نشده";
                }
                catch
                {
                    TagFilterStatus = false;
                    _TagFilterDesc = "تگ رجیستر نشده";

                }
            }
            else// نادیده گرفتن تگ رجیستر نشده
            {
                if (_DetectedTagList.Count > 0 && _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial) != null)
                {
                    _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 2;
                    _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagReedSaveStatus = 0;

                }
                TagFilterStatus = false;
                _TagFilterDesc = "Ignoring";

            }
        }


        if (_TagFilterDesc != "Ignoring")
        {
            if (Properties.Settings.Default.StartActionKind == "0")
            {
                if (_GateResultListFiltered.Count == 0)
                {
                    StartAction(0);
                }
            }
            this.Invoke(new Action(() => _LatestTrueTagReadDateTime = DateTime.Now));
            _LatestTrueTagReadDateTime = DateTime.Now;
        }

        if (!_FlagActionStart)
        {
            _TagFilterDesc = "Ignoring";
        }
        else
        {
            if (AlarmDesc != "")
                Alarm("Error", AlarmDesc, AlarmTypeDesc, _tag.TagSerial, _tag.ProductSerial);
        }
        return _TagFilterDesc;

    }


    private async void SendCargoTruckSignal()
    {
        try
        {
            if (lblWMUserId.Text != "")
                await _apiBusiness.SendCargoTruckSignal(lblActionId.Text, lblWMUserId.Text, lblWMId.Text, _actionDescription, _actionStatus);
        }
        catch
        {

        }
    }
    private void ShowProduct()
    {
        try
        {
            if (_GateResultList != null && _GateResultList.Count > 0 && !_InSaveActionProgress)
            {

                bool FlagAlarm = false;

                foreach (GateResult _tag in _GateResultList)
                {
                    if (_tag.TagGateResultStatus == "")
                    {
                        var _TagFilterDesc = "";
                        _TagFilterDesc = TagFiltering(_tag);
                        if (_FlagActionStart)
                        {
                            if (_TagFilterDesc == "")
                            {
                                _tag.TagGateResultStatus = "تأیید";
                                if (_FlagActionStart && _GateResultListFiltered.FirstOrDefault(p => p.TagSerial == _tag.TagSerial) == null)
                                    _GateResultListFiltered.Add(_tag);

                            }
                            else if (_TagFilterDesc != "Ignoring")
                            {
                                FlagAlarm = true;
                                _tag.TagGateResultStatus = _TagFilterDesc;
                                if (_GateResultListFiltered.FirstOrDefault(p => p.TagSerial == _tag.TagSerial) == null)
                                    _GateResultListFiltered.Add(_tag);

                            }
                            if (_GateResultListFiltered.Count > 0)
                            {
                                lblLocationStore.Text = _GateResultListFiltered[0].PMToStoreTitle;
                                lblLocationCode.Text = _GateResultListFiltered[0].PMToZoneCode;
                            }
                        }
                    }
                }
                if (_GateResultListFiltered.Count > 0 && _FlagActionStart)
                {

                    _RequireClear = false;

                    switch (KindShowProduct)
                    {
                        case 0://one Or Two Serial

                            if (_GateResultListFiltered.Count >= 1)
                            {
                                lblPnlTwoSerial_ProductCode1.Text = _GateResultListFiltered[0].ProductCode.ToString();
                                lblPnlTwoSerial_ProductCount1.Text = _GateResultListFiltered[0].Count.ToString();
                                lblPnlTwoSerial_ProductRegCode1.Text = _GateResultListFiltered[0].ProductTechnicalCode.ToString();
                                lblPnlTwoSerial_ProductSerial1.Text = _GateResultListFiltered[0].ProductSerial.ToString();
                                lblPnlTwoSerial_ProductStatus1.Text = _GateResultListFiltered[0].ProductStatus.ToString();
                                lblPnlTwoSerial_ProductTitle1.Text = _GateResultListFiltered[0].ProductName.ToString();
                                lblPnlTwoSerial_ProductInspectStatus1.Text = (_GateResultListFiltered[0].LastInspectResult.ToString() != "[]") ? (_GateResultListFiltered[0].Lock.ToString() == "True") ? "مردود" : "تأیید" : "بازرسی نشده";



                            }
                            if (_GateResultListFiltered.Count > 1)
                            {
                                lblPnlTwoSerial_ProductCode2.Text = _GateResultListFiltered[1].ProductCode.ToString();
                                lblPnlTwoSerial_ProductCount2.Text = _GateResultListFiltered[1].Count.ToString();
                                lblPnlTwoSerial_ProductRegCode2.Text = _GateResultListFiltered[1].ProductTechnicalCode.ToString();
                                lblPnlTwoSerial_ProductSerial2.Text = _GateResultListFiltered[1].ProductSerial.ToString();
                                lblPnlTwoSerial_ProductStatus2.Text = _GateResultListFiltered[1].ProductStatus.ToString();
                                lblPnlTwoSerial_ProductTitle2.Text = _GateResultListFiltered[1].ProductName.ToString();
                                lblPnlTwoSerial_ProductInspectStatus2.Text = (_GateResultListFiltered[1].LastInspectResult.ToString() != "[]") ? (_GateResultListFiltered[1].Lock.ToString() == "True") ? "مردود" : "تأیید" : "بازرسی نشده";

                            }


                            break;
                        case 1: // One ProductCode

                            _GateResultListTajamoee = ComputeTajamoee(_GateResultListFiltered, KindShowProduct);
                            var OneProduct = _GateResultListTajamoee.FirstOrDefault();
                            if (OneProduct != null)
                            {
                                lblOneProduct_TechnicalCode.Text = OneProduct.ProductTechnicalCode;
                                lblOneProduct_ProductCode.Text = OneProduct.ProductCode;
                                lblOneProduct_ProductStatus.Text = OneProduct.ProductStatus;
                                lblOneProduct_ProductTitle.Text = OneProduct.ProductName;
                                lblOneProduct_AllCount.Text = OneProduct.Count;
                                lblOneProduct_AllSumValue.Text = OneProduct.SumValue;
                                lblOneProduct_ProductDocumentId.Text = OneProduct.DocumentId;
                            }
                            if (Properties.Settings.Default.TagDetectInMultiPart == "1")// MultiPart
                            {
                                lblPnlPart_Count.Text = _GateResultListFiltered.Where(p => p.TagPackageStatus == 0 && p.TagGateResultStatus == "تأیید").Count().ToString();
                                lblPnlPart_DocumentId.Text = (_GateResultListFiltered.Where(p => p.TagPackageStatus == 0 && p.TagGateResultStatus == "تأیید").ToList().Count > 0) ? _GateResultListFiltered.Where(p => p.TagPackageStatus == 0).First().DocumentId : "";
                                lblPnlPart_ProductTitle.Text = (_GateResultListFiltered.Where(p => p.TagPackageStatus == 0 && p.TagGateResultStatus == "تأیید").ToList().Count > 0) ? _GateResultListFiltered.Where(p => p.TagPackageStatus == 0).First().ProductTechnicalCode : "";
                            }

                            break; // Multi ProductCode
                        case 2:
                            _GateResultListTajamoee = ComputeTajamoee(_GateResultListFiltered, KindShowProduct);
                            double SumAllValue = 0;
                            double SumAllCount = 0;
                            if (_GateResultListTajamoee != null)
                            {
                                foreach (GateResult _tajamoee in _GateResultListTajamoee)
                                {
                                    bool ExistIndgv = false;
                                    foreach (DataGridViewRow dr in dgvMutiProduct.Rows)
                                    {
                                        if (dr.Cells["dgvMutiProductProductCode"].Value.ToString() == _tajamoee.ProductCode)
                                        {
                                            dr.Cells["dgvMutiProductCount"].Value = _tajamoee.Count.ToString();
                                            dr.Cells["dgvMutiProductValue"].Value = _tajamoee.SumValue.ToString();
                                            ExistIndgv = true;
                                            break;
                                        }
                                    }
                                    if (!ExistIndgv)
                                        dgvMutiProduct.Rows.Add(dgvMutiProduct.Rows.Count + 1, _tajamoee.ProductCode, _tajamoee.ProductTechnicalCode, _tajamoee.ProductName, _tajamoee.DocumentId, _tajamoee.Count, _tajamoee.SumValue);
                                    SumAllValue += Convert.ToDouble(_tajamoee.SumValue);
                                    SumAllCount += Convert.ToDouble(_tajamoee.Count);
                                }
                                lblMultiProductAllCount.Text = SumAllCount.ToString();
                                lblMultiProductAllSumValue.Text = SumAllValue.ToString();

                                if (_GateResultListTajamoee.Count < dgvMutiProduct.Rows.Count)
                                {
                                    foreach (DataGridViewRow dr in dgvMutiProduct.Rows)
                                    {
                                        bool FlagCheck = false;
                                        foreach (GateResult _tajamoee in _GateResultListTajamoee)
                                        {
                                            if (dr.Cells["dgvMutiProductProductCode"].Value.ToString() == _tajamoee.ProductCode)
                                            {
                                                FlagCheck = true;
                                                break;
                                            }
                                        }

                                        if (!FlagCheck)
                                        {
                                            dgvMutiProduct.Rows.Remove(dr);
                                            break;
                                        }
                                    }
                                }
                            }
                            if (Properties.Settings.Default.TagDetectInMultiPart == "1")// MultiPart
                            {
                                lblPnlPart_Count.Text = _GateResultListFiltered.Where(p => p.TagPackageStatus == 0 && p.TagGateResultStatus == "تأیید").Count().ToString();
                                //  lblPnlPart_DocumentId.Text=(_GateResultListTajamoee[0].DocumentId!=null)?_GateResultListTajamoee[0].DocumentId:"";
                            }
                            break;
                    }
                    if (!FlagAlarm && _GateResultListFiltered.Where(p => p.TagGateResultStatus != "تأیید").ToList().Count == 0)
                    {
                        _actionStatus = ActionStatus.InOperation;
                        this.BackColor = Color.White;
                        pnlMassage.Visible = false;
                        FormLockANDInlock(false);
                    }
                }



                CheckGateResultByDocumentItem();

                try
                {
                    var tdg = _DetectedTagList.LastOrDefault();
                    var _tempGateResult = _GateResultList.FirstOrDefault(p => p.TagSerial == tdg.TagEPC);
                    if (!_FlagActionStart && tdg.TagActionStatus == 2 && !_ErrorDetected)
                        lblLocationCode.Text = _tempGateResult.ProductSerial;
                    else
                        lblLocationCode.Text = "";
                }
                catch
                {
                    lblLocationCode.Text = "";
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private void CheckGateResultByDocumentItem()
    {
        try
        {
            if (_FlagActionStart && Properties.Settings.Default.RequirmentDocument == "1" && txtDocumentId.Text.Trim() != "" && Properties.Settings.Default.ControlByDocument == "1" && _documentItem.Count > 0)
            {
                _DocumentCheck = true;
                foreach (GateResult _gateresultjamoee in _GateResultListTajamoee)
                {
                    if (_documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode) == null)
                    {
                        _gateresultjamoee.DocumentCheckStatusDesc = "مغایرت-شناسایی کالای نادرست";
                        _DocumentCheck = false;
                    }
                    else
                    {
                        if (Convert.ToDecimal( _documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode).Count) != Convert.ToDecimal(((FlagControlsDocBySumValue) ? _gateresultjamoee.SumValue : _gateresultjamoee.Count)))
                        {
                            decimal difference =  Convert.ToDecimal(((FlagControlsDocBySumValue) ? _gateresultjamoee.SumValue:  _gateresultjamoee.Count)) -Convert.ToDecimal( _documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode).Count);
                            _DocumentCheck = false;

                            if (difference > 0)
                                _gateresultjamoee.DocumentCheckStatusDesc = "مغایرت تعدادی " + _documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode).Count.ToString() + " -  " + difference.ToString() + " اضافی";
                            else
                                _gateresultjamoee.DocumentCheckStatusDesc = "مغایرت تعدادی " + _documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode).Count.ToString() + " -  " + difference.ToString() + " کسری";

                            _documentItem.FirstOrDefault(p => p.ProductCode == _gateresultjamoee.ProductCode).StatusDesc = _gateresultjamoee.DocumentCheckStatusDesc;
                        }
                        else
                        {
                            _gateresultjamoee.DocumentCheckStatusDesc = "";

                        }
                    }
                }

                foreach (DocumentItem _docitem in _documentItem)
                {
                    if (_GateResultListTajamoee.FirstOrDefault(p => p.ProductCode == _docitem.ProductCode) == null)
                    {
                        _docitem.StatusDesc = "مغایرت-کالا شناسایی نشده";
                        _DocumentCheck = false;
                    }

                }


                foreach (DataGridViewRow _dr in dgvMutiProduct.Rows)
                {
                    string checkStatusDesc = _GateResultListTajamoee.FirstOrDefault(p => p.ProductCode == _dr.Cells["dgvMutiProductProductCode"].Value.ToString()).DocumentCheckStatusDesc;
                    if (checkStatusDesc != "")
                    {
                        _dr.Cells["colMultiProductStatus"].Value = checkStatusDesc;
                        _dr.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        _dr.Cells["colMultiProductStatus"].Value = "تأیید";
                        _dr.DefaultCellStyle.BackColor = Color.White;

                    }
                }



                foreach (DataGridViewRow _dr in dgvDocumentItems.Rows)
                {
                    string checkStatusDesc = (_GateResultListTajamoee.FirstOrDefault(p => p.ProductCode == _dr.Cells["colDocumentItemProductCode"].Value.ToString()) == null) ? "مغایرت با شناسایی - کالا شناسایی نشده" : _GateResultListTajamoee.FirstOrDefault(p => p.ProductCode == _dr.Cells["colDocumentItemProductCode"].Value.ToString()).DocumentCheckStatusDesc;
                    _dr.Cells["colDocumentItemStatus"].Value = checkStatusDesc;
                    if (checkStatusDesc == "مغایرت با شناسایی - کالا شناسایی نشده")
                    {
                        _dr.Cells["colDocumentItemStatus"].Value = checkStatusDesc;
                        _dr.DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        //var GateResulcheckStatusDesc = _GateResultListTajamoee.FirstOrDefault(p => p.ProductCode == _dr.Cells["colDocumentItemProductCode"].Value.ToString()).DocumentCheckStatusDesc;
                        //_dr.Cells["colDocumentItemStatus"].Value = (GateResulcheckStatusDesc == "") ? "تأیید" : GateResulcheckStatusDesc;

                        if (checkStatusDesc.Contains("مغایرت"))
                            _dr.DefaultCellStyle.BackColor = Color.Yellow;
                        else
                            _dr.DefaultCellStyle.BackColor = Color.White;
                    }

                }

            }
            else
            {
                if (Properties.Settings.Default.RequirmentDocument != "1" && Properties.Settings.Default.ControlByDocument != "1")
                {
                    _DocumentCheck = true;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private void Alarm(string AlarmType, string AlarmDesc, string AlarmTypeDesc, string AlarmTagEpc, string AlarmProductSerial)
    {
        try
        {
            pnlMassage.Visible = true;
            if (AlarmType == "ErrorWithOutLockForm")
            {
                _actionStatus = ActionStatus.Error;
                lblMassage.BackColor = Color.Red;
                this.BackColor = Color.Red;

                PlaySound("ErrorWithOutLockForm");
                SendToRelay("ErrorWithOutLockForm");

            }
            if (AlarmType == "Error")
            {
                FormLockANDInlock(true);
                foreach (var dttag in _DetectedTagList)
                {
                    if (dttag.TagActionStatus == 0)
                    {
                        dttag.TagActionStatus = 1;
                        dttag.TagReedSaveStatus = 0;
                    }
                }
                _actionStatus = ActionStatus.Error;
                lblMassage.BackColor = Color.Red;
                this.BackColor = Color.Red;
                _ErrorDetectedDateTime = DateTime.Now;
                _ErrorDetected = true;

                PlaySound("Error");
                SendToRelay("Error");



            }
            else if (AlarmType == "NotError")
            {
                FormLockANDInlock(true);

                _RequireClear = true;
                _RequireClearDetectedDateTime = DateTime.Now;
                lblMassage.BackColor = Color.Green;
                if (Properties.Settings.Default.KindShowProduct == 0 && lblPnlTwoSerial_ProductSerial1.Text != "" && lblPnlTwoSerial_ProductSerial2.Text != "")
                {
                    PlaySound("TwoActionVerify");
                    SendToRelay("TwoActionVerify");

                }
                else
                {
                    PlaySound("ActionVerify");
                    SendToRelay("ActionVerify");

                }


            }
            _actionDescription = AlarmDesc;

            lblMassage.Text = AlarmDesc;


            if (AlarmTypeDesc != "")
            {

                _apiBusiness.SaveAlarmLog(AlarmTypeDesc, AlarmTagEpc, AlarmProductSerial, lblActionId.Text, lblWMUserId.Text);

            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }


    private void SendToRelay(string TypeRelayAction)
    {
        switch (TypeRelayAction)
        {
            case "ActionVerify":
                if (Properties.Settings.Default.TrueRelay == "1")
                {
                    if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "timax")
                    {
                        try
                        {
                            if (VerifyRelayConnectionStatus)
                            {
                                byte RelayID = 1;
                                byte Value = 1;
                                byte Duration = 6; // in second (pass 0 to ignore it)
                                string Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                CUDP.ResultCode Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);

                                RelayID = 2;
                                Value = 1;
                                Duration = 1; // in second (pass 0 to ignore it)
                                Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);

                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }

                    }
                    else if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "arduino")
                    {
                        try
                        {
                            serialPort1.Write("5");
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                }
                break;

            case "TwoActionVerify":
                if (Properties.Settings.Default.TrueRelay == "1")
                {
                    if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "timax")
                    {
                        try
                        {
                            if (VerifyRelayConnectionStatus)
                            {
                                byte RelayID = 1;
                                byte Value = 1;
                                byte Duration = 6; // in second (pass 0 to ignore it)
                                string Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                CUDP.ResultCode Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);

                                RelayID = 2;
                                Value = 1;
                                Duration = 1; // in second (pass 0 to ignore it)
                                Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);
                                Thread.Sleep(1000);
                                RelayID = 2;
                                Value = 1;
                                Duration = 1; // in second (pass 0 to ignore it)
                                Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);

                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }

                    }
                    else if (Properties.Settings.Default.TrueRelayDetails4.ToLower() == "arduino")
                    {
                        try
                        {
                            serialPort1.Write("4");
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                }
                break;

            case "Error":
                if (Properties.Settings.Default.AlarmRelay == "1")
                {
                    if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "timax")
                    {
                        try
                        {
                            if (VerifyRelayConnectionStatus)
                            {
                                byte RelayID = 2;
                                byte Value = 1;
                                byte Duration = 4; // in second (pass 0 to ignore it)
                                string Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                CUDP.ResultCode Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                    else if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "arduino")
                    {
                        try
                        {
                            serialPort1.Write("7");
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                }
                break;

            case "ErrorWithOutLockForm":
                if (Properties.Settings.Default.AlarmRelay == "1")
                {
                    if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "timax")
                    {
                        try
                        {
                            if (VerifyRelayConnectionStatus)
                            {
                                byte RelayID = 2;
                                byte Value = 1;
                                byte Duration = 4; // in second (pass 0 to ignore it)
                                string Password = Properties.Settings.Default.TrueRelayDetails3; // 10 characters communication password (set "" if not used)

                                CUDP.ResultCode Result = Commander.SetRelay(RelayID, Value, Duration, Password, Properties.Settings.Default.TrueRelayDetails1, Convert.ToInt16(Properties.Settings.Default.TrueRelayDetails2), true, 2000, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                    else if (Properties.Settings.Default.AlarmRelayDetails4.ToLower() == "arduino")
                    {
                        try
                        {
                            serialPort1.Write("7");
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.WriteExceptionLogs(ex);
                        }
                    }
                }
                break;
        }
    }//
    private async void ShowWMInfo(string WMEPC)
    {
        try
        {
            if (!_detectWM)
            {
                foreach (WarehouseMachines wm in WarehouseMachineslist)
                {
                    if (wm.WMRFID == WMEPC)
                    {
                        if (!_detectWM)
                        {
                            _detectWM = true;
                            this.Invoke(new Action(() => _WMDetectedDateTime = DateTime.Now));

                            if (Properties.Settings.Default.StartActionKind == "1")
                            {
                                StartAction(-1 * Convert.ToInt32(Properties.Settings.Default.StartActionDetails));
                                foreach (Tags _tag in _DetectedTagList)
                                {
                                    if (_tag.TagReedTime >= _WMDetectedDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.StartActionDetails) * -1))
                                    {
                                        _tag.TagActionStatus = 0;
                                        _tag.TagReedSaveStatus = 0;
                                    }
                                }


                                foreach (GateResult _tag in _GateResultList)
                                {
                                    if (_tag.TagGateReadTime >= _WMDetectedDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.StartActionDetails) * -1))
                                    {
                                        _tag.TagGateResultStatus = "";
                                    }
                                }



                                _GateResultListFiltered = _GateResultListFiltered.Where(p => p.TagGateReadTime < _WMDetectedDateTime.AddSeconds(Convert.ToInt32(Properties.Settings.Default.StartActionDetails) * -1)).ToList();



                            }
                        }

                        this.Invoke(new Action(() => lblWMId.Text = wm.WMCode.ToString()));
                        this.Invoke(new Action(() => lblWMTitle.Text = wm.WMDriverName));
                        this.Invoke(new Action(() => lblWMUserId.Text = wm.WMDriverUserId));



                        _RequireWmClear = true;
                        _RequireWMClearDetectedDateTime = DateTime.Now;

                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }


    private void ClearWMInfo()
    {
        this.Invoke(new Action(() => lblWMId.Text = ""));
        this.Invoke(new Action(() => lblWMTitle.Text = ""));
        this.Invoke(new Action(() => lblWMUserId.Text = ""));
        _detectWM = false;

    }

    private List<GateResult> ComputeTajamoee(List<GateResult> _listGateResultFiltered, int KindTajamo)
    {
        try
        {
            List<GateResult> _GateResultListTajamoeeForResult = new List<GateResult>();
            var _TemplistGateResultFilteredTajamoee = _listGateResultFiltered.Where(p => p.TagPackageStatus == 1 && p.TagGateResultStatus == "تأیید").GroupBy(p => p.ProductCode);
            foreach (var group in _TemplistGateResultFilteredTajamoee)
            {
                GateResult grp = new GateResult();
                grp.ProductCode = group.Key;
                foreach (var _tag in group)
                {
                    grp.Count = (grp.Count == "" || grp.Count == "0") ? "1" : (Convert.ToInt32(grp.Count) + 1).ToString();
                    grp.SumValue = (grp.SumValue == "" || grp.SumValue == "0") ? "1" : (Convert.ToDecimal(grp.SumValue) + Convert.ToDecimal(_tag.SumValue)).ToString();

                    grp.ProductName = _tag.ProductName;
                    grp.ProductTechnicalCode = _tag.ProductTechnicalCode;
                    grp.ProductType = _tag.ProductType;
                    grp.TagStatus = _tag.ProductStatus;
                    grp.DocumentId = _tag.DocumentId;
                    grp.DocumentCheckStatusDesc = "";
                }

                _GateResultListTajamoeeForResult.Add(grp);
            }
            return _GateResultListTajamoeeForResult;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return null;
        }

    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        bool _AllowToExit = false;

        if (_FlagResetApp)
        {
            _AllowToExit = true;
        }
        else
        {
            if (MessageBox.Show("آیا از بستن نرم افزار اطمینان دارید؟", "هشدار", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)


            {
                _AllowToExit = true;

            }
        }

        if (_AllowToExit)
        {
            try
            {
                _UhfReaders.Stop();
                _UhfReaders.DisConnect();
            }
            catch
            {

            }
        }
        else
        {
            e.Cancel = true;
        }
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        Application.Exit();
    }

    private void btnMultiPartSave_Click(object sender, EventArgs e)
    {
        if (Properties.Settings.Default.TagDetectInMultiPart != "0" && lblPnlPart_Count.Text != "" && lblPnlPart_Count.Text != "0")
        {
            try
            {
                dgvPnlPart.Rows.Add(MultiPartRowIndex, lblPnlPart_Count.Text, lblPnlPart_DocumentId.Text, lblWMTitle.Text, lblWMId.Text, lblWMUserId.Text);


                foreach (GateResult _tag in _GateResultListFiltered)
                {
                    if (_tag.TagPackageStatus == 0)
                    {
                        _tag.TagPackageStatus = 1;
                        _tag.WMUsertId = lblWMUserId.Text;
                        _tag.TagPackageId = MultiPartRowIndex;

                    }
                }

                ShowProduct();
                MultiPartRowIndex++;
            }
            catch
            {

            }
        }
    }

    private void grpShowProduct_Enter(object sender, EventArgs e)
    {

    }

    private void btnMultiPartDelete_Click(object sender, EventArgs e)
    {
        try
        {
            var GeteResultTemp = _GateResultList.Where(p => p.TagPackageId == MultiPartRowIndex).ToList();
            foreach (GateResult _tag in GeteResultTemp)
            {
                _DetectedTagList.FirstOrDefault(p => p.TagEPC == _tag.TagSerial).TagActionStatus = 3;
            }
            _GateResultListFiltered = _GateResultListFiltered.Where(p => p.TagPackageId != MultiPartRowIndex).ToList();
            _GateResultList = _GateResultList.Where(p => p.TagPackageId != MultiPartRowIndex).ToList();
            lblPnlPart_Count.Text = "0";
        }
        catch
        {

        }
    }


    private void lblCountInventory_Click(object sender, EventArgs e)
    {

    }

    private async void FinishANDSave()
    {
        try
        {
            _InSaveActionProgressActionId = "";
            grpAction.Enabled = false;
            if (Properties.Settings.Default.RequirmentDocument == "1" && txtDocumentId.Text.Trim() == "")
            {
                Alarm("ErrorWithOutLockForm", "سند عطف مشخص نشده است", "", "", "");
                return;
            }


            if (_ActionId != "" && !_InSaveActionProgress && _GateResultListFiltered.Count > 0 && !_ErrorDetected && _FlagActionStart && !_FlagActionEnd && _actionStatus != ActionStatus.Error && _DocumentCheck)
            {
                _InSaveActionProgress = true;

                _InSaveActionProgressActionId = _ActionId;
                _ActionId = "";
                lblActionId.Text = "";
                await GetProductDataList(_InSaveActionProgressActionId);
                if (!_ErrorDetected)
                {
                    if (Properties.Settings.Default.RequirmentDocument == "1" && txtDocumentId.Text.Trim() != "" && Properties.Settings.Default.ControlByDocument == "1")
                    {
                        GetDocumentData(txtDocumentId.Text);
                        if (!_DocumentCheck)
                        {
                            Alarm("ErrorWithOutLockForm", "مغایرت با سند عطف", "مغایرت با سند عطف", "", "");
                            _ActionId = _InSaveActionProgressActionId;
                            lblActionId.Text = _InSaveActionProgressActionId;
                            _InSaveActionProgress = false;

                            return;
                        }
                    }



                    FormLockANDInlock(true);
                    timer_checkSave.Enabled = true;
                    if (Properties.Settings.Default.KindGateSave == 1)
                    {
                        string MovementActionDesc = "جایجایی کالا - " + Properties.Settings.Default.GateTitle;
                        string DocumentID = txtDocumentId.Text;



                        dynamic exo = new System.Dynamic.ExpandoObject();

                        foreach (var field in _dynamicFieldDtos)
                        {
                            ((IDictionary<String, Object>)exo).Add(field.Title, field.Value);
                        }

                        var dynamicFieldWithValue = Newtonsoft.Json.JsonConvert.SerializeObject(exo);


                        var ActionData = JToken.Parse(dynamicFieldWithValue);



                        if (await _apiBusiness.SaveAction(_InSaveActionProgressActionId, lblWMId.Text, "", ActionData, MovementActionDesc, DocumentID, lblWMId.Text))
                        {
                            _ActionId = "";
                            lblActionId.Text = "";
                            _InSaveActionProgressActionId = "";
                            _EndActionDateTime = DateTime.Now;
                            _FlagActionEnd = true;
                            _actionStatus = ActionStatus.Finished;
                            Alarm("NotError", "عملیات با موفقیت ثبت شد", "", "", "");
                            timer_checkSave.Enabled = false;
                            _InSaveActionProgress = false;


                        }
                        else if (Properties.Settings.Default.KindGateSave == 0)
                        {
                            _ActionId = _InSaveActionProgressActionId;
                            lblActionId.Text = _InSaveActionProgressActionId;
                            _InSaveActionProgressActionId = "";
                            _InSaveActionProgress = false;

                            Alarm("Error", "خطایی در ثبت عملیات به وجود آمده است", "", "", "");

                        }
                    }
                    else
                    {
                        _ActionId = "";
                        lblActionId.Text = "";
                        _EndActionDateTime = DateTime.Now;
                        _FlagActionEnd = true;
                        _actionStatus = ActionStatus.Finished;
                        _InSaveActionProgressActionId = "";
                        _InSaveActionProgress = false;

                        Alarm("NotError", "عملیات با موفقیت ثبت شد", "", "", "");
                    }

                }
            }
            grpAction.Enabled = true;


        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }


    private async Task GetANDSetDynamicFieldDtos()
    {
        ActionType = await _apiBusiness.GetActionType((Properties.Settings.Default.FromStore.Contains(',')) ? Properties.Settings.Default.FromStore.Split(',')[0] : Properties.Settings.Default.FromStore, Properties.Settings.Default.ToStore);
        this.Invoke(new Action(() => lblActionType.Text = ActionType.ToString()));


        //            _dynamicFieldDtos.FirstOrDefault(p => p.Id.ToString() == ((Control)sender).Tag.ToString()).Value = ((Control)sender).Text;

        _dynamicFieldDtos = await _apiBusiness.GetDynamicFieldsByActionTypeId(ActionType.ToString());
        int index = 0;
        foreach (DynamicFieldDto field in _dynamicFieldDtos)
        {
            if (field.FieldType == DynamicFieldType.HeaderData)
            {
                //switch (field.DataTyp)
                //{
                //    case "":

                System.Windows.Forms.Label lable_temp = new Label();

                string[] _DataTemp = field.RelatedTitle1.Split('،');

                lable_temp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
                lable_temp.Font = new System.Drawing.Font("IRANSans(FaNum)", 10F);
                lable_temp.Location = new System.Drawing.Point(592, (index * 30) + (index * 4) + 49);
                lable_temp.Name = "label" + field.Id;
                lable_temp.Size = new System.Drawing.Size(190, 30);
                lable_temp.TabIndex = 200 + index;
                lable_temp.Text = field.Title + ":";
                lable_temp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

                System.Windows.Forms.TextBox inpute_temp = new TextBox();


                inpute_temp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
| System.Windows.Forms.AnchorStyles.Right)));
                inpute_temp.Font = new System.Drawing.Font("IRANSans(FaNum)", 10F);
                inpute_temp.Location = new System.Drawing.Point(3, (index * 30) + (index * 4) + 49);
                inpute_temp.Name = "dynamic_Control" + field.Title;
                inpute_temp.Tag = field.Id;
                inpute_temp.Size = new System.Drawing.Size(584, 30);
                inpute_temp.TabIndex = 100 + index;
                inpute_temp.TextAlign = HorizontalAlignment.Left;

                inpute_temp.Leave += new System.EventHandler(this.DynamicField_Leave);


                pnlDynamicFeilds.Controls.Add(lable_temp);
                pnlDynamicFeilds.Controls.Add(inpute_temp);

                //        break;
                //}
                index++;
            }
        }

    }

    private void DynamicField_Leave(object sender, EventArgs e)
    {
        _dynamicFieldDtos.FirstOrDefault(p => p.Id.ToString() == ((Control)sender).Tag.ToString()).Value = ((Control)sender).Text;

    }



    private void btnSave_Click(object sender, EventArgs e)
    {
        FinishANDSave();
    }

    private void pnl_show_TwoSeial_Paint(object sender, PaintEventArgs e)
    {

    }

    private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {

    }

    private void pictureBox8_Click(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {
        txtAddManualEPC.Visible = true;
        txtAddManualEPC.Focus();
        txtAddManualEPC.Text = "";
    }

    private void pcbMachin_Click(object sender, EventArgs e)
    {
        if (Properties.Settings.Default.SearchMachin == "1")
        {
            TruckCross _selectedTruckCross = new TruckCross();
            new frmSearchMachin(_selectedTruckCross).ShowDialog();
            lblPlaque.Text = _selectedTruckCross.plaque;
            lblWMId.Text = _selectedTruckCross.Id;
            lblWMTitle.Text = _selectedTruckCross.DriverName;
        }
    }

    private void btnActionExtraData_Click(object sender, EventArgs e)
    {
        pnlDynamicFeilds.Visible = true;
    }

    private void panel3_Paint(object sender, PaintEventArgs e)
    {

    }


    private async void GetDocumentData(string DocumentId)
    {
        try
        {
            if (DocumentId != "")
            {
                if (!GetDocumentDataFlag)
                {
                    label13.Text = "اطلاعات و اقلام سند عطف عملیات: " + DocumentId;
                    dgvDocumentHeader.Rows.Clear();
                    dgvDocumentItems.Rows.Clear();
                    _documentItem.Clear();
                    this.Invoke(new Action(() => dgvDocumentHeader.Rows.Clear()));
                    this.Invoke(new Action(() => dgvDocumentItems.Rows.Clear()));
                    this.Invoke(new Action(() => _documentItem.Clear()));

                    var result = await _apiBusiness.GetDocumentData(DocumentId, ActionType);

                    if (result.ToString() == "")
                    {

                        MessageBox.Show("اطلاعاتی برای کد سند : " + DocumentId + " یافت نشد", "توجه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        pnlDocumentInfo.Visible = false;
                        return;


                    }

                    dgvDocumentHeader.Rows.Clear();

                    var headerJson = JToken.Parse(result["headerData"].ToString());
                    foreach (JToken item in headerJson)
                    {
                        string[] info = item.ToString().Split(':');
                        var temp_title = info[0].Replace(@"""", "").Replace(@"{", "").Replace(@"}", "").Replace(@"\r\n", "").Replace(@"\", "").Trim();
                        var temp_Value = info[1].Replace(@"""", "").Replace(@"{", "").Replace(@"}", "").Replace(@"\r\n", "").Replace(@"\", "").Trim();

                        dgvDocumentHeader.Rows.Add(temp_title, temp_Value);
                        string TargetControlName = "dynamic_Control" + temp_title.Replace("\r\n", "").Trim();

                        foreach (Control cnt in pnlDynamicFeilds.Controls)
                        {
                            if (cnt.Name.ToString() == TargetControlName)
                            {
                                if (cnt.Text == "")
                                    cnt.Text = temp_Value.Replace("\r\n", "").Trim();
                            }
                        }

                        foreach (DynamicFieldDto _dto in _dynamicFieldDtos)
                        {
                            if (_dto.Title == temp_title.Replace("\r\n", "").Trim())
                            {
                                if (_dto.Value.ToString() == "")
                                    _dto.Value = temp_Value.Replace("\r\n", "").Trim();
                            }

                        }


                    }
                    int index = 1;
                    dgvDocumentItems.Rows.Clear();
                    var documentItemsJson = JToken.Parse(result["documentItems"].ToString());
                    foreach (JToken item in documentItemsJson)
                    {

                        if (_documentItem.FirstOrDefault(p => p.ProductCode == item["productCode"].ToString()) == null)
                        {
                            DocumentItem _item = new DocumentItem
                            {
                                ProductCode = item["productCode"].ToString(),
                                ProductTitle = item["productTitle"].ToString(),
                                Count = Convert.ToInt32(item["count"].ToString()),
                                StatusDesc = ""

                            };
                            _documentItem.Add(_item);
                        }
                        else
                        {
                            _documentItem.FirstOrDefault(p => p.ProductCode == item["productCode"].ToString()).Count += Convert.ToInt32(item["count"].ToString());
                        }
                        string productCode = item["productCode"].ToString();
                        dgvDocumentItems.Rows.Add(index, item["productCode"].ToString(), item["productTitle"].ToString(), item["count"].ToString(), "");
                        index++;
                    }
                    GetDocumentDataFlag = true;
                    if (Properties.Settings.Default.ChangeDocumentStatusAfterCheck == "1")
                    {
                        _apiBusiness.SChangeDocumentStatus(DocumentId, ActionType);
                    }
                }
                CheckGateResultByDocumentItem();

            }
            else
            {
                MessageBox.Show("لطفا ابتدا کد سند را مشخص نمایید", "توجه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDocumentId.Focus();
                pnlDocumentInfo.Visible = false;
                return;
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }
    private void btnDocumentInfo_Click(object sender, EventArgs e)
    {
        pnlDocumentInfo.Visible = true;
        GetDocumentData(txtDocumentId.Text);
    }

    private void btnShowSerials_Click(object sender, EventArgs e)
    {
        pnlShowSerials.Visible = true;
        ShowSerialList("");
    }

    private void ShowSerialList(string ProductCode)
    {
        try
        {
            int index = 1;
            dgvSerialList.Rows.Clear();
            foreach (GateResult _tag in _GateResultListFiltered)
            {
                if (ProductCode == "" || _tag.ProductCode == ProductCode)
                {
                    dgvSerialList.Rows.Add(index, _tag.ProductSerial, _tag.ProductOldSerial, _tag.ProductCode, _tag.ProductTechnicalCode, _tag.ProductName, _tag.DocumentId, _tag.Count, _tag.TagGateResultStatus, _tag.TagSerial, "حذف");
                    if (_tag.TagGateResultStatus != "تأیید")
                    {
                        dgvSerialList.Rows[dgvSerialList.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Red;
                        if (_tag.TagGateResultStatus == "تگ رجیستر نشده")
                            dgvSerialList.Rows[dgvSerialList.Rows.Count - 1].Cells["colSerialSerialList"].Value = _tag.TagSerial;
                    }
                    index++;
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private void btnClosePnlShowSerials_Click(object sender, EventArgs e)
    {
        pnlShowSerials.Visible = false;
    }

    private void dgvSerialList_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (e.RowIndex >= 0 && dgvSerialList.Columns[e.ColumnIndex].Name == "colbtnDeleteSerialList")
            {
                if (MessageBox.Show("آیا از حذف اطلاعات اطمینان دارید؟", "هشدار", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (dgvSerialList.Rows[e.RowIndex].Cells["colStatusSerialList"].Value.ToString() != "تأیید")
                    {

                        if (Properties.Settings.Default.UserAccessForDeleteErrorSerial == "1")
                        {
                            if (DeleteSerialFromGateResult(dgvSerialList.Rows[e.RowIndex].Cells["colSerialSerialList"].Value.ToString(), dgvSerialList.Rows[e.RowIndex].Cells["colEpcSerialList"].Value.ToString()))
                            {
                                dgvSerialList.Rows.RemoveAt(e.RowIndex);

                            }
                        }
                        else
                        {

                            var _frmLogin = new frmLogin();
                            if (_frmLogin.ShowDialog() == DialogResult.Yes)
                            {
                                if (DeleteSerialFromGateResult(dgvSerialList.Rows[e.RowIndex].Cells["colSerialSerialList"].Value.ToString(), dgvSerialList.Rows[e.RowIndex].Cells["colEpcSerialList"].Value.ToString()))
                                {
                                    dgvSerialList.Rows.RemoveAt(e.RowIndex);

                                }

                            }

                        }
                    }
                    else
                    {

                        if (Properties.Settings.Default.UserAccessForDeleteTrueSerial == "1")
                        {
                            if (DeleteSerialFromGateResult(dgvSerialList.Rows[e.RowIndex].Cells["colSerialSerialList"].Value.ToString(), dgvSerialList.Rows[e.RowIndex].Cells["colEpcSerialList"].Value.ToString()))
                            {
                                dgvSerialList.Rows.RemoveAt(e.RowIndex);
                            }
                        }
                        else
                        {
                            var _frmLogin = new frmLogin();
                            if (_frmLogin.ShowDialog() == DialogResult.Yes)
                            {
                                if (DeleteSerialFromGateResult(dgvSerialList.Rows[e.RowIndex].Cells["colSerialSerialList"].Value.ToString(), dgvSerialList.Rows[e.RowIndex].Cells["colEpcSerialList"].Value.ToString()))
                                {
                                    dgvSerialList.Rows.RemoveAt(e.RowIndex);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {

        }
    }


    private void FormLockANDInlock(bool Lock)
    {
        if (Lock)
        {
            groupBox1.Enabled = grpMachin.Enabled = grpPartDetected.Enabled = grpShowProduct.Enabled = grpSummary.Enabled = false;
        }
        else
        {
            groupBox1.Enabled = grpMachin.Enabled = grpPartDetected.Enabled = grpShowProduct.Enabled = grpSummary.Enabled = true;

        }
    }
    private bool DeleteSerialFromGateResult(string Serial, string Epc)
    {
        try
        {
            try
            {
                _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == Epc.ToLower()).TagActionStatus = 9;
                _DetectedTagList.FirstOrDefault(p => p.TagEPC.ToLower() == Epc.ToLower()).TagReedSaveStatus = 0;

            }
            catch
            {

            }
            _GateResultList = _GateResultList.Where(p => p.TagSerial.ToLower() != Epc.ToLower()).ToList();
            _GateResultListFiltered = _GateResultListFiltered.Where(p => p.TagSerial.ToLower() != Epc.ToLower()).ToList();
            ShowProduct();
            return true;


        }
        catch
        {
            ShowProduct();
            return false;

        }


    }

    private void lblLocationCode_Click(object sender, EventArgs e)
    {

    }

    private void btnSetting_Click(object sender, EventArgs e)
    {
        var _frmLogin = new frmLogin();
        if (_frmLogin.ShowDialog() == DialogResult.Yes)
        {
            _UhfReaders.Stop();
            _UhfReaders.DisConnect();
            lblConnectToReaderStatus.BackColor = Color.Red;

            new frmSetting().ShowDialog();
            GetGateSetting();
            ShowProduct();


            _UhfReaders.Connect();


            _UhfReaders.SetConfig(Convert.ToInt32(lblReaderPowerSet.Text));
            lblConnectToReaderStatus.Text = _UhfReaders.GetPower().ToString();


            if (_UhfReaders.Start())
            {
                lblConnectToReaderStatus.BackColor = Color.Yellow;
                timer_RefreshReaderBuffer.Enabled = true;

            }
            else
            {
                lblConnectToReaderStatus.BackColor = Color.Red;
                lblConnectToReaderStatus.Text = "0";
            }
        }
    }

    private void dgvMutiProduct_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && dgvMutiProduct.Columns[e.ColumnIndex].Name == "btncolMultiProductShowSerial")
        {
            pnlShowSerials.Visible = true;
            ShowSerialList(dgvMutiProduct.Rows[e.RowIndex].Cells["dgvMutiProductProductCode"].Value.ToString());
        }
    }

    private void dgvSerialList_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }

    private void label19_Click(object sender, EventArgs e)
    {
        WM_ReadEPC = "AAAAAAAA0000000000000001";
        ShowWMInfo(WM_ReadEPC);
    }

    private void btnClosepnlActionExtraData_Click(object sender, EventArgs e)
    {
        pnlActionIdChange.Visible = false;
    }

    private void btnClosepnlDocumentInfo_Click(object sender, EventArgs e)
    {
        pnlDocumentInfo.Visible = false;

    }

    private void btnRefreshDetectedTagList_Click(object sender, EventArgs e)
    {
        RefreshDetectedTagList();
    }

    private void RefreshDetectedTagList()
    {
        try
        {
            _UhfReaders.Refresh();
            _DetectedTagList.Clear();
            lblDetectionTagsCount.Text = "0/0";
            this.Invoke(new Action(() => _UhfReaders.Refresh()));
            this.Invoke(new Action(() => _DetectedTagList.Clear()));
            this.Invoke(new Action(() => lblDetectionTagsCount.Text = "0/0"));

            foreach (var oldtag in _GateResultList)
            {
                if (_DetectedTagList.FirstOrDefault(p => p.TagEPC == oldtag.TagSerial) == null)
                {
                    Tags _tag = new Tags();
                    _tag.TagActionStatus = 0;
                    _tag.TagReedSaveStatus = 0;
                    _tag.TagPackageId = MultiPartRowIndex;
                    _tag.DocumentId = lblPnlPart_DocumentId.Text;
                    _tag.WMUsertId = lblWMUserId.Text;
                    _tag.TagReedTime = DateTime.Now;
                    _tag.TagEPC = oldtag.TagSerial;
                    _DetectedTagList.Add(_tag);

                }
            }

            lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/0";
            this.Invoke(new Action(() => lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/0"));


        }
        catch
        {

        }
    }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }

    private void grpAction_Enter(object sender, EventArgs e)
    {

    }

    private void txtDocumentId_Leave(object sender, EventArgs e)
    {
        if (txtDocumentId.Text.Trim() != "")
            GetDocumentData(txtDocumentId.Text);
    }

    private void txtDocumentId_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (txtDocumentId.Text.Trim() != "")
                GetDocumentData(txtDocumentId.Text);
        }
    }

    private void lblMassage_Click(object sender, EventArgs e)
    {

    }

    private async void timer_GetFromHandHeld_Tick(object sender, EventArgs e)
    {
        try
        {
            if (_ActionId == "")
                return;
            if (Properties.Settings.Default.EndActionKind == "3")
            {
                if (!_detectWM)
                    return;
            }
            if (_InSaveActionProgress)
                return;
            var _TempGateResultList = await _apiBusiness.SSaveGateLogAndShowResultForHandHeldTags(_ActionId, ActionType.ToString());
            if (_TempGateResultList != null && !_InSaveActionProgress)
            {
                if (!_FlagActionStart && _TempGateResultList.Count > 0)
                    StartAction(0);
                foreach (GateResult _tag in _TempGateResultList)
                {
                    if (_GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial) == null)
                    {
                        _tag.TagGateResultStatus = "";
                        _tag.TagPackageId = (Properties.Settings.Default.TagDetectInMultiPart == "0") ? 0 : MultiPartRowIndex;
                        _tag.TagPackageStatus = (_tag.TagPackageStatus == 1) ? 1 : ((Properties.Settings.Default.TagDetectInMultiPart == "0") ? 1 : 0);
                        _tag.TagGateReadTime = DateTime.Now;
                        _GateResultList.Add(_tag);
                    }
                    else
                    {
                        if (_GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial).ProductSerial != _tag.ProductSerial || _GateResultList.FirstOrDefault(p => p.TagSerial == _tag.TagSerial).TagInDestinationId != _tag.TagInDestinationId)// تگ  بعد از شناسایی گیت رجیستر شده
                        {
                            _GateResultList = _GateResultList.Where(p => p.TagSerial != _tag.TagSerial).ToList();
                            _GateResultListFiltered = _GateResultListFiltered.Where(p => p.TagSerial != _tag.TagSerial).ToList();
                            _tag.TagGateResultStatus = "";
                            _tag.TagPackageId = (Properties.Settings.Default.TagDetectInMultiPart == "0") ? 0 : MultiPartRowIndex;
                            _tag.TagPackageStatus = (_tag.TagPackageStatus == 1) ? 1 : ((Properties.Settings.Default.TagDetectInMultiPart == "0") ? 1 : 0);
                            _tag.TagGateReadTime = DateTime.Now;
                            _GateResultList.Add(_tag);
                        }

                    }

                    Tags _newHandheldTag = new Tags();
                    _newHandheldTag.TagEPC = _tag.TagSerial;
                    _newHandheldTag.TagPackageId = MultiPartRowIndex;
                    if (!_newHandheldTag.TagEPC.Contains(Properties.Settings.Default.WM_EPC_Pattern))
                    {
                        if (_DetectedTagList.FirstOrDefault(p => p.TagEPC == _newHandheldTag.TagEPC) == null)
                        {
                            _LatestTagReadDateTime = DateTime.Now;

                            _newHandheldTag.TagReedTime = DateTime.Now;
                            _newHandheldTag.TagActionStatus = 0;
                            _newHandheldTag.TagReedSaveStatus = 1;
                            _newHandheldTag.TagPackageId = MultiPartRowIndex;
                            _newHandheldTag.DocumentId = lblPnlPart_DocumentId.Text;
                            _newHandheldTag.WMUsertId = lblWMUserId.Text;



                            if (Properties.Settings.Default.StartActionKind == "0")
                            {
                                if (_DetectedTagList.Count == 0)
                                    _StartActionDateTime = DateTime.Now;
                            }

                            if (Properties.Settings.Default.EndActionKind == "1")
                            {

                                _LatestTrueTagReadDateTime = DateTime.Now;

                            }
                            if (Properties.Settings.Default.EndActionKind == "5")
                            {

                                _LatestTagReadDateTime = DateTime.Now;

                            }



                            _DetectedTagList.Add(_newHandheldTag);
                        }
                    }
                    else
                    {
                        WM_ReadEPC = _newHandheldTag.TagEPC;
                        ShowWMInfo(WM_ReadEPC);
                    }
                }
                lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/" + _TempGateResultList.Count.ToString();
                this.Invoke(new Action(() => lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/" + _TempGateResultList.Count.ToString()));


            }

            ShowProduct();
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }

    private void timer_checkSave_Tick(object sender, EventArgs e)
    {
        if (!groupBox1.Enabled)
        {
            Alarm("Error", "خطایی در ثبت عملیات به وجود آمده است", "", "", "");
        }
    }

    private void panel3_Paint_1(object sender, PaintEventArgs e)
    {

    }

    private void trackBarReaderPower_Scroll(object sender, EventArgs e)
    {
    }

    private void lblConnectToReaderStatus_Click(object sender, EventArgs e)
    {
        if (lblConnectToReaderStatus.Text != "0" && !pnlReaderPowerSet.Visible)
        {
            pnlReaderPowerSet.Visible = true;
            trackBarReaderPower.Value = Convert.ToInt32(lblConnectToReaderStatus.Text);
            lblReaderPowerSet.Text = lblConnectToReaderStatus.Text;
        }
    }

    private void trackBarReaderPower_ValueChanged(object sender, EventArgs e)
    {
        lblReaderPowerSet.Text = trackBarReaderPower.Value.ToString();
    }

    private void btnRaderPowerSet_Click(object sender, EventArgs e)
    {
        try
        {
            _UhfReaders.Stop();
            lblConnectToReaderStatus.BackColor = Color.Red;

            Properties.Settings.Default.ReaderPower = lblReaderPowerSet.Text;
            Properties.Settings.Default.Save();
            GetGateSetting();
            _UhfReaders.SetConfig(Convert.ToInt32(lblReaderPowerSet.Text));
            lblConnectToReaderStatus.Text = _UhfReaders.GetPower().ToString();
            if (_UhfReaders.Start())
            {
                lblConnectToReaderStatus.BackColor = Color.Yellow;
                timer_RefreshReaderBuffer.Enabled = true;

            }
            else
            {
                lblConnectToReaderStatus.BackColor = Color.Red;
                lblConnectToReaderStatus.Text = "0";
            }
            pnlReaderPowerSet.Visible = false;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }

    }

    private void btnReaderPowerSetCancel_Click(object sender, EventArgs e)
    {
        pnlReaderPowerSet.Visible = false;
    }

    private void btnReaderPowerAdd_Click(object sender, EventArgs e)
    {
        if (Convert.ToInt32(lblReaderPowerSet.Text) < 30)
        {
            trackBarReaderPower.Value++;
        }
    }

    private void label20_Click(object sender, EventArgs e)
    {
        if (Convert.ToInt32(lblReaderPowerSet.Text) > 1)
        {
            trackBarReaderPower.Value--;
        }
    }

    private async void btnPreviousActionId_Click(object sender, EventArgs e)
    {
        txtNewActionId.Text = await _apiBusiness.GetNextPreviousInvIdByCurrentId(true, txtNewActionId.Text);
    }

    private async void btnNextActionId_Click(object sender, EventArgs e)
    {
        txtNewActionId.Text = await _apiBusiness.GetNextPreviousInvIdByCurrentId(false, txtNewActionId.Text);
    }

    private async void btnConfirmNewActionId_Click(object sender, EventArgs e)
    {
        timer_checkSave.Enabled = false;

        Clear();

        _ActionId = txtNewActionId.Text;
        this.Invoke(new Action(() => lblActionId.Text = txtNewActionId.Text));

        var _gateResult = await _apiBusiness.ChangeActionIdGetEpcList(_ActionId);

        foreach (var _gateResulttag in _gateResult)
        {
            Tags _tag = new Tags();
            _tag.TagActionStatus = 0;
            _tag.TagReedSaveStatus = 1;
            _tag.TagPackageId = MultiPartRowIndex;
            _tag.DocumentId = lblPnlPart_DocumentId.Text;
            _tag.WMUsertId = lblWMUserId.Text;
            _tag.TagReedTime = DateTime.Now;
            _tag.TagEPC = _gateResulttag.TagSerial;
            _DetectedTagList.Add(_tag);

        }

        AddDetectedTagListWhenGateInSaveActionIntoMainTagList();


        lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/0";
        this.Invoke(new Action(() => lblDetectionTagsCount.Text = _DetectedTagList.Count.ToString() + "/0"));

        StartAction(0);

    }

    private void label44_Click(object sender, EventArgs e)
    {
        var _frmLogin = new frmLogin();
        if (_frmLogin.ShowDialog() == DialogResult.Yes)
        {
            pnlActionIdChange.Visible = true;
            txtNewActionId.Text = lblActionId.Text;

        }
    }

    private void btnClosePnlDynamicFeilds_Click(object sender, EventArgs e)
    {

        pnlDynamicFeilds.Visible = false;
    }

    private void timer_appReset_Tick(object sender, EventArgs e)
    {
        if (!_FlagActionStart )
        {
            _FlagResetApp = true;
            try
            {
                _UhfReaders.Stop();
                _UhfReaders.DisConnect();
            }
            catch
            {

            }
        }
    }

    private void btnSearchDoc_Click(object sender, EventArgs e)
    {
        pnlDocs.Visible = true;
        GetAllPermitedDocs();
        dgvDocsList.Focus();
    }

    private async void GetAllPermitedDocs()
    {
        var _docList = await _apiBusiness.GetAggDocsByDocTypeAndStatus(lblActionType.Text);
        dgvDocsList.Rows.Clear();
        if (_docList != null)
        {
            foreach (var doc in _docList)
            {
                dgvDocsList.Rows.Add(doc.DocumentKey, doc.DocumentData, "انتخاب");
            }
        }
    }

    private void dgvDocsList_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (e.RowIndex >= 0 && dgvDocsList.Columns[e.ColumnIndex].Name == "colbtnSelectDoc")
            {
                txtDocumentId.Text = dgvDocsList.Rows[e.RowIndex].Cells[0].Value.ToString();
                GetDocumentDataFlag = false;
                GetDocumentData(txtDocumentId.Text);
                pnlDocs.Visible = false;

            }
        }
        catch
        {

        }

    }

    private void btnCancelPnlDocs_Click(object sender, EventArgs e)
    {
        pnlDocs.Visible = false;
    }

    private void pictureBox3_Click(object sender, EventArgs e)
    {
        pnlIgnoreSerials.Visible = true;
        txtIgnoreSerials.Text = Properties.Settings.Default.IgnoreSerials;
        txtIgnoreSerials.Focus();
    }

    private void label36_Click(object sender, EventArgs e)
    {
        pnlIgnoreSerials.Visible = false;
    }

    private void btnSaveIgnoreSerials_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.IgnoreSerials = txtIgnoreSerials.Text;
        Properties.Settings.Default.Save();
        pnlIgnoreSerials.Visible = false;

    }

    private void txtDocumentId_TextChanged(object sender, EventArgs e)
    {
        GetDocumentDataFlag = false;
    }

    private void lblDetectionTagsCount_Click(object sender, EventArgs e)
    {
        if (pnlDetectedTags.Visible)
        {
            pnlDetectedTags.Visible = false;

        }
        else
        {
            pnlDetectedTags.Visible = true;
            dgvDetectedTags.Rows.Clear();
            int Row = 1;
            foreach (Tags tdg in _DetectedTagList)
            {
                var _tempGateResult = _GateResultList.FirstOrDefault(p => p.TagSerial == tdg.TagEPC);
                string StatusDesc = "";

                switch (tdg.TagActionStatus)
                {
                    case 0:
                        StatusDesc = "صحیح";
                        break;
                    case 2:
                        StatusDesc = "در انبار مبدأ ناموجود";
                        break;
                    case 3:
                        StatusDesc = "تگ رجیستر نشده";
                        break;
                    case 4:
                        StatusDesc = "کالای فریز شده";
                        break;
                    case 5:
                        StatusDesc = "بازرسی مردود";
                        break;
                    case 7:
                        StatusDesc = "گذشت زمان مشخص از رجیستر";
                        break;
                    case 9:
                        StatusDesc = "تگ حذف شده توسط کاربر";
                        break;
                    default:
                        StatusDesc = "سایر";
                        break;


                }
                if (_tempGateResult != null)
                {
                    dgvDetectedTags.Rows.Add(Row.ToString(), tdg.TagEPC, _tempGateResult.ProductSerial, _tempGateResult.ProductTechnicalCode, _tempGateResult.ProductCode, StatusDesc, tdg.TagActionStatus);




                }
                else
                {
                    dgvDetectedTags.Rows.Add(Row.ToString(), tdg.TagEPC, "", "", "", "نامشخص", tdg.TagActionStatus);
                }
                Row++;

            }
        }
    }

    private void btnpnlDetectedTagsCancel_Click(object sender, EventArgs e)
    {
        pnlDetectedTags.Visible = false;
    }

    private void chbDetectedTagNotTrue_CheckedChanged(object sender, EventArgs e)
    {
        foreach (DataGridViewRow dr in dgvDetectedTags.Rows)
        {
            if (chbDetectedTagNotTrue.Checked)
            {
                if (dr.Cells["dgvDetectedTagActionStatus"].Value.ToString() == "0")
                {
                    dr.Visible = false;
                }
            }
            else
            {
                dr.Visible = true;
            }
        }
    }

    private void txtAddManualEPC_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            List<Tags> _detectedTagList = new List<Tags>();
            Tags _tag = new Tags();
            _tag.TagActionStatus = 0;
            _tag.TagReedSaveStatus = 0;
            _tag.TagPackageId = MultiPartRowIndex;
            _tag.DocumentId = lblPnlPart_DocumentId.Text;
            _tag.WMUsertId = lblWMUserId.Text;
            _tag.TagReedTime = DateTime.Now;
            _tag.TagEPC = txtAddManualEPC.Text;
            _detectedTagList.Add(_tag);
            _UhfReaders__setTextCallback(_detectedTagList);
        }
    }

    private void groupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void grpSummary_Enter(object sender, EventArgs e)
    {

    }

    private void lblConnectToReaderStatus_MouseHover(object sender, EventArgs e)
    {
        toolTip1.Show(_LatestTagReadDateTime.ToString(), lblConnectToReaderStatus);
    }

    private void btnGetNewActionId_Click(object sender, EventArgs e)
    {
        btnManualGetNewActionId();
    }

    private async Task btnManualGetNewActionId()
    {
        txtNewActionId.Text = (await _apiBusiness.GetNextIdByGateCode());

    }

    private void timer_RefreshReaderBuffer_Tick(object sender, EventArgs e)
    {
        _UhfReaders.Refresh();
    }
}
