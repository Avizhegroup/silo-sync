using Newtonsoft.Json.Linq;

namespace Silo.Tools;

public static class ExcelExportTools
{
    /// <summary>
    /// This method gets telerik grid data and add dynamic field columns with their values.
    /// The object that passes to telerik grid Data property must contain one of these JTokens:
    /// ProductProperties (from tbl_Tags), MovementActionData (from tbl_MovementAction), TechnicalInfoData (from tbl_Products).
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static DataTable GetDataTableWithDynamicColumnAndValues(GridBeforeExcelExportEventArgs args)
    {
        try
        {
            args.IsCancelled = true;

            var dataTable = new DataTable();

            foreach (var column in args.Columns.Where(p => p.Title.HasValue()))
            {
                dataTable.Columns.Add(column.Title);
            }

            foreach (var data in args.Data)
            {
                int indexer = 0;
                DataRow row = dataTable.NewRow();

                foreach (var column in args.Columns.Where(p => p.Title.HasValue()))
                {
                    Type type = data.GetType();

                    var property = type.GetProperties().FirstOrDefault(p => p.Name.Equals(column.Field));

                    var value = property.GetValue(data);

                    switch (property.Name)
                    {
                        case "ProductProperties":
                            row[indexer] = JToken.Parse((string)value)[$"{column.Title}"];
                            break;
                        case "MovementActionData":
                            row[indexer] = JToken.Parse((string)value)[$"{column.Title}"];
                            break;
                        case "TechnicalInfoData":
                            row[indexer] = JToken.Parse(((string)value).Split("-,-").First())[$"{column.Title}"];
                            break;
                        default:
                            row[indexer] = property.GetValue(data);
                            break;
                    }

                    indexer++;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
