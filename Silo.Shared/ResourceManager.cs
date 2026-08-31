using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace Silo.Shared;
public static class ResourceManager
{
    private static readonly ConcurrentDictionary<string, string> _values = new(StringComparer.Ordinal);

    public static bool IsLoaded { get; private set; }

    [Obsolete(" Kept for backward compatibility; the underlying dictionary should be populated via Load().")]
    public static void Initialize(IConfiguration configuration)
    {
    }

    public static void Load(IDictionary<string, string?> values)
    {
        _values.Clear();

        foreach (var item in values)
        {
            if (item.Key is not null)
            {
                _values[item.Key] = item.Value ?? string.Empty;
            }
        }

        IsLoaded = true;
    }

    public static string? GetString(string key)
    {
        if (key is null || !_values.TryGetValue(key, out var value))
        {
            return null;
        }

        return value;
    }
}
