namespace Silo.Application.Features;
public class GetAllDynamicFieldVm
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Title { get; set; }
    public DynamicFieldType FieldType { get; set; }
    public string FieldTypeTitle { get; set; }
    public bool IsSystematicField { get; set; }
    public bool IsHeaderKey { get; set; }
    public string UserName { get; set; }
    public string UserId { get; set; }
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
    public List<string>? ValueOptionList
    {
        get => ValueOptions?.Split('|').ToList();
        set => ValueOptions = value != null ? string.Join('|', value) : null;
    }
    public bool? IsRequired { get; set; } = false;
    public int? Order { get; set; } 
    public int? SectionId { get; set; }
    public string? Value { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public bool? IsReadOnly { get; set; } = false;
    public int? NumericValue
    {
        get => int.TryParse(Value, out var val) ? val : 0;
        set => Value = value?.ToString();
    }
}
