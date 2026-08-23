using Silo.Application.Features;

namespace Silo.Application.Dto;
public class DynamicFieldDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DynamicFieldType FieldType { get; set; }
    public bool IsSystematicField { get; set; }
    public bool IsHeaderKey { get; set; }
    public string UserName { get; set; }
    public string? UserId { get; set; }
    public DateTime DateTime { get; set; }
    public string RelatedTitle1 { get; set; }
    public string RelatedTitle2 { get; set; }
    public string RelatedTitle3 { get; set; }
    public int? ActionType { get; set; }
    public bool FieldShowColumn { get; set; }
    public bool FieldShowColumnForAction { get; set; }
    public DynamicFieldValueType ValueType { get; set; }
    public string? ValueOptions { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
    public int? SectionId { get; set; }
    public int? Order { get; set; }
    public int? ParentId { get; set; }
    public bool? IsReadOnly { get; set; } = false;
}
