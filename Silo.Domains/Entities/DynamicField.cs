
namespace Silo.Domains.Entities;

[Table("tbl_DynamicFields")]
public class DynamicField
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_DynamicFieldId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_DynamicFieldUser")]
    public string? UserId { get; set; }
    public User User { get; set; }

    [StringLength(50)]
    [Column("fld_DynamicFieldTitle")]
    public string? Title { get; set; }

    [Column("fld_DynamicFieldType")]
    public int FieldType { get; set; }

    [Column("fld_IsSystematicField")]
    public bool IsSystematicField { get; set; }

    [Column("fld_IsHeaderKey")]
    public bool IsHeaderKey { get; set; }

    [Column("fld_DynamicFieldDateTime")]
    public DateTime? DateTime { get; set; }

    [StringLength(50)]
    [Column("fld_DynamicFieldRelatedTitle1")]
    public string? RelatedTitle1 { get; set; }

    [StringLength(50)]
    [Column("fld_DynamicFieldRelatedTitle2")]
    public string? RelatedTitle2 { get; set; }

    [StringLength(50)]
    [Column("fld_DynamicFieldRelatedTitle3")]
    public string? RelatedTitle3 { get; set; }

    [Column("fld_DynamicFieldActionType")]
    public int? ActionType { get; set; }

    [Column("fld_DynamicFieldShowColumn")]
    public bool FieldShowColumn { get; set; }

    [Column("fld_DynamicFieldShowColumnForAction")]
    public bool FieldShowColumnForAction { get; set; }

    [Column("fld_DynamicFieldDocGroupAggregate")]
    public bool IsDocAggregateField { get; set; }

    [Column("fld_DynamicFieldValueType")]
    public int? ValueType { get; set; }

    [StringLength(128)]
    [Column("fld_DynamicFieldDefaultValue")]
    public string? DefaultValue { get; set; }

    [Column("fld_DynamicFieldValueOptions")]
    public string? ValueOptions { get; set; }

    [Column("fld_DynamicFieldRequirement")]
    public bool? IsRequired { get; set; }

    [Column("fld_DynamicFieldOrder")]
    public int? Order { get; set; }

    [Column("fld_DynamicFieldSectionId")]
    public int? SectionId { get; set; }
    public DynamicFieldSection? DynamicFieldSection { get; set; }

    [Column("fld_DynamicFieldParentId")]
    public int? ParentId { get; set; }

    [Column("fld_DynamicFieldIsReadOnly")]
    public bool? IsReadOnly { get; set; }
}
