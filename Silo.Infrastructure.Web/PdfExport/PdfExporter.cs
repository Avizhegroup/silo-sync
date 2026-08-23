/*using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Stimulsoft.Base;
using Stimulsoft.Drawing;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;

namespace Silo.Infrastructure;

public partial class PdfExporter(IHostingEnvironment Environment
    , IJSRuntime JSRuntime
    , ILogger<PdfExporter> Logger
    , IExport Export) : IPdfExporter
{
    public async Task DownloadPdf(string reportFile
        , string title
        , List<KeyValuePair<string, object>> dataSources
        , List<KeyValuePair<string, string>> images
        , List<KeyValuePair<string, object>> variables)
    {
        StiReport report = CreateReportObject(reportFile, dataSources, images, variables);

        report.Render(false);

        MemoryStream stream = new();

        report.ExportDocument(StiExportFormat.Pdf, stream, new StiPdfExportSettings());

        stream.Seek(0, SeekOrigin.Begin);

        await Export.ExportAndDownload(stream, $"{title}.pdf");
    }

    public async Task PrintReport(string reportFile
        , List<KeyValuePair<string, object>> dataSources
        , List<KeyValuePair<string, string>> images
        , List<KeyValuePair<string, object>> variables)
    {
        StiReport report = CreateReportObject(reportFile, dataSources, images, variables);

        report.Render(false);

        MemoryStream stream = new();

        report.ExportDocument(StiExportFormat.Pdf, stream, new StiPdfExportSettings());

        stream.Seek(0, SeekOrigin.Begin);

        await Export.ExportAndPrint(stream, "application/pdf");
    }

    #region Privates
    private StiReport CreateReportObject(string reportFile
        , List<KeyValuePair<string, object>> dataSources
        , List<KeyValuePair<string, string>> images
        , List<KeyValuePair<string, object>> variables)
    {
        CheckLicense();

        StiReport report = new();
        report.Load(Path.Combine(Environment.WebRootPath, "reports", $"{reportFile}.mrt"));

        report.Dictionary.DataStore.Clear();

        foreach (KeyValuePair<string, object> dataSource in dataSources)
        {
            report.RegData(dataSource.Key, dataSource.Value);
        }

        foreach (KeyValuePair<string, object> variable in variables)
        {
            report.Dictionary.Variables.Add(variable.Key, variable.Value);
        }

        foreach (KeyValuePair<string, string> image in images)
        {
            var imageComponent = report.GetComponentByName(image.Key) as StiImage;

            using var imageStream = File.OpenRead(image.Value.ToString());

            imageComponent.Image = Image.FromStream(imageStream);
        }

        report.Dictionary.SynchronizeBusinessObjects(2);

        return report;
    }

    private void CheckLicense()
    {
        StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkgpgFGkUl79uxVs8X+uspx6K+tqdtOB5G1S6PFPRrlVNvMUiSiNYl724EZbrUAWwAYHlGLRbvxMviMExTh2l9xZJ2xc4K1z3ZVudRpQpuDdFq+fe0wKXSKlB6okl0hUd2ikQHfyzsAN8fJltqvGRa5LI8BFkA/f7tffwK6jzW5xYYhHxQpU3hy4fmKo/BSg6yKAoUq3yMZTG6tWeKnWcI6ftCDxEHd30EjMISNn1LCdLN0/4YmedTjM7x+0dMiI2Qif/yI+y8gmdbostOE8S2ZjrpKsgxVv2AAZPdzHEkzYSzx81RHDzZBhKRZc5mwWAmXsWBFRQol9PdSQ8BZYLqvJ4Jzrcrext+t1ZD7HE1RZPLPAqErO9eo+7Zn9Cvu5O73+b9dxhE2sRyAv9Tl1lV2WqMezWRsO55Q3LntawkPq0HvBkd9f8uVuq9zk7VKegetCDLb0wszBAs1mjWzN+ACVHiPVKIk94/QlCkj31dWCg8YTrT5btsKcLibxog7pv1+2e4yocZKWsposmcJbgG0";
    }
    #endregion
}
*/
