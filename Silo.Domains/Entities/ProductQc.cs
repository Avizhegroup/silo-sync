namespace Silo.Domains.Entities;

[Table("tbl_ProductStatus")]
public class ProductQc
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ProductStatusId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("ProductStatusCode")]
    public string? Code { get; set; }

    [StringLength(128)]
    [Column("ProductStatusTitle")]
    public string? Title { get; set; }

    [Column("ProductStatusDesc")]
    public string? Desc { get; set; }

    public ICollection<Product> Products { get; set; }
}
