namespace Silo.Domains.Entities;

[Table("tbl_PrintFormats")]
public class PrintFormat
{
    [Column("fld_Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_Name")]
    [MaxLength(256)]
    [Required]
    public string Name { get; set; }

    [Column("fld_PageTitle")]
    [MaxLength(256)]
    [Required]
    public string PageTitle { get; set; }

    [Column("fld_Path")]
    [Required]
    public string Path { get; set; }
}
