namespace Silo.Domains.Entities;

[Table("tbl_ProductGroup")]
public class ProductGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductGroupId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroupCode")]
    public string Code { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroupTitle")]
    public string Title { get; set; }

    [Column("fld_ProductGroupData")]
    public string? Data { get; set; }

    public ICollection<Product> Products { get; set; }

    public ICollection<ProductSubGroup> ProductSubGroups { get; set; }
}
