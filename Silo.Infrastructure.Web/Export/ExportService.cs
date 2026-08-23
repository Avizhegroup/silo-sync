using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Silo.Infrastructure.Web;
public partial class ExportService(IJSRuntime JSRuntime
    , ILogger<ExportService> Logger
    , IConfiguration Configuration) : IExport
{
    public async Task ExportAndDownload(MemoryStream stream, string fileName)
    {
        DotNetStreamReference streamRef = new(stream: stream);

        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }

    public async Task ExportAndDownload(string path, string fileName)
    {
        using FileStream fileStream = System.IO.File.OpenRead(path);

        using MemoryStream stream = new();

        byte[] bytes = new byte[fileStream.Length];

        await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length);

        stream.Write(bytes, 0, bytes.Length);

        stream.Seek(0, SeekOrigin.Begin);

        DotNetStreamReference streamRef = new(stream: stream);

        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
    }

    public async Task ExportAndPrint(MemoryStream stream, string mimeType)
    {
        DotNetStreamReference streamRef = new(stream: stream);

        await JSRuntime.InvokeVoidAsync("printFileFromStream", streamRef, mimeType);
    }

    public async Task ExportAndDownloadUsingBypass(int reportId)
    {
        var url = Configuration["BypassIp"];

        await JSRuntime.InvokeVoidAsync("window.open"
            , $"{url}/bypass/print/{reportId}"
            , "_blank");
    }
}
