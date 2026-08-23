namespace Silo.Domains.Entities;

[Table("tbl_NonDocFileLog")]
public class NonDocFileLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_NDFLId")]
    public int Id { get; set; }

    [StringLength(256)]
    [Column("fld_NDFLName")]
    public string? FileName { get; set; }
    
    [Column("fld_NDFLDateTime")]
    public DateTime? DateTime { get; set; }
   
    [Column("fld_NDFLType")]
    public int Type { get; set; }

    [StringLength(128)]
    [Column("fld_NDFLUser")]
    public string? User { get; set; }

    [Column("fld_NDFLData")]
    public string? Data { get; set; }
}
