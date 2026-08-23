namespace Silo.Domains.Android;

[Table("tbl_DynamicFieldsSection")]
public class AndroidDynamicFieldSection
{
    [Key]
    [Column("fld_DynamicFieldSectionId", Order = 0)]
    public int Id { get; set; }

    [Required]
    [Column("fld_DynamicFieldType", Order = 1)]
    public int DynamicFieldType { get; set; }

    [Required]
    [Column("fld_DynamicFieldSectionTitle", Order = 2)]
    public string Title { get; set; }
}
