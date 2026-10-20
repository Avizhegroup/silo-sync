using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Windows.Register;

/// <summary>
/// Texts shared with the web application, loaded the same way the web app does it (FormalDataCache.GetTextResources):
/// one anonymous POST to <c>TextResource/ReadAll</c> of the Silo API at start-up, kept in memory in
/// <see cref="Silo.Shared.ResourceManager"/> and read through <see cref="Silo.Shared.TextResources"/>.
/// The API address is the <c>ServerIP</c> of the host settings (<c>http://{ServerIP}/RfidCore/v2/</c>, like the web app).
/// When the server cannot be reached, every text keeps its English fallback, so the host never fails because of a text.
/// </summary>
internal static class Localizer
{
    private const string Endpoint = "TextResource/ReadAll";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed class ApiResult
    {
        [JsonPropertyName("successful")] public bool Successful { get; set; }
        [JsonPropertyName("value")] public List<TextItem>? Value { get; set; }
    }

    private sealed class TextItem
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }

    public static bool IsLoaded => Silo.Shared.ResourceManager.IsLoaded;

    /// <returns>true when the texts were loaded; false (with <paramref name="error"/>) when the server did not give them.</returns>
    public static async Task<(bool Loaded, string Error)> LoadAsync(string? serverIp)
    {
        if (string.IsNullOrWhiteSpace(serverIp))
        {
            return (false, "The server address is not set.");
        }

        try
        {
            using var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            using var response = await Http.PostAsync($"http://{serverIp.Trim()}/RfidCore/v2/{Endpoint}", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResult>(JsonOptions);
            if (result is not { Successful: true, Value: not null })
            {
                return (false, "The server returned no texts.");
            }

            var values = new Dictionary<string, string?>(StringComparer.Ordinal);
            foreach (var item in result.Value.Where(p => !string.IsNullOrEmpty(p.Key)))
            {
                values[item.Key!] = item.Value;
            }

            Silo.Shared.ResourceManager.Load(values);
            return (true, string.Empty);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException
            or InvalidOperationException or UriFormatException)
        {
            return (false, exception.Message);
        }
    }

    /// <summary>The resource text, or <paramref name="fallback"/> when the texts were not loaded or the key is empty.</summary>
    public static string Text(string? resource, string fallback)
        => string.IsNullOrWhiteSpace(resource) ? fallback : resource;
}
