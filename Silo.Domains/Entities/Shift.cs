namespace Silo.Domains.Entities;

[Table("tbl_ProductPropertyB")]

public class Shift
{
    [Key]
    [StringLength(128)]
    [Column("fld_ProductPropertyBId")]
    public string Id { get; set; }

    [Required]
    [StringLength(512)]
    [Column("fld_ProductPropertyBTitle")]
    public string Title { get; set; }

    [StringLength(512)]
    [Column("fld_ProductPropertyBDesc")]
    public string? Desc { get; set; }

    [Required]
    [Column("fld_ProductPropertyBData")]
    public string Data { get; set; }
    
    [Column("fld_ProductPropertyAId")]
    [StringLength(128)]
    public string? LineId { get; set; }
    public Line? Line { get; set; }
}
