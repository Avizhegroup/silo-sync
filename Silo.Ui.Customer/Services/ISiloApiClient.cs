using Silo.Application.Dto;
using System.Text.Json.Serialization;

namespace Silo.Ui.Customer.Services;

/// <summary>
/// Custom HTTP client interface for communicating with Silo.Api.Bypass endpoints
/// </summary>
public interface ISiloApiClient
{
    /// <summary>
    /// Posts data to authenticated API endpoints
    /// </summary>
    Task<ApiResponse<T>> PostAsync<T>(string methodName, params KeyValuePair<string, object>[] data);

    /// <summary>
    /// Posts data to authenticated API endpoints with custom URI
    /// </summary>
    Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, string methodName, params KeyValuePair<string, object>[] data);

    /// <summary>
    /// Posts data to anonymous API endpoints using base URL and URI from configuration
    /// </summary>
    Task<ApiResponse<T>> PostAsyncByBaseUrlAndUri<T>(string configKey, string uri, string methodName, params KeyValuePair<string, object>[] data);

    /// <summary>
    /// Posts data to anonymous API endpoints with JSON serializer context
    /// </summary>
    Task<ApiResponse<T>> PostAsyncByBaseUrlAndUriAndContext<T>(string configKey, string uri, string methodName, JsonSerializerContext context, params KeyValuePair<string, object>[] data);

    /// <summary>
    /// Posts data to authenticated API endpoints with JSON serializer context
    /// </summary>
    Task<ApiResponse<T>> PostAsyncByUriAndContext<T>(string uri, string methodName, JsonSerializerContext context, params KeyValuePair<string, object>[] data);

    /// <summary>
    /// Posts data directly to specified URI with custom data object
    /// </summary>
    Task<ApiResponse<T>> SendAsyncObjectByUri<T>(HttpMethod method, string uri, object? data = null);

    /// <summary>
    /// Posts raw data to URI without REST API wrapper
    /// </summary>
    Task<ApiResponse<T>> PostAsyncByUri<T>(string uri, params KeyValuePair<string, object>[] data);
}
