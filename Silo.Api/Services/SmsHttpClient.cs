using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Api.Services;

public partial class SmsHttpClient(ILogger<SmsHttpClient> Logger
    , IConfiguration Configuration) : HttpClientHandler
{
    private string baseUri;
    private const int bufferSize = 4096;

    public string Post(params KeyValuePair<string, string>[] data)
    {
        if (baseUri.HasNoValue())
        {
            baseUri = Configuration.GetSection("ProjectConfigs").GetSection("WmsConfigs").GetSection("Notification").GetSection("Sms")["Api"];
        }

        var dataDict = new Dictionary<string, string>();
        foreach (var item in data)
        {
            dataDict.Add(item.Key, item.Value);
        }

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        HttpRequestMessage request = new(HttpMethod.Post, baseUri);

        request.Content = new FormUrlEncodedContent(dataDict);

        var resultStream = Send(request, new CancellationToken());

        return (resultStream.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
    }
}
