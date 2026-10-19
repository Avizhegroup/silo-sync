using System.Diagnostics;
using FastReport;
using FastReport.Export.PdfSimple;

namespace Silo.Windows.Register.Printing;

/// <summary>One label of a FastReport job: token name → value (see the web LabelFieldBuilder) and the product technical data.</summary>
public sealed record FrxItem(Dictionary<string, string>? Fields, Dictionary<string, string>? TechnicalData);

/// <summary>
/// FastReport (.frx) labels, the legacy "print format" files. Each label is one report run; several labels of one
/// job are appended to the same prepared report so they leave as one print job or as one merged PDF (legacy
/// "MergPrintToPDF"). A PDF is written to the temp folder and opened with the default viewer on this workstation
/// (legacy MergPdf.pdf); it is also what replaces the legacy preview window.
/// File names follow the legacy convention <c>title_printer_brand.frx</c>: the second token is the printer
/// ("0" = the configured default printer), the third the brand code used by "only formats of the product's brand".
/// </summary>
public sealed class FastReportPrinter
{
    private const int MaximumJobs = 4;
    private static readonly TimeSpan JobLifetime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan PdfLifetime = TimeSpan.FromDays(1);

    private readonly object _gate = new();
    private readonly Dictionary<string, PendingJob> _jobs = new();

    private sealed class PendingJob(Report report)
    {
        public Report Report { get; } = report;
        public DateTime Created { get; } = DateTime.UtcNow;
    }

    /// <summary>The printer encoded in the file name (second token); null when absent or "0".</summary>
    public static string? PrinterOfName(string formatName)
    {
        var parts = formatName.Split('_');
        return parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) && parts[1].Trim() != "0" ? parts[1].Trim() : null;
    }

    /// <summary>
    /// Prints or exports <paramref name="items"/>. With <paramref name="job"/> set and <paramref name="more"/> true the
    /// labels are only collected; the final call (more = false) prints or exports everything collected so far.
    /// </summary>
    /// <param name="printer">Installed printer name; required when <paramref name="output"/> is "print".</param>
    /// <param name="output">"print" or "pdf".</param>
    public ZminResult Run(string templatePath, string? printer, IReadOnlyList<FrxItem> items, string output, string? job, bool more)
    {
        lock (_gate)
        {
            DropStaleJobs();

            Report? report = null;
            var appending = false;
            if (!string.IsNullOrEmpty(job) && _jobs.TryGetValue(job, out var pending))
            {
                report = pending.Report;
                appending = true;
            }

            try
            {
                if (report is null)
                {
                    report = new Report();
                    report.Load(templatePath);
                }

                foreach (var item in items)
                {
                    Fill(report, item);
                    report.Prepare(appending);
                    appending = true;
                }

                if (more && !string.IsNullOrEmpty(job))
                {
                    _jobs[job] = new PendingJob(report);
                    report = null; // kept for the next call
                    return new ZminResult(true, "Queued.");
                }

                if (!string.IsNullOrEmpty(job))
                {
                    _jobs.Remove(job);
                }

                return output == "pdf" ? ExportPdf(report) : PrintPrepared(report, printer);
            }
            catch
            {
                if (!string.IsNullOrEmpty(job))
                {
                    _jobs.Remove(job);
                }

                throw;
            }
            finally
            {
                report?.Dispose();
            }
        }
    }

    private static ZminResult PrintPrepared(Report report, string? printer)
    {
        if (string.IsNullOrWhiteSpace(printer))
        {
            return new ZminResult(false, "No printer was given for the FastReport label.");
        }

        //report.PrintSettings.Printer = printer;
        //report.PrintSettings.ShowDialog = false;
        //report.PrintPrepared();
        return new ZminResult(true, "Sent.");
    }

    private static ZminResult ExportPdf(Report report)
    {
        var folder = Path.Combine(Path.GetTempPath(), "Silo.Windows.Register");
        Directory.CreateDirectory(folder);
        DeleteOldPdfs(folder);

        var path = Path.Combine(folder, $"labels-{DateTime.Now:yyyyMMdd-HHmmss-fff}.pdf");
        report.Export(new PDFSimpleExport(), path);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        return new ZminResult(true, $"PDF opened on the workstation: {path}");
    }

    /// <summary>
    /// Technical data first, then the fields (fields win, like <see cref="LabelTokens"/>). The web sends token names with
    /// underscores (<c>سریال_محموله</c>); the legacy templates declare the same parameters with spaces
    /// (<c>سریال محموله</c>), so both spellings are set.
    /// </summary>
    private static void Fill(Report report, FrxItem item)
    {
        if (item.TechnicalData is not null)
        {
            foreach (var (name, value) in item.TechnicalData)
            {
                SetParameter(report, name, value);
            }
        }

        if (item.Fields is null)
        {
            return;
        }

        foreach (var (name, value) in item.Fields)
        {
            SetParameter(report, name, value);

            var spaced = name.Replace('_', ' ');
            if (spaced != name)
            {
                SetParameter(report, spaced, value);
            }
        }
    }

    private static void SetParameter(Report report, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        try
        {
            report.SetParameterValue(name, value ?? string.Empty);
        }
        catch (Exception)
        {
            // A value the template cannot take is skipped; the label is still produced (legacy swallowed these too).
        }
    }

    private void DropStaleJobs()
    {
        foreach (var key in _jobs.Where(p => DateTime.UtcNow - p.Value.Created > JobLifetime).Select(p => p.Key).ToList())
        {
            _jobs[key].Report.Dispose();
            _jobs.Remove(key);
        }

        while (_jobs.Count >= MaximumJobs)
        {
            var oldest = _jobs.OrderBy(p => p.Value.Created).First().Key;
            _jobs[oldest].Report.Dispose();
            _jobs.Remove(oldest);
        }
    }

    private static void DeleteOldPdfs(string folder)
    {
        try
        {
            foreach (var file in Directory.EnumerateFiles(folder, "labels-*.pdf"))
            {
                if (DateTime.UtcNow - File.GetLastWriteTimeUtc(file) > PdfLifetime)
                {
                    File.Delete(file);
                }
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
