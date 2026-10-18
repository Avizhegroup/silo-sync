using System.Text.RegularExpressions;

namespace Silo.Modules.Register.Models;

/// <summary>Reads the serial out of a scanned barcode and validates serials and EPCs (legacy F8 capture rules).</summary>
public static partial class BarcodeParser
{
    public const int EpcLength = 24;

    [GeneratedRegex("^[0-9A-Fa-f]{24}$")]
    private static partial Regex EpcPattern();

    [GeneratedRegex(@"^[A-Za-z0-9_\-./]+$")]
    private static partial Regex SerialPattern();

    /// <summary>
    /// Compound barcodes are split on a single separator character: three parts give the third, two parts give the
    /// second, anything else is used whole. An empty separator disables splitting.
    /// </summary>
    public static string ExtractSerial(string? barcode, string? separator)
    {
        var value = (barcode ?? string.Empty).Trim();
        if (value.Length == 0 || string.IsNullOrEmpty(separator))
        {
            return value;
        }

        var parts = value.Split(separator[0]);
        return parts.Length switch
        {
            3 => parts[2].Trim(),
            2 => parts[1].Trim(),
            _ => value
        };
    }

    public static bool IsValidSerial(string? serial) => !string.IsNullOrEmpty(serial) && SerialPattern().IsMatch(serial);

    public static bool IsValidEpc(string? epc) => !string.IsNullOrEmpty(epc) && EpcPattern().IsMatch(epc);

    public static string NormalizeEpc(string? epc) => (epc ?? string.Empty).Trim().ToUpperInvariant();
}
