using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Silo.Ui.Gate.DAL;

internal class Connector
{
    private const int bufferSize = 4096;
    internal static async Task<string> ConnectAsync(object passedData, string sendMethod, int timeout, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = sendMethod.ToUpper();
        req.Timeout = timeout;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectByMethodAsync(object passedData, string sendMethod, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = sendMethod.ToUpper();
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(object passedData, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(string url, int timeout)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(string url, int timeout, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(object passedData, int timeout, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(object passedData, int timeout, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(object passedData, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static async Task<string> ConnectAsync(object passedData, string sendMethod, int timeout, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "Url is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = sendMethod.ToUpper();
        req.Timeout = timeout;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        var outStream = await req.GetRequestStreamAsync();
        var outStreamWriter = new StreamWriter(outStream);
        await outStreamWriter.WriteAsync(JsonConvert.SerializeObject(passedData));
        await outStreamWriter.FlushAsync();
        outStream.Close();
        WebResponse res = await req.GetResponseAsync();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            var readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                await memoryStream.WriteAsync(buff, 0, readedBytes);
                readedBytes = await httpStream.ReadAsync(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }
            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }


    internal static string Connect(object passedData, string sendMethod, int timeout, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = sendMethod.ToUpper();
        req.Timeout = timeout;
        req.ContentType = "application/json";
        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static string Connect(object passedData, string sendMethod, int timeout, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = sendMethod.ToUpper();
        req.Timeout = timeout;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }
        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static string Connect(object passedData, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }



        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";

        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }

        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static string Connect(object passedData, string url, int timeout)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }



        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";

        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }

        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static string Connect(object passedData, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = 60_000;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }

        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
    internal static string Connect(object passedData, int timeout, string url)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }

        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }

    internal static string Connect(object passedData, int timeout, string url, string bearer)
    {
        if (url == string.Empty)
        {
            var errorDic = new Dictionary<string, object>();
            errorDic.Add("Successful", "false");
            errorDic.Add("Successful", "ServerIP is not set");
            return JsonConvert.SerializeObject(errorDic);
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.AllowWriteStreamBuffering = true;
        req.Method = "POST";
        req.Timeout = timeout;
        req.ContentType = "application/json";
        req.Headers.Add("Authorization", "Bearer " + bearer);
        Stream outStream = req.GetRequestStream();
        StreamWriter outStreamWriter = new StreamWriter(outStream);
        outStreamWriter.Write(JsonConvert.SerializeObject(passedData));
        outStreamWriter.Flush();
        outStream.Close();
        WebResponse res = req.GetResponse();
        Stream httpStream = res.GetResponseStream();
        MemoryStream memoryStream = new MemoryStream();
        try
        {
            byte[] buff = new byte[bufferSize];
            int readedBytes = httpStream.Read(buff, 0, buff.Length);
            while (readedBytes > 0)
            {
                memoryStream.Write(buff, 0, readedBytes);
                readedBytes = httpStream.Read(buff, 0, buff.Length);
            }
        }
        finally
        {
            if (httpStream != null)
            {
                httpStream.Close();
            }

            if (memoryStream != null)
            {
                memoryStream.Close();
            }
        }

        byte[] data = memoryStream.ToArray();
        string result = Encoding.UTF8.GetString(data, 0, data.Length);
        return result;
    }
}
