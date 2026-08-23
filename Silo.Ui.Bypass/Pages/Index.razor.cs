using Microsoft.JSInterop;
using Silo.Ui.Bypass.Services.Http;
using System.Dynamic;
using System.Text.Json;

namespace Silo.Ui.Bypass.Pages;
public partial class Index
{
    public bool IsLoading = true;
    public PdfExporter PdfExport = new();

    [Parameter] public int? PrintId { get; set; }

    [Inject] public ApiHandler Api { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (PrintId is null)
        {
            return;
        }

        var response = (await Api.SendAsyncObjectByUri<GetPreparedReportByIdVm>(HttpMethod.Get
                 , "PreparedReport/GetById"
                 , new GetPreparedReportByIdQuery()
                 {
                     Id = PrintId.Value
                 })).Value;

        var convertedDataSources = ConvertKeyValuePairsToDataTable(response.DataSources);
       
        var convertedVariables = ConvertKeyValuePairs(response.Variables);

        var stream = await PdfExport.PrintReport(response.ReportFileName
            , convertedDataSources
            , response.Images
            , convertedVariables);

        DotNetStreamReference streamRef = new(stream);

        await JSRuntime.InvokeVoidAsync("downloadFileFromStream", $"{response.Title}.pdf", streamRef);

        IsLoading = false;
    }

    #region Private Methods
    private List<KeyValuePair<string, object>> ConvertKeyValuePairsToDataTable(List<KeyValuePair<string, object>> source)
    {
        var result = new List<KeyValuePair<string, object>>();
        
        foreach (var kvp in source)
        {
            var convertedValue = ConvertJsonElement(kvp.Value);
            
            if (convertedValue is List<object> list && list.Count > 0)
            {
                var dataTable = ConvertListToDataTable(list, kvp.Key);

                result.Add(new KeyValuePair<string, object>(kvp.Key, dataTable));
            }
            else
            {
                result.Add(new KeyValuePair<string, object>(kvp.Key, convertedValue));
            }
        }
        
        return result;
    }

    private DataTable ConvertListToDataTable(List<object> list, string tableName)
    {
        var dataTable = new DataTable(tableName);
        
        if (list.Count == 0) return dataTable;
        
        if (list[0] is IDictionary<string, object> firstItem)
        {
            foreach (var key in firstItem.Keys)
            {
                dataTable.Columns.Add(key, typeof(object));
            }
            
            foreach (var item in list)
            {
                if (item is IDictionary<string, object> dictItem)
                {
                    var row = dataTable.NewRow();
                    foreach (var column in dataTable.Columns.Cast<DataColumn>())
                    {
                        if (dictItem.TryGetValue(column.ColumnName, out var value))
                        {
                            row[column.ColumnName] = value ?? DBNull.Value;
                        }
                        else
                        {
                            row[column.ColumnName] = DBNull.Value;
                        }
                    }
                    dataTable.Rows.Add(row);
                }
            }
        }
        
        return dataTable;
    }

    private List<KeyValuePair<string, object>> ConvertKeyValuePairs(List<KeyValuePair<string, object>> source)
    {
        var result = new List<KeyValuePair<string, object>>();
        
        foreach (var kvp in source)
        {
            var convertedValue = ConvertJsonElement(kvp.Value);

            result.Add(new KeyValuePair<string, object>(kvp.Key, convertedValue));
        }
        
        return result;
    }

    private object ConvertJsonElement(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.String => jsonElement.GetString(),
                JsonValueKind.Number => jsonElement.TryGetInt64(out var l) ? l : jsonElement.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Array => ConvertJsonArray(jsonElement),
                JsonValueKind.Object => ConvertJsonObject(jsonElement),
                _ => jsonElement.GetRawText()
            };
        }

        return value;
    }

    private List<object> ConvertJsonArray(JsonElement arrayElement)
    {
        List<object> rtn = new();
        foreach (var item in arrayElement.EnumerateArray())
        {
            rtn.Add(ConvertJsonElement(item));
        }
        return rtn;
    }

    private object ConvertJsonObject(JsonElement objectElement)
    {
        dynamic objExpando = new ExpandoObject();
        
        var obj = objExpando as IDictionary<string, object>;

        foreach (var property in objectElement.EnumerateObject())
        {
            string key = property.Name;
            var value = ConvertJsonElement(property.Value);

            if (value is null)
            {
                obj[key] = null;
            }
            else
            {
                obj[key] = value;
            }
        }

        return obj;
    }
    #endregion
}
