namespace Silo.Ui.Gate.Models;
public class DynamicFieldDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DynamicFieldType FieldType { get; set; }
    public bool IsSystematicField { get; set; }
    public bool IsHeaderKey { get; set; }
    public string UserName { get; set; }
    public string UserId { get; set; }
    public DateTime DateTime { get; set; }
    public string RelatedTitle1 { get; set; }
    public string RelatedTitle2 { get; set; }
    public string RelatedTitle3 { get; set; }
    public int ActionType { get; set; }
    public string Value { get; set; }
}

public enum DynamicFieldType
{
    ItemData,
    HeaderData
}

public class SaveDocumentStatusCommand
{
    public List<DocumentKeyTypeDto> DocumentKeyTypes { get; set; }
    public string User { get; set; }
}

public class DocumentKeyTypeDto
{
    public string Key { get; set; }
    public string Type { get; set; }
}
