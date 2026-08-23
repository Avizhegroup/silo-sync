using Silo.Ui.Gate.Models;

namespace Silo.Ui.Gate.BLL;

public class UhfReaders
{
    bool FlagReaderConnected = false;
    bool FlagIndentify = false;
    public delegate void DelegateOpen(bool open);
    public static event DelegateOpen eventOpen ;

    private Task trdReadEpcTask;
    private CancellationTokenSource cts;
    long beginTime = 0;
    public delegate void SetTextCallback(List<Tags> _DetectedTagList);
    public event SetTextCallback _setTextCallback;
    List<Tags> _DetectedTagsList = new List<Tags>();


    public static UHFAPI uhf = null;






    public void Connect()
    {
        bool result = false;
        
             uhf = UHFAPI.getInstance();
    

        if (Properties.Settings.Default.ReaderConnectionType=="پورت سریال")
        {
            try
            {

                int ComPort = int.Parse(Properties.Settings.Default.ReaderConnectDetails.Trim().Replace("COM", ""));
                result = uhf.Open(ComPort);


                 if (result)
                {
                    FlagReaderConnected = true;
                    if (eventOpen != null)
                        eventOpen.Invoke(true);


                    if (FlagReaderConnected)
                    {
                        if (!SetConfig(Convert.ToInt32(Properties.Settings.Default.ReaderPower)))
                            FlagReaderConnected=false;
                    }
                }
                else
                {
                    FlagReaderConnected = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
            }
        }
        else if (Properties.Settings.Default.ReaderConnectionType=="شبکه LAN")
        {
            try
            {
                
                string ReaderIp = "";

                ReaderIp = Properties.Settings.Default.ReaderConnectDetails;

                
                    result = uhf.TcpConnect(ReaderIp, 8888);
                 

                  
                if (result)
                {
                   

                    FlagReaderConnected = true;
                    if (eventOpen != null)
                    {
                        eventOpen.Invoke(true);
                   
                        //Chainway.UHFAPP.IPConfig.IPEntity entity = new Chainway.UHFAPP.IPConfig.IPEntity();
                        //entity.Port = (int)8888;
                        //entity.Ip = ipControl1.IpData;
                        //Chainway.UHFAPP.IPConfig.setIPConfig(entity);
                  
                    }


                    if (FlagReaderConnected)
                    {

                        if (!SetConfig(Convert.ToInt32(Properties.Settings.Default.ReaderPower)))
                            FlagReaderConnected=false;


                    }
                }
                else
                {
                    FlagReaderConnected = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
            }

        }
        else if (Properties.Settings.Default.ReaderConnectionType == "usb")
        {
           

            try
            {
                 result = uhf.OpenUsb();

                if (result)
                {
                    FlagReaderConnected = true;
                    if (eventOpen != null)
                        eventOpen.Invoke(true);


                    if (FlagReaderConnected)
                    {
                        if (!SetConfig(Convert.ToInt32(Properties.Settings.Default.ReaderPower)))
                            FlagReaderConnected = false;
                    }
                }
                else
                {
                    FlagReaderConnected = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteExceptionLogs(ex);
            }

        }

    }

    public bool Start()
    {
        try
        {
            if (uhf.Inventory())
            {
                FlagIndentify = true;
                cts = new CancellationTokenSource();

                trdReadEpcTask = Task.Run(() => ReadEPC(cts.Token), cts.Token);
            }
            else
            {
                FlagIndentify = false;
            }
            return FlagIndentify;
        }
        catch(Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;
        }
    }

    public bool Stop()
    {
        try
        {
            uhf.StopGet();

            FlagIndentify = false;
            if (cts != null)
            {
                cts.Cancel();
            }

            if (trdReadEpcTask != null)
            {
                try
                {
                    trdReadEpcTask.Wait(TimeSpan.FromSeconds(5)); 
                }
                catch (AggregateException ae)
                {
                    ae.Handle(e => e is TaskCanceledException);
                }
            }

            return FlagIndentify;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;
        }
    }
    public bool DisConnect()
    {
        try
        {
            if(FlagIndentify)
            {
                Stop();

            }
            if (FlagReaderConnected)
            {
                if (Properties.Settings.Default.ReaderConnectionType == "شبکه LAN")

                {
                    uhf.TcpDisconnect();
                }
                else if (Properties.Settings.Default.ReaderConnectionType == "پورت سریال")
                {
                    uhf.Close();
                }
                else if (Properties.Settings.Default.ReaderConnectionType == "usb")
                {
                    uhf.CloseUsb();

                }
                FlagReaderConnected = false;
                if (eventOpen != null)
                    eventOpen.Invoke(false);
            }

            return FlagReaderConnected;

        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;
        }
    }
    public bool SetConfig(int Power)
    {
        try
        {
            if (FlagReaderConnected)
            {
                byte power = (byte)Power;
                byte save = (byte)(1);
                return uhf.SetPower(save, power);
            }
            else
                return false;
        }
        catch(Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return false;
        }

    }


    public int GetPower()
    {
        try
        {
            if (FlagReaderConnected)
            {
                byte power = new byte();
                uhf.GetPower(ref power);
                return (Int32)power;
            }
            else
                return 10;
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
            return 0;
        }

    }


    public void Refresh()
    {
        try
        {
            _DetectedTagsList.Clear();
        }
        catch
        {

        }
    }

    private async Task ReadEPC(CancellationToken token)
    {
        try
        {
            string strID = "";
            beginTime = System.Environment.TickCount;
            Tags _tagReeded = new Tags();

            while (!token.IsCancellationRequested && FlagIndentify)
            {
                UHFTAGInfo info = uhf.uhfGetReceived();

                if (info != null && info.Epc.Length > 20)
                {
                    strID = info.Epc.Replace(" ", "");
                    _tagReeded = new Tags
                    {
                        TagEPC = strID,
                        TagReedGateNumber = Convert.ToInt32(Properties.Settings.Default.GateNumber),
                        TagReedTime = DateTime.Now,
                        TagReedSaveStatus = 0
                    };

                    if (_DetectedTagsList.FirstOrDefault(p => p.TagEPC == _tagReeded.TagEPC) == null)
                    {
                        _DetectedTagsList.Add(_tagReeded);
                        try
                        {
                            _setTextCallback?.Invoke(_DetectedTagsList);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    await Task.Delay(5, token);
                }
            }

            bool resultReceived = false;
            for (int k = 0; (k < 2) || resultReceived; k++)
            {
                await Task.Delay(1, token); 

                string epc = "";
                string tid = "";
                string rssi = "";
                string ant = "";
                resultReceived = UHFAPI.getInstance().uhfGetReceived(ref epc, ref tid, ref rssi, ref ant);

                if (resultReceived && epc.Length > 20)
                {
                    strID = epc;
                    _tagReeded = new Tags
                    {
                        TagEPC = strID,
                        TagReedGateNumber = Convert.ToInt32(Properties.Settings.Default.GateNumber),
                        TagReedTime = DateTime.Now,
                        TagReedSaveStatus = 0,
                        TagPackageId = 0
                    };

                    if (_DetectedTagsList.FirstOrDefault(p => p.TagEPC == _tagReeded.TagEPC) == null)
                    {
                        _DetectedTagsList.Add(_tagReeded);
                        _setTextCallback?.Invoke(_DetectedTagsList);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.WriteExceptionLogs(ex);
        }
    }
}
