namespace Silo.Domains.Entities;

[Table("tbl_ProductType")]
public class ProductType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ProductTypeId")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("ProductTypeCode")]
    public string Code { get; set; }

    [StringLength(50)]
    [Column("ProductTypeTitle")]
    public string Title { get; set; }

    [StringLength(50)]
    [Column("ProductTypeParentId")]
    public string? ParentId { get; set; }

    [Column("ProductTypeParentsId")]
    public string? ParentsId { get; set; }

    public ICollection<Product> Products { get; set; }
}
