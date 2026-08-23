namespace Silo.Domains.Android;

[Table("tbl_DynamicFields")]
public class AndroidDynamicFields
{
    [Key]
    [Column("fld_DynamicFieldId", Order =0)]
    public int Id { get; set; }

    [Column("fld_DynamicFieldActionType", Order = 1)]
    public string? ActionType { get; set; }

    [Column("fld_DynamicFieldTitle", Order = 2)]
    public string? FieldTitle { get; set; }

    [Column("fld_DynamicFieldValueType", Order = 3)]
    public int? ValueType { get; set; }

    [Column("fld_DynamicFieldDefaultValue", Order = 4)]
    public string? DefaultValue { get; set; }

    [Column("fld_DynamicFieldValueOptions", Order = 5)]
    public string? ValueOptions { get; set; }

    [Column("fld_DynamicFieldRequirement", Order = 6)]
    public int? IsFieldRequired { get; set; }

    [Column("fld_DynamicFieldOrder", Order = 7)]
    public int? Order { get; set; }

    [Column("fld_DynamicFieldSectionId", Order = 8)]
    public int? SectionId { get; set; }

    [Column("fld_DynamicFieldType", Order = 9)]
    public int? FieldType { get; set; }

    [Column("fld_DynamicFieldIsReadOnly", Order = 10)]
    public bool? IsReadOnly { get; set; }
}
