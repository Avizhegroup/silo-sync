namespace Silo.Domains.Entities;

[Table("tbl_DynamicFieldsSection")]
public class DynamicFieldSection
{
    [Key]
    [Column("fld_DfSectionId")]
    public int Id { get; set; }

    [Required]
    [StringLength(256)]
    [Column("fld_DfSectionTitle")]
    public string Title { get; set; }

    [Required]
    [Column("fld_DfType")]
    public int DynamicFieldType { get; set; }

    public ICollection<DynamicField> DynamicFields { get; set; }
}
