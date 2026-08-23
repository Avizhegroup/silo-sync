namespace Silo.Application.Features;
public class GetAllDocumentLogQuery
{
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public string Description { get; set; }
    public string User { get; set; }
    public string DocumentEventType { get; set; }
    public string HeaderData { get; set; }
}
