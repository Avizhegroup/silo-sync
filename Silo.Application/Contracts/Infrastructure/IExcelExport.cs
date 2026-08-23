namespace Silo.Application;

public interface IExcelExport
{
    MemoryStream ExportDatatable(DataTable table);
    Task ExportJsonData(string fileName, List<object> data, List<string> columns);
}
