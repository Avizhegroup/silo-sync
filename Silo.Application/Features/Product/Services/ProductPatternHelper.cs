using System.Text;
using System.Text.Json;

namespace Silo.Application.Features;

/// <summary>
/// Pure helpers for the "product pattern" feature: build the AI prompt, parse the AI answer into a
/// normalized pattern JSON (stored in tbl_Products.fld_ProductPattern) and score the similarity of two patterns.
/// Has no infrastructure dependencies so it can be unit tested in isolation.
/// </summary>
public static class ProductPatternHelper
{
    public const int MaxImages = 8;
    public const long MaxImageBytes = 10L * 1024 * 1024;
    public const double KeywordWeight = 3d;
    public const double SummaryWeight = 1d;

    public static readonly string[] AllowedMediaTypes = ["image/png", "image/jpeg", "image/webp"];

    public const string Prompt =
        "Analyze the product shown in the image(s) and describe its visual pattern. " +
        "Answer ONLY with a single JSON object, no markdown, with the keys: " +
        "\"summary\" (one short sentence), \"category\" (product type), \"colors\" (array), " +
        "\"materials\" (array), \"shapes\" (array), \"keywords\" (array of 5-15 short descriptive words, " +
        "Persian and English).";

    /// <summary>The built-in prompt is only needed when no instruction key is configured (the key carries the instructions).</summary>
    public static string PromptFor(string instructionKey)
        => string.IsNullOrWhiteSpace(instructionKey) ? Prompt : null;

    public static bool IsAllowedMediaType(string mediaType)
        => !string.IsNullOrWhiteSpace(mediaType)
           && AllowedMediaTypes.Contains(mediaType.Trim().ToLowerInvariant());

