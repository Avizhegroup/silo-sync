using Silo.Windows.Register.Devices;

namespace Silo.Windows.Register.Printing;

/// <summary>ZPL label printing: fills a template and sends it as a RAW job to the Windows printer.</summary>
public static class ZplLabelPrinter
{
    /// <summary>
    /// Builds the final ZPL. As in the legacy application the line breaks are removed (graphic data
    /// rows are concatenated), which is valid because every ZPL command starts with ^ or ~.
    /// </summary>
    public static string Render(
        string template,
        IReadOnlyDictionary<string, string>? fields,
        IReadOnlyDictionary<string, string>? technicalData)
    {
        var zpl = LabelTokens.Apply(template, fields, technicalData, sanitizeForZpl: true);
        return zpl.Replace("\r", string.Empty).Replace("\n", string.Empty);
    }

    public static void Print(
        string printer,
        string templatePath,
        IReadOnlyDictionary<string, string>? fields,
        IReadOnlyDictionary<string, string>? technicalData)
    {
        var zpl = Render(File.ReadAllText(templatePath), fields, technicalData);
        PrinterService.SendRaw(printer, zpl);
    }
}
