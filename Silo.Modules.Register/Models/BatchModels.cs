using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace Silo.Modules.Register.Models;

/// <summary>A production document ("roll code") as returned by SGetDocumentData / SGetDocumentsByDocType.</summary>
public sealed class DocumentInfo
{
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? HeaderData { get; set; }
    public List<DocumentItemInfo>? DocumentItems { get; set; }
}

public sealed class DocumentItemInfo
{
    public string? ProductCode { get; set; }
    public string? ProductTitle { get; set; }
    public decimal Count { get; set; }

    /// <summary>Remaining quantity (requested minus already registered); filled by the page.</summary>
    public decimal? Remain { get; set; }
}

/// <summary>A row of SGetRemainRegisterByDoc: the quantity of a product still free in the document.</summary>
public sealed class RemainRow
{
    public string? ProductCode { get; set; }
    public decimal Count { get; set; }
}

public static partial class DocumentKey
{
    // The server builds the remaining-quantity query by string concatenation, so only plain keys are sent.
    [GeneratedRegex(@"^[\p{L}\p{N}_\-./ ]{1,50}$")]
    private static partial Regex Pattern();

    public static bool IsValid(string? key) => !string.IsNullOrEmpty(key) && Pattern().IsMatch(key);

    /// <summary>Legacy: an empty key, "0" or "00" means no document; "000" is the page default.</summary>
    public static bool IsNone(string? key) => string.IsNullOrWhiteSpace(key) || key is "0" or "00" or "000";
}

/// <summary>Builds serial lists by range (legacy "serial range" tab) or from an Excel file (legacy "Excel" tab).</summary>
public static class SerialListBuilder
{
    public const int MaxSerials = 50_000;
    public const long MaxExcelBytes = 5 * 1024 * 1024;

    /// <summary>
    /// Legacy rule: serials run from <paramref name="from"/> up to but excluding <paramref name="to"/>.
    /// When <paramref name="cutLength"/> is set, that slice of the start value is a fixed prefix and is removed
    /// from both numbers before counting; the counter is left-padded to the length of the start value minus the prefix.
    /// </summary>
    public static bool TryRange(string? from, string? to, int cutStart, int cutLength, out List<string> serials, out string error)
    {
        serials = [];
        error = string.Empty;
        from = (from ?? string.Empty).Trim();
        to = (to ?? string.Empty).Trim();

        if (cutStart < 0 || cutLength < 0)
        {
            error = RegisterTexts.SerialPrefixNegative;
            return false;
        }

        var prefix = string.Empty;
        if (cutStart != 0 || cutLength != 0)
        {
            if (cutStart + cutLength > from.Length || cutStart + cutLength > to.Length)
            {
                error = RegisterTexts.SerialPrefixTooLong;
                return false;
            }

            prefix = from.Substring(cutStart, cutLength);
            from = from.Remove(cutStart, cutLength);
            to = to.Remove(cutStart, cutLength);
        }

        if (!long.TryParse(from, out var start) || !long.TryParse(to, out var end) || start < 0 || end < 0)
        {
            error = RegisterTexts.SerialRangeNotNumbers;
            return false;
        }

        if (end <= start)
        {
            error = RegisterTexts.SerialRangeOrder;
            return false;
        }

        if (end - start > MaxSerials)
        {
            error = RegisterTexts.SerialRangeTooMany(MaxSerials);
            return false;
        }

        var width = from.Length;
        for (var i = start; i < end; i++)
        {
            serials.Add(prefix + i.ToString().PadLeft(width, '0'));
        }

        return Validate(serials, out error);
    }

    /// <summary>The first column of the first worksheet, one serial per non-empty cell.</summary>
    public static bool TryExcel(Stream stream, bool firstRowIsHeader, out List<string> serials, out string error)
    {
        serials = [];
        error = string.Empty;

        try
        {
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheets.FirstOrDefault();
            if (sheet is null)
            {
                error = RegisterTexts.SerialFileNoSheet;
                return false;
            }

            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
            for (var row = firstRowIsHeader ? 2 : 1; row <= lastRow; row++)
            {
                var text = sheet.Cell(row, 1).GetFormattedString().Trim();
                if (text.Length > 0)
                {
                    serials.Add(text);
                }

                if (serials.Count > MaxSerials)
                {
                    error = RegisterTexts.SerialFileTooMany(MaxSerials);
                    serials = [];
                    return false;
                }
            }
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or ArgumentException or NotSupportedException)
        {
            error = TextResources.APP_StringKeys_Validation_Excel_Format;
            serials = [];
            return false;
        }

        if (serials.Count == 0)
        {
            error = TextResources.APP_StringKeys_Error_SerialsNotFound;
            return false;
        }

        return Validate(serials, out error);
    }

    private static bool Validate(List<string> serials, out string error)
    {
        error = string.Empty;

        var invalid = serials.Where(p => !BarcodeParser.IsValidSerial(p)).Take(3).ToList();
        if (invalid.Count > 0)
        {
            error = RegisterTexts.SerialInvalid(string.Join(", ", invalid));
            return false;
        }

        var duplicate = serials.GroupBy(p => p, StringComparer.OrdinalIgnoreCase).FirstOrDefault(p => p.Count() > 1);
        if (duplicate is not null)
        {
            error = RegisterTexts.SerialDuplicate(duplicate.Key);
            return false;
        }

        return true;
    }
}
