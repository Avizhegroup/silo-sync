namespace Silo.Domains.Entities;

[Table("tbl_ProductClass")]
public class ProductClass
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductClassId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_ProductClassCode")]
    public string Code { get; set; }

    [StringLength(256)]
    [Column("fld_ProductClassTitle")]
    public string Title { get; set; }

    [StringLength(512)]
    [Column("fld_ProductClassSubTitle")]
    public string? SubTitle { get; set; }

    [StringLength(512)]
    [Column("fld_ProductClassDesc")]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
}
