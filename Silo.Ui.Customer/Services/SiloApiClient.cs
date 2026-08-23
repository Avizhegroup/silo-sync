using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Silo.Application.Dto;
using Silo.Shared.JsonConverters;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Ui.Customer.Services;

/// <summary>
/// Custom HTTP client implementation for communicating with Silo.Api.Bypass endpoints
/// Replaces RfidConnectApi functionality with direct HTTP client calls
/// </summary>
public class SiloApiClient : ISiloApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<SiloApiClient> _logger;
    private readonly ProtectedLocalStorage _storage;
    private readonly JsonSerializerOptions _defaultJsonOptions;

    public SiloApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        NavigationManager navigationManager,
        ILogger<SiloApiClient> logger,
        ProtectedLocalStorage storage)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _navigationManager = navigationManager;
        _logger = logger;
        _storage = storage;
        
        // Configure default JSON options to match RfidConnectApi behavior
        _defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new NullableDateTimeConverter() }
        };

        // Set base address if not already set
        ConfigureBaseAddress();
    }

    private void ConfigureBaseAddress()
    {
        if (_httpClient.BaseAddress == null)
        {
            var baseUri = GetBaseUri();
            _httpClient.BaseAddress = new Uri(baseUri);
        }
    }

    private string GetBaseUri()
    {
        var uri = _configuration.GetSection("RfidConnectApi").GetSection("Uri");
        var ip = _configuration.GetSection("RfidConnectApi")["Ip"];

        if (string.IsNullOrEmpty(uri.Value))
        {
            return $"http://{ip}/api/v2/";
        }
        else
        {
            return $"http://{ip}{uri.Value}";
        }
    }

    private string? GetAnonymousBaseUri()
    {
        return _configuration.GetSection("RfidConnectApi")["IpAno"];
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string methodName, params KeyValuePair<string, object>[] data)
    {
        return await PostAsyncByUri<T>("Wms/PostObject", methodName, data);
    }

    public async Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, string methodName, params KeyValuePair<string, object>[] data)
    {
        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var requestPayload = new Dictionary<string, object>
        {
            {"interface", "RestAPI"},
            {"method", methodName},
            {"parameters", dataDict}
        };

        return await SendRequestAsync<T>(uri, requestPayload, _defaultJsonOptions);
    }

    public async Task<ApiResponse<T>> PostAsyncByBaseUrlAndUri<T>(string configKey, string uri, string methodName, params KeyValuePair<string, object>[] data)
    {
        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var requestPayload = new Dictionary<string, object>
        {
            {"interface", "RestAPI"},
            {"method", methodName},
            {"parameters", dataDict}
        };

        var baseUrl = _configuration[configKey];
        return await SendRequestToBaseUrlAsync<T>(baseUrl, uri, requestPayload, _defaultJsonOptions);
    }

    public async Task<ApiResponse<T>> PostAsyncByBaseUrlAndUriAndContext<T>(string configKey, string uri, string methodName, JsonSerializerContext context, params KeyValuePair<string, object>[] data)
    {
        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var requestPayload = new Dictionary<string, object>
        {
            {"interface", "RestAPI"},
            {"method", methodName},
            {"parameters", dataDict}
        };

        var options = new JsonSerializerOptions(_defaultJsonOptions)
        {
            TypeInfoResolver = context
        };

        var baseUrl = _configuration[configKey];
        return await SendRequestToBaseUrlAsync<T>(baseUrl, uri, requestPayload, options);
    }

    public async Task<ApiResponse<T>> PostAsyncByUriAndContext<T>(string uri, string methodName, JsonSerializerContext context, params KeyValuePair<string, object>[] data)
    {
        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        var requestPayload = new Dictionary<string, object>
        {
            {"interface", "RestAPI"},
            {"method", methodName},
            {"parameters", dataDict}
        };

        var options = new JsonSerializerOptions(_defaultJsonOptions)
        {
            TypeInfoResolver = context
        };

        return await SendRequestAsync<T>(uri, requestPayload, options);
    }

    public async Task<ApiResponse<T>> SendAsyncObjectByUri<T>(HttpMethod method, string uri, object? data = null)
    {
        return await SendRequestAsync<T>(uri, data, _defaultJsonOptions, method);
    }

    public async Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, params KeyValuePair<string, object>[] data)
    {
        var dataDict = new Dictionary<string, object>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        return await SendRequestAsync<T>(uri, dataDict, _defaultJsonOptions);
    }

    private async Task<ApiResponse<T>> SendRequestAsync<T>(string uri, object? data, JsonSerializerOptions options, HttpMethod? method = null)
    {
            method ??= HttpMethod.Post;

            await AddAuthorizationHeaderAsync();
            var json = JsonSerializer.Serialize(data, _defaultJsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(method, uri) { Content = content };
            
            var response = await _httpClient.SendAsync(request);

            await HandleResponseStatusAsync(response);

            if (response.IsSuccessStatusCode)
            {
                // Handle HTTP 204 No Content specifically
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Received HTTP 204 No Content response for {Uri}", uri);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                // Check if there's content to read
                var contentLength = response.Content.Headers.ContentLength;
                if (contentLength == 0)
                {
                    _logger.LogInformation("Received empty content response for {Uri}", uri);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                var responseStream = await response.Content.ReadAsStreamAsync();
                
                if (responseStream.Length == 0)
                {
                    _logger.LogInformation("Received empty stream response for {Uri}", uri);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(responseStream, options);
                return result ?? new ApiResponse<T> { Successful = false };
            }
            else
            {
                _logger.LogWarning("HTTP request failed with status: {StatusCode}", response.StatusCode);
                return new ApiResponse<T> 
                { 
                    Successful = false, 
                    Messages = new[] { $"HTTP {response.StatusCode}: {response.ReasonPhrase}" }
                };
            }
    }

    private async Task<ApiResponse<T>> SendRequestToBaseUrlAsync<T>(string baseUrl, string uri, object data, JsonSerializerOptions options)
    {
            using var client = new HttpClient();
            
            await AddAuthorizationHeaderAsync(client);

            var json = JsonSerializer.Serialize(data, _defaultJsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var fullUrl = baseUrl.TrimEnd('/') + "/" + uri.TrimStart('/');
            var response = await client.PostAsync(fullUrl, content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Unauthorized access to anonymous endpoint: {Url}", fullUrl);
            }

            if (response.IsSuccessStatusCode)
            {
                // Handle HTTP 204 No Content specifically
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Received HTTP 204 No Content response for {Url}", fullUrl);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                var contentLength = response.Content.Headers.ContentLength;
                if (contentLength == 0)
                {
                    _logger.LogInformation("Received empty content response for {Url}", fullUrl);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                var responseStream = await response.Content.ReadAsStreamAsync();
                
                if (responseStream.Length == 0)
                {
                    _logger.LogInformation("Received empty stream response for {Url}", fullUrl);
                    return new ApiResponse<T> 
                    { 
                        Successful = true,
                        Value = default(T)
                    };
                }

                var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(responseStream, options);
                return result ?? new ApiResponse<T> { Successful = false };
            }
            else
            {
                _logger.LogWarning("HTTP request to {Url} failed with status: {StatusCode}", fullUrl, response.StatusCode);
                return new ApiResponse<T> 
                { 
                    Successful = false, 
                    Messages = new[] { $"HTTP {response.StatusCode}: {response.ReasonPhrase}" }
                };
            }
    }

    private async Task AddAuthorizationHeaderAsync(HttpClient client = null)
    {
            var targetClient = client ?? _httpClient;
            var storageResult = await _storage.GetAsync<string>("jwt");

            if (storageResult.Success && storageResult.Value.HasValue())
            {
                targetClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storageResult.Value);
            }
            else
            {
                targetClient.DefaultRequestHeaders.Authorization = null;
            }
    }

    private async Task HandleResponseStatusAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            try
            {
                await _storage.DeleteAsync("token");
                await _storage.DeleteAsync("username");
                await _storage.DeleteAsync("jwt");
                await _storage.DeleteAsync("signTime");

                _navigationManager.NavigateTo("/account/login", true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear storage on unauthorized response");
            }
        }
        else if (response.StatusCode == HttpStatusCode.Ambiguous)
        {
            _navigationManager.NavigateTo("/settings/apisettings");
        }

        if (!response.IsSuccessStatusCode && response.Content?.Headers?.ContentType?.MediaType == "application/json")
        {
            try
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                _logger.LogWarning("API error response: {Error}", errorResult);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse error response");
            }
        }
    }
}
