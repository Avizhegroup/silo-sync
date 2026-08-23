using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Silo.Application.Dto;

namespace Silo.Jobs.Win.Services;
public partial class Api(IConfiguration Configuration) : HttpClientHandler
{
    private string baseUri;
    private const int bufferSize = 4096;

    #region PostAsync
    public async Task<ApiResponse<T>> PostAsync<T>(string methodName, params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/PostObject");

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        Stream httpStream = await resultStream.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream, option);

        return result;
    }

    public async Task<string> PostAsync(string methodName, params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/v");

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        return await resultStream.Content.ReadAsStringAsync();
    }

    public async Task<Stream> PostAsync(string uri, string fileName)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/" + uri);

        var result = await SendAsync(request, new());

        Stream stream = await result.Content.ReadAsStreamAsync();

        return stream;
    }

    public async Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, string methodName, params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + uri);

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        Stream httpStream = await resultStream.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream, option);

        return result;
    }

    public async Task<ApiResponse<T>> PostAsyncByUriAndContext<T>(string uri, string methodName, JsonSerializerContext context,  params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            TypeInfoResolver = context
        };

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + uri);

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        Stream httpStream = await resultStream.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream, option);

        return result;
    }

    public async Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();

        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var passDataJsonString = JsonSerializer.Serialize(dataDict);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + uri);

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        Stream httpStream = await resultStream.Content.ReadAsStreamAsync();
        using StreamReader sr = new StreamReader(httpStream);

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream, option);
        return result;
    }

    public async Task<ApiResponse<T>> PostAsyncByContext<T>(string methodName, JsonSerializerContext context, params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = context,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/PostObject");

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        var result = (ApiResponse<T>)await resultStream.Content.ReadFromJsonAsync(typeof(ApiResponse<T>), option);

        return result;
    }

    public async Task<ApiResponse<T>> PostAsyncByContextAndOption<T>(string methodName
        , JsonSerializerContext context
        , JsonSerializerOptions options
        , params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        options.PropertyNameCaseInsensitive = true;

        options.TypeInfoResolver = context;

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/PostObject");

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        var result = (ApiResponse<T>)await resultStream.Content.ReadFromJsonAsync(typeof(ApiResponse<T>), options);

        return result;
    }

    public async Task<ApiResponse<T>> PostAsyncByOption<T>(string methodName
        , JsonSerializerOptions options
        , params KeyValuePair<string, object>[] data)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var finalDic = new Dictionary<string, object>();
        finalDic.Add("interface", "RestAPI");
        finalDic.Add("method", methodName);
        finalDic.Add("parameters", dataDict);

        var passDataJsonString = JsonSerializer.Serialize(finalDic);
        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/PostObject");

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        var result = (ApiResponse<T>)await resultStream.Content.ReadFromJsonAsync(typeof(ApiResponse<T>), options);

        return result;
    }

    public async Task<ApiResponse<T>> PostFileAsync<T>(string uri, string filePath, params KeyValuePair<string, string>[] headers)
    {
        if (baseUri.HasNoValue())
        {
            SetUri();
        }

        byte[] file = File.ReadAllBytes(filePath);

        var byteContent = new ByteArrayContent(file);

        using MultipartFormDataContent multipartContent = new();

        multipartContent.Add(byteContent, "file", filePath.Split("\\").Last());

        foreach (KeyValuePair<string, string> header in headers)
        {
            multipartContent.Headers.Add(header.Key, new List<string>() { header.Value });
        }

        HttpRequestMessage request = new(HttpMethod.Post, baseUri + "Wms/" + uri);

        request.Content = multipartContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        using Stream httpStream = await resultStream.Content.ReadAsStreamAsync();

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream);

        return result;
    }
    #endregion

    private void SetUri()
    {
        var uri = Configuration["RfidConnectApi:Uri"];

        if (uri.HasNoValue())
        {
            baseUri = $"http://{Configuration["RfidConnectApi:Ip"]}/RfidCore/v2/";
        }
        else
        {
            baseUri = $"http://{Configuration["RfidConnectApi:Ip"]}{uri}";
        }
    }
}
