using System.Text.Json;
using Microsoft.Extensions.Logging;
using Silo.Application.Dto;

namespace Silo.Identity.Server;

public class MavadkaranSsoService(ILogger<MavadkaranSsoService> logger)
{
    const string authority = "https://mks.mapnamk.com/realms/mapnamk";
    const string clientId = "avijeh";
    const string clientSecret = "ooAeliBqWN924ZTnNry7uSlCehhsQVeA";

    public async Task<string> GetAccessTokenAsync(ApiAuthenticateDto request)
    {
        using var httpClient = new HttpClient();
        var requestData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["username"] = request.Username,
            ["password"] = request.Password
        };

        var requestContent = new FormUrlEncodedContent(requestData);

        var response = await httpClient.PostAsync($"{authority}/protocol/openid-connect/token", requestContent);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation($"Mavadkaran SSO request failed with status code: {response.StatusCode}{Environment.NewLine}Response:{responseContent}");

            return string.Empty;
        }

        using var document = JsonDocument.Parse(responseContent);

        if (!document.RootElement.TryGetProperty("access_token", out var tokenElement))
        {
            logger.LogInformation($"Mavadkaran SSO request failed: token is empty {Environment.NewLine}status code: {response.StatusCode}{Environment.NewLine}Response:{responseContent}");
          
            return string.Empty;
        }

        var accessToken = tokenElement.GetString();

        return accessToken;
    }
}
