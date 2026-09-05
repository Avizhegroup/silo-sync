using System.Net.Http.Json;
using Silo.Application.Dto;
using Silo.Application.Features;

namespace Silo.Services;

public class SyncAdminApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SyncAdminApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    private string BaseUrl => $"http://{_configuration.GetSection("RfidConnectApi")["Ip"]}/RfidCore/v2/SyncAdmin";

    public async Task<List<GetSyncSourcesVm>> GetSourcesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<GetSyncSourcesVm>>>($"{BaseUrl}/sources");
        return response?.Value ?? new List<GetSyncSourcesVm>();
    }

    public async Task<CreateSyncSourceVm> CreateSourceAsync(CreateSyncSourceCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sources", command);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<CreateSyncSourceVm>>())?.Value ?? new CreateSyncSourceVm();
    }

    public async Task<UpdateSyncSourceVm> UpdateSourceAsync(int id, UpdateSyncSourceCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/sources/{id}", command);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<UpdateSyncSourceVm>>())?.Value ?? new UpdateSyncSourceVm();
    }

    public async Task<DeleteSyncSourceVm> DeleteSourceAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/sources/{id}");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<DeleteSyncSourceVm>>())?.Value ?? new DeleteSyncSourceVm();
    }

    public async Task<EnableDisableSyncSourceVm> SetSourceEnabledAsync(int id, bool isEnabled)
    {
        var action = isEnabled ? "enable" : "disable";
        var response = await _httpClient.PostAsync($"{BaseUrl}/sources/{id}/{action}", null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<EnableDisableSyncSourceVm>>())?.Value ?? new EnableDisableSyncSourceVm();
    }

    public async Task<TestSyncSourceQueryVm> TestSourceAsync(int id)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sources/{id}/test-query", new TestSyncSourceQueryCommand());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<TestSyncSourceQueryVm>>())?.Value ?? new TestSyncSourceQueryVm();
    }

    public async Task<List<GetSyncRunHistoryVm>> GetRunHistoryAsync(GetSyncRunHistoryQuery query)
    {
        var url = $"{BaseUrl}/runs?sourceKey={Uri.EscapeDataString(query.SourceKey ?? "")}";
        if (query.From.HasValue)
        {
            url += $"&from={Uri.EscapeDataString(query.From.Value.ToString("O"))}";
        }
        if (query.To.HasValue)
        {
            url += $"&to={Uri.EscapeDataString(query.To.Value.ToString("O"))}";
        }
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<GetSyncRunHistoryVm>>>(url);
        return response?.Value ?? new List<GetSyncRunHistoryVm>();
    }

    public async Task<List<GetOpenSyncFailuresVm>> GetFailuresAsync(string? status = null)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<GetOpenSyncFailuresVm>>>($"{BaseUrl}/failures?status={Uri.EscapeDataString(status ?? "Pending")}");
        return response?.Value ?? new List<GetOpenSyncFailuresVm>();
    }

    public async Task<RetrySyncRowFailureVm> RetryFailureAsync(int id)
    {
        var response = await _httpClient.PostAsync($"{BaseUrl}/failures/{id}/retry", null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<RetrySyncRowFailureVm>>())?.Value ?? new RetrySyncRowFailureVm();
    }
}
