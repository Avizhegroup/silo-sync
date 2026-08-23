using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Ui.Bypass.Services.Http;
public class ApiHandler(IConfiguration Configuration) : HttpClientHandler
{
    public async Task<ApiResponse<T>> SendAsyncObjectByUri<T>(HttpMethod method
, string uri
, object data = null
, JsonSerializerContext context = null)
    {
        var baseUri = Configuration["Api:Ip"];

        JsonSerializerOptions option = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        if (context is not null)
        {
            option.TypeInfoResolver = context;
        }

        var passDataJsonString = JsonSerializer.Serialize(data);

        var buffer = Encoding.UTF8.GetBytes(passDataJsonString);

        var byteContent = new ByteArrayContent(buffer);

        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpRequestMessage request = new(method, baseUri + uri);

        request.Content = byteContent;

        var resultStream = await SendAsync(request, new CancellationToken());

        Stream httpStream = await resultStream.Content.ReadAsStreamAsync();
        using StreamReader sr = new(httpStream);

        var result = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(httpStream, option);
        
        return result;
    }
}
