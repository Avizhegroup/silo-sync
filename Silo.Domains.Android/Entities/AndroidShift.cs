namespace Silo.Domains.Android;

[Table("tbl_ProductPropertyB")]
public class AndroidShift
{
    [Key]
    [Column("fld_ProductPropertyBId", Order = 1)]
    public string Id { get; set; }

    [Required]
    [StringLength(512)]
    [Column("fld_ProductPropertyBTitle", Order = 2)]
    public string Title { get; set; }

    [StringLength(512)]
    [Column("fld_ProductPropertyBDesc", Order = 3)]
    public string? Desc { get; set; }

    [Required]
    [Column("fld_ProductPropertyBData", Order = 4)]
    public string Data { get; set; }

    [Column("fld_ProductPropertyAId", Order = 5)]
    [StringLength(128)]
    public string? LineId { get; set; }
}
