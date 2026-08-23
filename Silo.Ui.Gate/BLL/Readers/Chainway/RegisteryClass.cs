using System.Management;

namespace Silo.Ui.Gate.BLL;

class RegisteryClass
{

    public static int CheckRegistery()
    {
        try
        {
            Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\TDAvizhe\\RFIDConnect_antenna", true);
            //string str = regkey.GetValue("AgmOfVars").ToString();
            if (regkey.ValueCount == 0)
            {
                regkey.Close();
                regkey.Flush();
                return 3;
            }
            else if (CreateActiveCode().ToLower() == regkey.GetValue("RFIDConnectOfVars").ToString())
            {
                regkey.Close();
                regkey.Flush();
                return 1;
            }                 
            else
            {
                regkey.Close();
                regkey.Flush();
                return 3;
            }
        }
        catch
        {
            return 3;
        }
    }

    internal class mathFunctions
    {
        private string rev(string s)
        {
            char[] c = s.ToCharArray();
            string ts = "";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                ts += c[i].ToString();
            }

            return (ts);
        }

        internal string DectoHex(string s)
        {
            int temp = int.Parse(s);
            string str = "";

            while (temp > 0)
            {
                int i = (int)temp % 16;
                switch (i)
                {
                    case 10: str += 'A'; break;
                    case 11: str += 'B'; break;
                    case 12: str += 'C'; break;
                    case 13: str += 'D'; break;
                    case 14: str += 'E'; break;
                    case 15: str += 'F'; break;
                    default: str += i.ToString(); break;
                }
                temp /= 16;
            }

            return (rev(str));
        }
    }


    public static string GetCPUId()
    {
        string cpuId = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
        foreach (ManagementObject queryObj in searcher.Get())
        {
            cpuId = queryObj["ProcessorId"].ToString();
        }
        return cpuId.ToLower();
    }

    public static string CreateActiveCode()
    {
        char[] cpuIdArray = GetCPUId().ToArray();
        char[] sid = "356017".ToArray();
        int firstNumber = (int)cpuIdArray[0] + (int)cpuIdArray[15] + int.Parse(sid[0].ToString());
        int secondNumber = (int)cpuIdArray[1] + (int)cpuIdArray[13] + int.Parse(sid[4].ToString());
        int thirdNumber = (int)cpuIdArray[6] + (int)cpuIdArray[11] + int.Parse(sid[1].ToString());
        int fourthNumber = (int)cpuIdArray[7] + (int)cpuIdArray[4] + int.Parse(sid[2].ToString());
        int fifthNumber = (int)cpuIdArray[2] + (int)cpuIdArray[14] + int.Parse(sid[3].ToString());
        int sixNumber = (int)cpuIdArray[3] + (int)cpuIdArray[5] + int.Parse(sid[5].ToString());

        mathFunctions mf = new mathFunctions();
        string ActiveCode = mf.DectoHex(firstNumber.ToString());
        ActiveCode += mf.DectoHex(secondNumber.ToString());
        ActiveCode += mf.DectoHex(thirdNumber.ToString());
        ActiveCode += mf.DectoHex(fourthNumber.ToString());
        ActiveCode += mf.DectoHex(fifthNumber.ToString());
        ActiveCode += mf.DectoHex(sixNumber.ToString());

        return ActiveCode.ToLower();
    }

}
