using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Text.Json;
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Hosting;

namespace Silo.Infrastructure.Web;
public partial class ExcelExport(IExport Exporter
    , IWebHostEnvironment Environment
    , IJSRuntime JSRuntime) : IExcelExport
{
    public MemoryStream ExportDatatable(DataTable table)
    {
        MemoryStream stream = new();

        using var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook();
        workbook.WorkbookPart.Workbook.Sheets = new Sheets();
        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        sheetPart.Worksheet = new Worksheet(sheetData);
        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
        uint sheetId = 1;
        if (sheets.Elements<Sheet>().Count() > 0)
        {
            sheetId =
                sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        }
        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
        sheets.Append(sheet);
        Row headerRow = new Row();
        List<String> columns = new List<string>();
        foreach (System.Data.DataColumn column in table.Columns)
        {
            columns.Add(column.ColumnName);
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(column.ColumnName);
            headerRow.AppendChild(cell);
        }
        sheetData.AppendChild(headerRow);
        foreach (System.Data.DataRow dsrow in table.Rows)
        {
            Row newRow = new Row();
            foreach (String col in columns)
            {
                Cell cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(dsrow[col].ToString()); //
                newRow.AppendChild(cell);
            }
            sheetData.AppendChild(newRow);
        }

        return stream;
    }

    public async Task ExportJsonData(string fileName, List<object> data, List<string> dataColumns)
    {
        string path = $"{Environment.WebRootPath}\\temp\\{Guid.NewGuid().ToString().Substring(0, 6)}.xlsx";

        using var workbook = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook();
        workbook.WorkbookPart.Workbook.Sheets = new Sheets();
        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        sheetPart.Worksheet = new Worksheet(sheetData);
        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
        uint sheetId = 1;
        if (sheets.Elements<Sheet>().Count() > 0)
        {
            sheetId =
                sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
        }
        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Sheet1" };
        sheets.Append(sheet);
        Row headerRow = new Row();
        List<String> columns = new List<string>();
        foreach (var column in dataColumns)
        {
            columns.Add(column);
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(column);
            headerRow.AppendChild(cell);
        }
        sheetData.AppendChild(headerRow);
        foreach (var dsrow in data)
        {
            Row newRow = new Row();
            foreach (string col in columns)
            {
                Cell cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(((JsonElement)dsrow).GetProperty(col).ToString()); //
                newRow.AppendChild(cell);
            }
            sheetData.AppendChild(newRow);
        }

        workbook.Save();

        workbook.Dispose();

        await Exporter.ExportAndDownload(path, $"{fileName}.xlsx");
    }
}
