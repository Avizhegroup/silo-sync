using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Silo.Application.Exceptions;

namespace Silo.Application.Api.Features;

/// <summary>
/// Shared helpers used by the TeamYar handlers.
/// </summary>
internal static class TeamYarHelper
{
    /// <summary>
    /// Key (shown on the label) that keeps the unit of the sub count, because the print/tag tables have no column for it.
    /// </summary>
    public const string SubCountUnitPropertyKey = "واحد مقدار فرعی";

    public static SiloValidationException Validation(IEnumerable<string> messages)
        => new(messages.Select(message => new ValidationResult(message)).ToList());

    public static SiloValidationException Validation(string message)
        => Validation(new[] { message });

    public static string Clean(string? value) => value?.Trim() ?? string.Empty;

    public static List<string> NormalizeSerials(IEnumerable<string?>? serials)
        => serials is null
            ? new List<string>()
            : serials.Where(s => !string.IsNullOrWhiteSpace(s))
                     .Select(s => s!.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .ToList();

    public static bool TryParsePersianDate(string? value, out DateTime date)
    {
        date = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            date = PersianCalendarTools.PersianToGregorian(value.Trim());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string? CurrentUserId(IHttpContextAccessor accessor)
    {
        try
        {
            return accessor.HttpContext?.User is { } user ? user.GetUserId() : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Sets the given keys inside a flat JSON object (the format of ProductProperties) and keeps the other keys.
    /// </summary>
    public static string? MergeProperties(string? existingJson, IEnumerable<KeyValuePair<string, string>> values)
    {
        var list = values.ToList();

        if (list.Count == 0)
        {
            return existingJson;
        }

        JObject obj;

        try
        {
            obj = string.IsNullOrWhiteSpace(existingJson) ? new JObject() : JObject.Parse(existingJson);
        }
        catch (Newtonsoft.Json.JsonException)
        {
            obj = new JObject();
        }

        foreach (var item in list)
        {
            obj[item.Key] = item.Value;
        }

        return obj.ToString(Newtonsoft.Json.Formatting.None);
    }

    public static async Task<HashSet<string>> GetTagSerialsAsync(WmsApiContext context
        , IReadOnlyCollection<string> serials
        , CancellationToken cancellationToken)
    {
        var found = await context.Tags.AsNoTracking()
                                 .Where(t => serials.Contains(t.ProductSerial))
                                 .Select(t => t.ProductSerial)
                                 .ToListAsync(cancellationToken);

        return found.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static async Task<HashSet<string>> GetPrintSerialsAsync(WmsApiContext context
        , IReadOnlyCollection<string> serials
        , CancellationToken cancellationToken)
    {
        var found = await context.Prints.AsNoTracking()
                                 .Where(p => serials.Contains(p.ProductSerial!))
                                 .Select(p => p.ProductSerial!)
                                 .Distinct()
                                 .ToListAsync(cancellationToken);

        return found.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static string Quote(IEnumerable<string> values) => string.Join(", ", values.Select(v => $"'{v}'"));
}
