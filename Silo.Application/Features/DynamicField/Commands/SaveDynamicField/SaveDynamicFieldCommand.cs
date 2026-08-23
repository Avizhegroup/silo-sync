namespace Silo.Application.Features;

public class SaveDynamicFieldCommand
{
    public int Id { get; set; }

    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_Title) )]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public string Title { get; set; }
    public DynamicFieldType? FieldType { get; set; } = null;
    public bool IsSystematicField { get; set; }
    public bool IsHeaderKey { get; set; }
    public string UserName { get; set; }
    public string UserId { get; set; }
    public DateTime DateTime { get; set; }
    public string RelatedTitle1 { get; set; }
    public string RelatedTitle2 { get; set; }
    public string RelatedTitle3 { get; set; }
    public int? Order { get; set; } 
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_ActionType))]
    [Required(ErrorMessageResourceType = typeof(TextResources), ErrorMessageResourceName = nameof(TextResources.APP_StringKeys_Validation_Required))]
    public int? ActionType { get; set; }
    public DynamicFieldValueType ValueType { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValueOptions
    {
        get => ValueOptionList is not null ? string.Join('|',ValueOptionList):null;
        //set => ValueOptionList = value != null ? value.Split('|').ToList() : new();
    }

    public List<string> ValueOptionList { get; set; } = new();

    public string? ValueOption { get; set; }
   
    [Display(ResourceType = typeof(TextResources), Name = nameof(TextResources.APP_StringKeys_SectionTitle))]
    public int? SectionId { get; set; }
    public bool IsRequired { get; set; }
    public int? ParentId { get; set; }
    public bool FieldShowColumn { get; set; } = false;
    public bool FieldShowColumnForAction { get; set; } = false;
    public bool? IsReadOnly { get; set; } = false;
}
