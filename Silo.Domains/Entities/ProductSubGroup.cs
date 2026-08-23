namespace Silo.Domains.Entities;

[Table("tbl_ProductSubGroup")]
public class ProductSubGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductSubGroupId")]
    public int Id { get; set; }

    [StringLength(128)]
    [Column("fld_ProductSubGroupCode")]
    public string Code { get; set; }

    [StringLength(256)]
    [Column("fld_ProductSubGroupTitle")]
    public string Title { get; set; }

    [StringLength(512)]
    [Column("fld_ProductSubGroupSubTitle")]
    public string? SubTitle { get; set; }

    [StringLength(512)]
    [Column("fld_ProductSubGroupDesc")]
    public string? Description { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroupCode")]
    public string ProductGroupCode { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ProductGroup ProductGroup { get; set; }

    public ICollection<Product> Products { get; set; }
}
