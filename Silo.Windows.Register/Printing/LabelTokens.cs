using System.Text.RegularExpressions;

namespace Silo.Windows.Register.Printing;

/// <summary>
/// Replaces [token] placeholders in a label template. The substitution is a single pass, so a value that
/// itself contains "[...]" is never substituted again. Tokens without a value are left untouched.
/// </summary>
public static partial class LabelTokens
{
    [GeneratedRegex(@"\[([^\[\]\r\n]+)\]")]
    private static partial Regex TokenPattern();

    /// <param name="fields">Primary values (token name without brackets, e.g. "سریال_محموله").</param>
    /// <param name="technicalData">Secondary values (the product technical-data properties); fields win on a clash.</param>
    /// <param name="sanitizeForZpl">Strips ZPL control characters (^ and ~) from the values so data cannot inject commands.</param>
    public static string Apply(
        string template,
        IReadOnlyDictionary<string, string>? fields,
        IReadOnlyDictionary<string, string>? technicalData,
        bool sanitizeForZpl)
    {
        return TokenPattern().Replace(template, match =>
        {
            var name = match.Groups[1].Value;
            if (!TryGet(fields, name, out var value) && !TryGet(technicalData, name, out value))
            {
                return match.Value;
            }

            return sanitizeForZpl ? SanitizeForZpl(value) : value;
        });
    }

    public static string SanitizeForZpl(string value)
        => value.Replace('^', ' ').Replace('~', ' ').Replace('\r', ' ').Replace('\n', ' ');

    private static bool TryGet(IReadOnlyDictionary<string, string>? source, string name, out string value)
    {
        if (source is not null && source.TryGetValue(name, out var found))
        {
            value = found ?? string.Empty;
            return true;
        }

        value = string.Empty;
        return false;
    }
}
