namespace Silo.Domains.Entities;

[Table("tbl_ProductPropertyC")]
public class ProductSize
{
    [Key]
    [StringLength(128)]
    [Column("fld_ProductPropertyCId")]
    public string Code { get; set; }

    [StringLength(256)]
    [Column("fld_ProductPropertyCTitle")]
    public string Title { get; set; }

    [Column("fld_ProductPropertyCDesc")]
    public string Desc { get; set; } = "";

    [Column("fld_ProductPropertyCData")]
    public string? Data { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductPropertyCIdentity")]
    public int Id { get; set; }

    [Column("fld_ProductPropertyCTemp")]
    public string? Temp { get; set; }

    public ICollection<Product> Products { get; set; }
}
