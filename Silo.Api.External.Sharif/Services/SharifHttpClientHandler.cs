using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Silo.Api.External.Sharif.Models;

namespace Silo.Api.External.Sharif.Services;

public class SharifHttpClientHandler
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<SharifHttpClientHandler> _logger;
    private const string LogPrefix = "[SHARIF_API]";

    public SharifHttpClientHandler(HttpClient httpClient, ILogger<SharifHttpClientHandler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }

    public async Task<SharifApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        
        try
        {
            _logger.LogInformation(
                "{LogPrefix} [GET] Request started - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}",
                LogPrefix, correlationId, endpoint);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            return await ProcessResponseAsync<T>(response, correlationId, "GET", endpoint, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "{LogPrefix} [GET] Network error - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(500, "Network error occurred", ex.Message, correlationId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(
                "{LogPrefix} [GET] Request timeout - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(408, "Request timeout", ex.Message, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "{LogPrefix} [GET] Unexpected error - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(500, "Unexpected error occurred", ex.Message, correlationId);
        }
    }

    public async Task<SharifApiResponse<T>> PostAsync<T>(string endpoint, object? content, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        
        try
        {
            var requestBody = content != null ? JsonSerializer.Serialize(content, _jsonOptions) : "null";
            
            _logger.LogInformation(
                "{LogPrefix} [POST] Request started - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, RequestBody: {RequestBody}",
                LogPrefix, correlationId, endpoint, requestBody);

            var response = await _httpClient.PostAsJsonAsync(endpoint, content, _jsonOptions, cancellationToken);
            return await ProcessResponseAsync<T>(response, correlationId, "POST", endpoint, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "{LogPrefix} [POST] Network error - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(500, "Network error occurred", ex.Message, correlationId);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(
                "{LogPrefix} [POST] Request timeout - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(408, "Request timeout", ex.Message, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "{LogPrefix} [POST] Unexpected error - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, Error: {ErrorMessage}",
                LogPrefix, correlationId, endpoint, ex.Message);
            
            return CreateErrorResponse<T>(500, "Unexpected error occurred", ex.Message, correlationId);
        }
    }

    private async Task<SharifApiResponse<T>> ProcessResponseAsync<T>(
        HttpResponseMessage response,
        string correlationId,
        string method,
        string endpoint,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<T>(responseBody, _jsonOptions);
                
                _logger.LogInformation(
                    "{LogPrefix} [{Method}] Request completed successfully - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, StatusCode: {StatusCode}, ResponseBody: {ResponseBody}",
                    LogPrefix, method, correlationId, endpoint, statusCode, responseBody);
                
                return new SharifApiResponse<T>
                {
                    IsSuccess = true,
                    Data = data,
                    StatusCode = statusCode
                };
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx,
                    "{LogPrefix} [{Method}] JSON deserialization error - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, StatusCode: {StatusCode}, Error: {ErrorMessage}",
                    LogPrefix, method, correlationId, endpoint, statusCode, jsonEx.Message);
                
                return CreateErrorResponse<T>(500, "Response deserialization failed", jsonEx.Message, correlationId);
            }
        }

        var errorResponse = await TryParseErrorResponseAsync(response, correlationId, method, endpoint, cancellationToken);
        
        _logger.LogWarning(
            "{LogPrefix} [{Method}] Request failed - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
            LogPrefix, method, correlationId, endpoint, statusCode, errorResponse.Code, errorResponse.Message);
        
        return new SharifApiResponse<T>
        {
            IsSuccess = false,
            Error = errorResponse,
            StatusCode = statusCode
        };
    }

    private async Task<SharifErrorResponse> TryParseErrorResponseAsync(
        HttpResponseMessage response,
        string correlationId,
        string method,
        string endpoint,
        CancellationToken cancellationToken)
    {
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            _logger.LogDebug(
                "{LogPrefix} [{Method}] Raw error response - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}, ErrorContent: {ErrorContent}",
                LogPrefix, method, correlationId, endpoint, errorContent);
            
            var errorResponse = JsonSerializer.Deserialize<SharifErrorResponse>(errorContent, _jsonOptions);
            
            if (errorResponse != null)
            {
                errorResponse.TraceId = string.IsNullOrEmpty(errorResponse.TraceId) 
                    ? correlationId 
                    : $"{errorResponse.TraceId}|{correlationId}";
                return errorResponse;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex,
                "{LogPrefix} [{Method}] Error parsing error response - CorrelationId: {CorrelationId}, Endpoint: {Endpoint}",
                LogPrefix, method, correlationId, endpoint);
        }

        return new SharifErrorResponse
        {
            Code = response.StatusCode.ToString(),
            Message = response.ReasonPhrase ?? "An error occurred",
            TraceId = correlationId
        };
    }

    private SharifApiResponse<T> CreateErrorResponse<T>(int statusCode, string message, string details, string correlationId)
    {
        return new SharifApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Error = new SharifErrorResponse
            {
                Code = statusCode.ToString(),
                Message = message,
                TraceId = correlationId,
                Details = new List<SharifErrorDetail>
                {
                    new SharifErrorDetail
                    {
                        Message = details,
                        Code = statusCode.ToString()
                    }
                }
            }
        };
    }
}
