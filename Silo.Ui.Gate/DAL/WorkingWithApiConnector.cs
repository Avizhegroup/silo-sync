using Newtonsoft.Json.Linq;

namespace Silo.Ui.Gate.DAL;

public class WorkingWithApiConnector
{
    #region Async
    internal static async Task<JToken> CreateSendDataAsync(string url, string data)
    {
        return JToken.Parse(await Connector.ConnectAsync((object)data, url));
    }

    internal static async Task<JToken> CreateSendDataAsync(string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
      
        foreach (KeyValuePair<string, object> item in dataList)
        {
            dataDictionary.Add(item.Key, item.Value); 
        }

        return JToken.Parse(await Connector.ConnectAsync(dataDictionary, url));
    }
    #endregion

    #region Dictionary async
    internal static async Task<JToken> CreateSendDataDictionaryAsync(string methodName, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, url));
    }
    internal static async Task<JToken> CreateSendDataDictionaryAsync(string methodName, string bearer, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, url, bearer));
    }
    internal static async Task<JToken> CreateSendDataDictionaryAsync(string methodName, string url, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, timeout, url));
    }
    internal static async Task<JToken> CreateSendDataDictionaryAsync(string methodName, string bearer, string url, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, url, timeout, bearer));
    }
    #endregion

    #region Dictionary
    internal static JToken CreateSendDataDictionary(string methodName, string url, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, timeout, url));
    }
    internal static JToken CreateSendDataDictionary(string methodName, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, url));
    }
    internal static JToken CreateSendDataDictionary(string methodName, string bearer, string url, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, timeout, url, bearer));
    }
    internal static JToken CreateSendDataDictionary(string methodName, string bearer, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, url, bearer));
    }
    internal static JToken CreateSendDataDictionary(string methodName, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, sendMethod, timeout, url));
    }
    internal static JToken CreateSendDataDictionary(string methodName, string bearer, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, sendMethod, timeout, url, bearer));
    }
    #endregion

    #region Send Direct Async
    internal static async Task<JToken> SendDataDirectAsync(object content, string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, url));
    }
    internal static async Task<JToken> SendDataDirectAsync(object content, string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, url, bearer));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, url));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, url, bearer));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, int timeout, string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, timeout, url));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, int timeout, string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, timeout, url, bearer));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, sendMethod, timeout, url));
    }
    internal static async Task<JToken> SendDataDirectAsync(object content, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, sendMethod, timeout, url));
    }
    internal static async Task<JToken> SendDataDirectAsync(object content, string sendMethod, int timeout, string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, sendMethod, timeout, url, bearer));
    }
    internal static async Task<JToken> SendDataDirectAsync(string content, string sendMethod, int timeout, string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, sendMethod, timeout, url, bearer));
    }
    internal static async Task<JToken> SendDirectAsync(string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(url));
    }
    internal static async Task<JToken> SendDirectAsync(string url, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, timeout));
    }
    internal static async Task<JToken> SendDirectAsync(string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, bearer));
    }
    internal static async Task<JToken> SendDirectAsync(string url, string bearer, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, timeout, bearer));
    }
    #endregion

    #region Send Direct
    internal static async Task<JToken> SendDirect(string url)
    {
        return JToken.Parse(await Connector.ConnectAsync(url));
    }
    internal static async Task<JToken> SendDirect(string url, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, timeout));
    }
    internal static async Task<JToken> SendDirect(string url, string bearer)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, bearer));
    }
    internal static async Task<JToken> SendDirect(string url, string bearer, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(url, timeout, bearer));
    }
    internal static JToken SendDataDirect(string content, string url)
    {
        return JToken.Parse(Connector.Connect(content, url));
    }
    internal static JToken SendDataDirect(string content, string url, int timeout)
    {
        return JToken.Parse(Connector.Connect(content, url, timeout));
    }
    internal static JToken SendDataDirect(object content, string url)
    {
        return JToken.Parse(Connector.Connect(content, url));
    }
    internal static JToken SendDataDirect(object content, string url, int timeout)
    {
        return JToken.Parse(Connector.Connect(content, url, timeout));
    }
    internal static JToken SendDataDirect(string content, string bearer, string url)
    {
        return JToken.Parse(Connector.Connect(content, url, bearer));
    }
    internal static JToken SendDataDirect(object content, string bearer, string url)
    {
        return JToken.Parse(Connector.Connect(content, url, bearer));
    }
    internal static JToken SendDataDirect(string content, string bearer, string url, int timeout)
    {
        return JToken.Parse(Connector.Connect(content, timeout, url, bearer));
    }
    internal static JToken SendDataDirect(object content, string bearer, string url, int timeout)
    {
        return JToken.Parse(Connector.Connect(content, timeout, url, bearer));
    }
    internal static JToken SendDataDirect(string content, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(Connector.Connect(content, sendMethod, timeout, url));
    }
    internal static JToken SendDataDirect(object content, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(Connector.Connect(content, sendMethod, timeout, url));
    }
    internal static JToken SendDataDirect(string content, string bearer, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(Connector.Connect(content, sendMethod, timeout, url, bearer));
    }
    internal static JToken SendDataDirect(object content, string bearer, string sendMethod, int timeout, string url)
    {
        return JToken.Parse(Connector.Connect(content, sendMethod, timeout, url, bearer));
    }
    #endregion

    #region Send Dictionary Async
    internal static async Task<JToken> CreateSendDataDictionaryByMethodAsync(string methodName, string sendMethod, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectByMethodAsync(finalDic, sendMethod, url));
    }
    internal static async Task<JToken> CreateSendDataDictionaryByMethodAsync(string methodName, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, sendMethod, timeout, url));
    }
    internal static async Task<JToken> CreateSendDataDictionaryByMethodAsync(string methodName, string bearer, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, sendMethod, timeout, url, bearer));
    }
    #endregion

    #region Send String
    internal static async Task<string> SendDataDirectAsyncString(string content, string url, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, timeout, url)).ToString();
    }
    internal static async Task<string> SendDataDirectAsyncString(string content, string bearer, string url, int timeout)
    {
        return JToken.Parse(await Connector.ConnectAsync(content, timeout, url, bearer)).ToString();
    }
    internal static string SendDataDirectString(string content, string url)
    {
        return JToken.Parse(Connector.Connect(content, url)).ToString();
    }
    internal static string SendDataDirectString(string content, string bearer, string url)
    {
        return JToken.Parse(Connector.Connect(content, url, bearer)).ToString();
    }
    #endregion

    #region Send Data Get String
    internal static async Task<string> CreateSendDataDictionaryAsyncString(string methodName, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, url)).ToString();
    }
    internal static async Task<string> CreateSendDataDictionaryAsyncString(string methodName, string url, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(await Connector.ConnectAsync(finalDic, timeout, url)).ToString();
    }
    internal static string CreateSendDataDictionaryString(string methodName, string url, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, url)).ToString();
    }
    internal static string CreateSendDataDictionaryString(string methodName, string url, string bearer, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, url, bearer)).ToString();
    }
    internal static string CreateSendDataDictionaryString(string methodName, string url, string bearer, int timeout, params KeyValuePair<string, object>[] dataList)
    {
        var dataDictionary = new Dictionary<string, object>();
        foreach (KeyValuePair<string, object> item in dataList)
            dataDictionary.Add(item.Key, item.Value);
        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDictionary);
        return JToken.Parse(Connector.Connect(finalDic, url, timeout, bearer)).ToString();
    }
    #endregion

    #region Load Direct
    public static string LoadDirect(string contents, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(string contents, string bearer, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, bearer,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(object contents, string bearer, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, bearer,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(object contents, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(string contents, string sendMethod, int timeout, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, sendMethod, timeout,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(object contents, string sendMethod, int timeout, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, sendMethod, timeout,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(string contents, string bearer, string sendMethod, int timeout, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, bearer, sendMethod, timeout,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadDirect(object contents, string bearer, string sendMethod, int timeout, string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDataDirect(contents, bearer, sendMethod, timeout,
                url).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(string contents, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents,
                url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(object contents, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents,
                url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(string contents, string bearer, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents,
                bearer, url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(object contents, string bearer, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents, bearer,
                url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(string contents, string sendMethod, int timeout, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents, sendMethod, timeout,
                url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(object contents, string sendMethod, int timeout, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents, sendMethod, timeout,
                url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(string contents, string bearer, string sendMethod, int timeout, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents, sendMethod, timeout,
                url, bearer)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadDirectAsync(object contents, string bearer, string sendMethod, int timeout, string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDataDirectAsync(contents, sendMethod, timeout,
                url, bearer)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    #endregion

    #region Load Direct Empty
    public static string LoadEmptyDirect(string url)
    {
        try
        {
            return WorkingWithApiConnector.SendDirect(url).GetAwaiter().GetResult().ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadEmptyDirect(string url, int timeout)
    {
        try
        {
            return WorkingWithApiConnector.SendDirect(url, timeout).GetAwaiter().GetResult().ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadEmptyDirect(string url, string bearer)
    {
        try
        {
            return WorkingWithApiConnector.SendDirect(url, bearer).GetAwaiter().GetResult().ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static string LoadEmptyDirect(string url, string bearer, int timeout)
    {
        try
        {
            return WorkingWithApiConnector.SendDirect(url, bearer, timeout).GetAwaiter().GetResult().ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadEmptyDirectAsync(string url)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDirectAsync(url)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadEmptyDirectAsync(string url, int timeout)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDirect(url, timeout)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadEmptyDirectAsync(string url, string bearer)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDirect(url, bearer)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    public static async Task<string> LoadEmptyDirectAsync(string url, string bearer, int timeout)
    {
        try
        {
            return (await WorkingWithApiConnector.SendDirect(url, bearer, timeout)).ToString();
        }
        catch (Exception e)
        {
            return "{\"Successful\": true,\"Value\": false}";
        }
    }
    #endregion

    #region Load Undirect
    public static JToken LoadUnDirect(string methodName, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static JToken LoadUnDirect(string methodName, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName, sendMethod, timeout,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static JToken LoadUnDirect(string methodName, string bearer, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName, bearer,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static JToken LoadUnDirect(string methodName, string bearer, string url, int timeout, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName, bearer,
                url, timeout, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static JToken LoadUnDirect(string methodName, string bearer, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName, bearer, sendMethod, timeout,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static JToken LoadUnDirect(string methodName, string url, int timeout, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return WorkingWithApiConnector.CreateSendDataDictionary(methodName,
                url, timeout, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }

    public static async Task<JToken> LoadUnDirectAsync(string methodName, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryAsync(methodName,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string url, string data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataAsync(url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataAsync(url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string methodName, string url, int timeout, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryAsync(methodName,
                url, timeout, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string methodName, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryByMethodAsync(methodName, sendMethod, timeout,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string methodName, string bearer, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryAsync(methodName, bearer,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string methodName, string bearer, string url, int timeout, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryAsync(methodName, bearer,
                url, timeout, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    public static async Task<JToken> LoadUnDirectAsync(string methodName, string bearer, string sendMethod, int timeout, string url, params KeyValuePair<string, object>[] data)
    {
        try
        {
            return await WorkingWithApiConnector.CreateSendDataDictionaryByMethodAsync(methodName, bearer, sendMethod, timeout,
                url, data);
        }
        catch (Exception e)
        {
            return JToken.Parse("{\"Successful\": true,\"Value\": false}");
        }
    }
    #endregion
}
