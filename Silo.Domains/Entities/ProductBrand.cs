namespace Silo.Domains.Entities;

[Table("tbl_ProductBrand")]
public class ProductBrand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductBrandId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_ProductBrandCode")]
    public string Code { get; set; }

    [StringLength(128)]
    [Column("fld_ProductBrandTitle")]
    public string Title { get; set; }

    [Column("fld_ProductBrandData")]
    public string? Data { get; set; }

    public ICollection<Product> Products { get; set; }
}
