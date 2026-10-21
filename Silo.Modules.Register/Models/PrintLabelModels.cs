using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Silo.Modules.Register.Models;

/// <summary>A label template reported by the device host (kind "zpl", "zmin" or "frx").</summary>
public sealed record DeviceFormat(string Kind, string Name)
{
    public string Key => $"{Kind}|{Name}";
    public string Display => $"{Kind.ToUpperInvariant()} - {Name}";

    public bool IsFastReport => Kind == "frx";

    /// <summary>
    /// Legacy FastReport naming <c>title_printer_brand</c>: the brand code is the third token. Null for other names.
    /// Used by "only formats of the product's brand".
    /// </summary>
    public string? Brand
    {
        get
        {
            if (!IsFastReport)
            {
                return null;
            }

            var parts = Name.Split('_');
            return parts.Length > 2 ? parts[2].Trim() : null;
        }
    }
}

/// <summary>One row of <c>SGetPrintListByPrintActionId</c> (all values arrive as strings except the flag).</summary>
public sealed class PrintLabelRow
{
    public string? Id { get; set; }
    public string? ProductSerial { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductTypeTitle { get; set; }
    public string? ProductCount { get; set; }
    public string? ProductCountInPack { get; set; }
    public string? ProductItemCount { get; set; }
    public string? ProductUnit { get; set; }
    public string? ProductBrand { get; set; }
    public string? ProductBrandTitle { get; set; }
    public string? ProductGroupTitle { get; set; }
    [JsonPropertyName("fld_ProductPropertyBTitle")] public string? ProductionShiftTitle { get; set; }
    public string? ProductProperties { get; set; }
    public string? ProductSizeTitle { get; set; }
    public string? ProductRegCode { get; set; }
    public string? ProductStatusTitle { get; set; }
    public string? ProductTechnicalData { get; set; }
    public string? ProductOldSerial { get; set; }
    public string? DocumentId { get; set; }
    public string? TagEpc { get; set; }
    public string? PrintActionDateTime { get; set; }
    public bool RegisterFlag { get; set; }

    [JsonIgnore] public bool IsSelected { get; set; }
    [JsonIgnore] public string Status { get; set; } = string.Empty;
    [JsonIgnore] public bool? Succeeded { get; set; }
}

/// <summary>
/// Builds the value map for a label template. Token names are the legacy ones (Persian, without brackets) so existing
/// templates keep working; the device host replaces <c>[token]</c> in the template.
/// </summary>
public static class LabelFieldBuilder
{
    public static bool TryBuild(
        PrintLabelRow row,
        string epc,
        out Dictionary<string, string> fields,
        out Dictionary<string, string> technicalData,
        out string error)
    {
        fields = [];
        technicalData = [];

        if (!TryPersianDate(row.PrintActionDateTime, out var date, out var day, out var month, out var shortYear))
        {
            error = RegisterTexts.PrintBadDate(row.PrintActionDateTime?.ToString() ?? string.Empty);
            return false;
        }

        fields["سریال_قدیمی"] = row.ProductOldSerial ?? string.Empty;
        fields["سریال_محموله"] = row.ProductSerial ?? string.Empty;
        fields["عنوان_کالا"] = row.ProductName ?? string.Empty;
        fields["عنوان_لاتین"] = row.ProductDescription ?? string.Empty;
        fields["کد_کالا"] = row.ProductCode ?? string.Empty;
        fields["تاریخ_تولید"] = date;
        fields["روز_تولید"] = day;
        fields["ماه_تولید"] = month;
        fields["سال_تولید"] = shortYear;
        fields["کد_فنی"] = row.ProductRegCode ?? string.Empty;
        // The legacy ZPL flow filled this token with the technical code by mistake; the ZMIN flow used the count.
        fields["مقدار_محموله"] = row.ProductCount ?? string.Empty;
        fields["کد_سند"] = row.DocumentId ?? string.Empty;
        fields["شناسه_تگ"] = epc;
        fields["وضعیت_کالا"] = row.ProductStatusTitle ?? string.Empty;
        fields["نوع_کالا"] = row.ProductTypeTitle ?? string.Empty;
        fields["سایز_کالا"] = row.ProductSizeTitle ?? string.Empty;
        fields["تعداد_کالا_در_محموله"] = row.ProductCountInPack ?? string.Empty;

        // Values the FastReport templates also use (legacy parameter names, "_" stands for a space there).
        fields["تاریخ_تولید_شمسی"] = date;
        fields["تاریخ_تولید_میلادی"] = row.PrintActionDateTime ?? string.Empty;
        fields["مقدار_هر_کالا"] = row.ProductItemCount ?? string.Empty;
        fields["واحد_کالا"] = row.ProductUnit ?? string.Empty;
        fields["شیفت_تولید"] = row.ProductionShiftTitle ?? string.Empty;
        fields["برند_کالا"] = row.ProductBrandTitle ?? string.Empty;
        fields["شانه_کالا"] = row.ProductGroupTitle ?? string.Empty;
        fields["عنوان_کالا_لاتین"] = row.ProductDescription ?? string.Empty;
        fields["RFID"] = epc;

        technicalData = ReadTechnicalData(row.ProductTechnicalData);

        // Legacy FastReport order: the technical data first, then the product properties override clashing names.
        foreach (var pair in ReadTechnicalData(row.ProductProperties))
        {
            technicalData[pair.Key] = pair.Value;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads the product technical data (a JSON object) into token → text; anything else yields no values.</summary>
    public static Dictionary<string, string> ReadTechnicalData(string? json)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(json))
        {
            return result;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return result;
            }

            foreach (var property in document.RootElement.EnumerateObject())
            {
                result[property.Name] = property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString() ?? string.Empty
                    : property.Value.ToString();
            }
        }
        catch (JsonException)
        {
        }

        return result;
    }

    /// <summary>Converts the print date to the Persian calendar: yyyy/MM/dd, dd, MM and the two-digit year.</summary>
    public static bool TryPersianDate(string? text, out string date, out string day, out string month, out string shortYear)
    {
        date = day = month = shortYear = string.Empty;
        if (!DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)
            && !DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out value))
        {
            return false;
        }

        var calendar = new PersianCalendar();
        var year = calendar.GetYear(value);
        var monthNumber = calendar.GetMonth(value);
        var dayNumber = calendar.GetDayOfMonth(value);

        date = $"{year:0000}/{monthNumber:00}/{dayNumber:00}";
        day = $"{dayNumber:00}";
        month = $"{monthNumber:00}";
        shortYear = $"{year % 100:00}";
        return true;
    }
}
