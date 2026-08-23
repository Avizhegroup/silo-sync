using System.Text;

namespace Silo.Service.Sharif.Dtos;

public class FileManage
{

    static string path = System.Environment.CurrentDirectory + "\\log.txt";
    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="data">数据</param>
    /// <param name="appdend">是否将数据追加到文件末尾</param>
    public static void WriterFile(string path,string data,bool appdend)
    {
        try
        {
            StreamWriter sw = new StreamWriter(path, appdend);
            sw.Write(data);
             
            sw.Close();
        }
        catch (System.Exception ex)
        {
        	
        }
    }
    public static byte[] ReadFileToBytes(string path)
    {

        FileStream fsr = new FileStream(path, FileMode.Open);
        //开辟内存区域 1024 * 1024 bytes
        byte[] readBytes = new byte[1024 * 1024];
        //开始读数据
        int count = fsr.Read(readBytes, 0, readBytes.Length);
        //关闭文件流
        fsr.Close();

        byte[] byteData = new byte[count];
        Array.Copy(readBytes, 0, byteData, 0, count);
        return byteData;

    }

    public static string ReadFile(string path)
    {
        string data = "";
        StreamReader sr = new StreamReader(path, Encoding.Default);
        data = sr.ReadToEnd();
        sr.Close();
        return data;
    }


    /// <summary>
    /// 记录APP日志
    /// </summary>
    /// <param name="log"></param>
    public static void WriterLog(LogType type,string log)
    {
        WriterFile(path, log,true);
   
    }


    public enum LogType {
       Error,
       Debug
    
    }



  
 
}