    /// <summary>Extracts the first JSON object from AI text and normalizes it. Falls back to plain text as summary.</summary>
    public static ProductPattern Parse(string aiText)
    {
        var pattern = new ProductPattern();

        if (string.IsNullOrWhiteSpace(aiText))
        {
            return pattern;
        }

        var json = ExtractJsonObject(aiText);

        if (json is not null)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    pattern.Summary = ReadString(root, "summary");
                    pattern.Category = ReadString(root, "category");
                    pattern.Colors = ReadList(root, "colors");
                    pattern.Materials = ReadList(root, "materials");
                    pattern.Shapes = ReadList(root, "shapes");
                    pattern.Keywords = ReadList(root, "keywords");

                    if (pattern.IsEmpty)
                    {
                        // Unknown JSON shape (the instructions stored under the AI key may differ): keep every text value.
                        var texts = new List<string>();
                        CollectStrings(root, texts);
                        pattern.Summary = string.Join(" ", texts);
                    }

                    return pattern;
                }
            }
            catch (JsonException)
            {
                // fall through to plain-text handling
            }
        }

        pattern.Summary = aiText.Trim();

        return pattern;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Serialize(ProductPattern pattern)
        => JsonSerializer.Serialize(pattern, SerializerOptions);

    /// <summary>Reads a stored pattern; non-JSON legacy text is treated as summary.</summary>
    public static ProductPattern FromStored(string stored)
        => string.IsNullOrWhiteSpace(stored) ? new ProductPattern() : Parse(stored);

    /// <summary>Similarity percentage 0..100 between two patterns (weighted cosine over tokens).</summary>
    public static double Similarity(ProductPattern a, ProductPattern b)
    {
        var va = BuildVector(a);
        var vb = BuildVector(b);

        if (va.Count == 0 || vb.Count == 0)
        {
            return 0;
        }

        double dot = 0, na = 0, nb = 0;

        foreach (var (token, w) in va)
        {
            na += w * w;

            if (vb.TryGetValue(token, out var w2))
            {
                dot += w * w2;
            }
        }

        foreach (var w in vb.Values)
        {
            nb += w * w;
        }

        var score = dot / (Math.Sqrt(na) * Math.Sqrt(nb));

        return Math.Round(Math.Min(1d, score) * 100d, 1);
    }

    public static Dictionary<string, double> BuildVector(ProductPattern pattern)
    {
        var vector = new Dictionary<string, double>();

        if (pattern is null)
        {
            return vector;
        }

        void Add(string text, double weight)
        {
            foreach (var token in Tokenize(text))
            {
                vector[token] = vector.TryGetValue(token, out var cur) ? Math.Max(cur, weight) : weight;
            }
        }

        foreach (var k in pattern.Keywords ?? []) Add(k, KeywordWeight);
        foreach (var k in pattern.Colors ?? []) Add(k, KeywordWeight);
        foreach (var k in pattern.Materials ?? []) Add(k, KeywordWeight);
        foreach (var k in pattern.Shapes ?? []) Add(k, KeywordWeight);
        Add(pattern.Category, KeywordWeight);
        Add(pattern.Summary, SummaryWeight);

        return vector;
    }

    public static IEnumerable<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var sb = new StringBuilder();

        foreach (var raw in text)
        {
            var c = NormalizeChar(raw);

            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
            }
            else if (sb.Length > 0)
            {
                var token = sb.ToString();
                sb.Clear();

                if (token.Length > 1)
                {
                    yield return token;
                }
            }
        }

        if (sb.Length > 1)
        {
            yield return sb.ToString();
        }
    }

    private static char NormalizeChar(char c) => c switch
    {
        'ي' => 'ی',
        'ك' => 'ک',
        '‌' => ' ', // ZWNJ
        _ => c
    };

    private static string ExtractJsonObject(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        return start >= 0 && end > start ? text[start..(end + 1)] : null;
    }

    private static void CollectStrings(JsonElement element, List<string> texts)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String when !string.IsNullOrWhiteSpace(element.GetString()):
                texts.Add(element.GetString()!.Trim());
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray()) CollectStrings(item, texts);
                break;
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject()) CollectStrings(property.Value, texts);
                break;
        }
    }

    private static string ReadString(JsonElement root, string name)
        => root.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String
            ? el.GetString()?.Trim() ?? string.Empty
            : string.Empty;

    private static List<string> ReadList(JsonElement root, string name)
    {
        var list = new List<string>();

        if (!root.TryGetProperty(name, out var el))
        {
            return list;
        }

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in el.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(item.GetString()))
                {
                    list.Add(item.GetString()!.Trim());
                }
            }
        }
        else if (el.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(el.GetString()))
        {
            list.AddRange(el.GetString()!.Split([',', '،'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        return list;
    }
}

public class ProductPattern
{
    public string Summary { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = [];
    public List<string> Materials { get; set; } = [];
    public List<string> Shapes { get; set; } = [];
    public List<string> Keywords { get; set; } = [];

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsEmpty => string.IsNullOrWhiteSpace(Summary) && string.IsNullOrWhiteSpace(Category)
        && Colors.Count == 0 && Materials.Count == 0 && Shapes.Count == 0 && Keywords.Count == 0;
}

public record ProductPatternCandidate(int Id, string Code, string Title, int? GalleryId, string StoredPattern);

public static class ProductPatternMatcher
{
    /// <summary>Scores candidates against the query pattern, filters by min score and returns the best matches first.</summary>
    public static List<ProductImageMatchVm> Rank(ProductPattern query
        , IEnumerable<ProductPatternCandidate> candidates
        , double minScore = 0
        , int maxResults = 20)
    {
        if (query is null || query.IsEmpty)
        {
            return [];
        }

        return candidates
            .Select(c =>
            {
                var pattern = ProductPatternHelper.FromStored(c.StoredPattern);

                return new ProductImageMatchVm
                {
                    ProductId = c.Id,
                    ProductCode = c.Code,
                    ProductTitle = c.Title,
                    ProductGalleryId = c.GalleryId ?? 0,
                    PatternSummary = pattern.Summary,
                    Score = ProductPatternHelper.Similarity(query, pattern)
                };
            })
            .Where(m => m.Score > 0 && m.Score >= minScore)
            .OrderByDescending(m => m.Score)
            .ThenBy(m => m.ProductCode, StringComparer.Ordinal)
            .Take(Math.Max(1, maxResults))
            .ToList();
    }
}
