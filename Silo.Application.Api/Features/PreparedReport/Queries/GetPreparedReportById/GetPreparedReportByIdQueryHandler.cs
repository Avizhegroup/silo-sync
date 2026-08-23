using System.Dynamic;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Silo.Application.Api.Features;
public class GetPreparedReportByIdQueryHandler(
    WmsApiContext context,
    IMapper mapper) : IRequestHandler<GetPreparedReportByIdQuery, GetPreparedReportByIdVm>
{
    public async Task<GetPreparedReportByIdVm> Handle(GetPreparedReportByIdQuery request, CancellationToken cancellationToken)
    {
        var preparedReport = await context.PreparedReports
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (preparedReport is null)
        {
            return null;
        }

        JsonSerializerOptions? options = new()
        {
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        var result = new GetPreparedReportByIdVm()
        {
            Id = preparedReport.Id,
            Title = preparedReport.Title,
            ReportFileName = preparedReport.ReportFileName,
            Variables = DeserializeKvpList(preparedReport.Variables),
            DataSources = DeserializeKvpList(preparedReport.DataSources),
            Images = preparedReport.Images.HasNoValue() ? new() : JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(preparedReport.Images, options),
            UserId = preparedReport.UserId,
            UserName = preparedReport.User?.Name
        };

        return result;
    }

    #region Private
    private List<KeyValuePair<string, object>> DeserializeKvpList(string json)
    {
        if (json.HasNoValue())
        {
            return new();
        }

        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        List<KeyValuePair<string, object>>? result = new();

        if (root.ValueKind != JsonValueKind.Array)
        {
            return result;
        }

        foreach (var item in root.EnumerateArray())
        {
            if (!item.TryGetProperty("Key", out var keyProp))
            {
                continue;
            }

            if (!item.TryGetProperty("Value", out var valueProp))
            {
                continue;
            }

            var key = keyProp.GetString();

            var value = ConvertJsonElement(valueProp);

            result.Add(new KeyValuePair<string, object>(key, value));
        }

        return result;
    }

    private object ConvertJsonElement(JsonElement element)
    => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Array => ConvertJsonArray(element),
        JsonValueKind.Object => ConvertJsonObject(element),
        _ => element.GetRawText()
    };

    private List<object> ConvertJsonArray(JsonElement arrayElement)
    {
        List<object> rtn = new();

        foreach (var item in arrayElement.EnumerateArray())
        {
            rtn.Add(ConvertJsonElement(item));
        }

        return rtn;
    }

    private object ConvertJsonObject(JsonElement objectElement)
    {
        dynamic objExpando = new ExpandoObject();

        var obj = objExpando as IDictionary<string, object>;

        foreach (var property in objectElement.EnumerateObject())
        {
            string key = property.Name;
            var value = ConvertJsonElement(property.Value);

            if (value is null)
            {
                obj[key] = null;
            }
            else
            {
                obj[key] = value;
            }
        }

        return obj;
    }
    #endregion
}
