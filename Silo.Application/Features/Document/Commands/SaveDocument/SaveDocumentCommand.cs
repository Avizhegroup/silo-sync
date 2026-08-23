namespace Silo.Application.Features;
public class SaveDocumentCommand
{
    public string DocumentKey { get; set; }
    public string DocumentType { get; set; }
    public string DocumentType1 { get; set; }
    public string DocumentType2 { get; set; }
    public int? DocumentCheckType { get; set; }
    public List<string> DocumentJsonData { get; set; }
}
