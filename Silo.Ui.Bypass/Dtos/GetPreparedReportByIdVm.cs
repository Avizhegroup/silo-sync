namespace Silo.Ui.Bypass;
public class GetPreparedReportByIdVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<KeyValuePair<string, dynamic>> Variables { get; set; } 
    public List<KeyValuePair<string, dynamic>> DataSources { get; set; }
    public List<KeyValuePair<string, string>> Images { get; set; } 
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string ReportFileName { get; set; }
}


