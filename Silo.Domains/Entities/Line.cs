namespace Silo.Domains.Entities;

[Table("tbl_ProductPropertyA")]
public class Line
{
    [Key]
    [StringLength(128)]
    [Column("fld_ProductPropertyAId")]
    public string Id { get; set; }

    [Required]
    [StringLength(256)]
    [Column("fld_ProductPropertyATitle")]
    public string Title { get; set; }

    [StringLength(512)]
    [Column("fld_ProductPropertyADesc")]
    public string? Desc { get; set; }

    
    [Column("fld_ProductPropertyAData")]
    public string? Data { get; set; }
    
    public ICollection<Shift> Shifts { get; set; }
}
