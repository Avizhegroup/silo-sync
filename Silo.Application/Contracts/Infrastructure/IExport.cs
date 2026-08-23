namespace Silo.Application;
public interface IExport
{
    Task ExportAndDownload(MemoryStream stream, string fileName);
    Task ExportAndDownload(string path, string fileName);
    Task ExportAndPrint(MemoryStream stream, string mimeType);
    Task ExportAndDownloadUsingBypass(int reportId);
}
