namespace Silo.Domains.Entities;

[Table("tbl_Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("ProductCode")]
    public string Code { get; set; }

    [StringLength(250)]
    [Column("ProductTitle")]
    public string? Title { get; set; }

    [StringLength(250)]
    [Column("ProductENTitle")]
    public string? ENTitle { get; set; }

    [Column("ProductPackValue")]
    public decimal? PackValue { get; set; }

    [Column("ProductPackWeight")]
    public decimal? PackWeight { get; set; }

    [Column("ProductPackVolume")]
    public decimal? PackVolume { get; set; }

    [Column("ProductCountInPack")]
    public decimal? CountInPack { get; set; }

    [Column("ProductValue")]
    public decimal? ProductValue { get; set; }

    [StringLength(50)]
    [Column("ProductTechnicalCode")]
    public string? TechnicalCode { get; set; }

    [Column("ProductProperties")]
    public string? ProductProperties { get; set; }

    [StringLength(50)]
    [Column("ProductType")]
    public string? ProductType { get; set; }
    public ProductType? ProductTypeEntity { get; set; }

    [StringLength(50)]
    [Column("ProductStatus")]
    public string? ProductQc { get; set; }
    public ProductQc? ProductQcEntity { get; set; }

    [StringLength(50)]
    [Column("ProductSize")]
    public string? ProductSize { get; set; }
    public ProductSize? ProductSizeEntity { get; set; }

    [StringLength(50)]
    [Column("ProductUnit")]
    public string? ProductUnit { get; set; }

    [StringLength(50)]
    [Column("ProductRegUser")]
    public string? RegUser { get; set; }

    [Column("ProductRegDateTime")]
    public DateTime? RegDateTime { get; set; }

    [Column("ProductGalleryId")]
    public int ProductGalleryId { get; set; }

    [Column("ProductTechnicalData")]
    public string? TechnicalData { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroup")]
    public string? ProductGroup { get; set; }
    public ProductGroup? ProductGroupEntity { get; set; }

    [StringLength(128)]
    [Column("fld_ProductBrand")]
    public string? ProductBrand { get; set; }
    public ProductBrand? ProductBrandEntity { get; set; }

    [StringLength(128)]
    [Column("fld_ProductSubGroup")]
    public string? ProductSubGroup { get; set; }
    public ProductSubGroup? ProductSubGroupEntity { get; set; }

    [StringLength(128)]
    [Column("fld_ProductClass")]
    public string? ProductClass { get; set; }
    public ProductClass? ProductClassEntity { get; set; }

    [Column("fld_ProductGuaranteeType")]
    public int? GuaranteeType { get; set; }

    [Column("fld_ProductGuaranteeDays")]
    public int? GuaranteeDays { get; set; }

    [Column("fld_ProductExpireType")]
    public int? ExpireType { get; set; }

    [Column("fld_ProductExpireDays")]
    public int? ExpireDays { get; set; }

    [Column("fld_ProductIsActive")]
    public bool? ProductIsActive { get; set; }

    [StringLength(10)]
    [Column("fld_ProductGuaranteeEndDate")]
    public string? GuaranteeEndDate { get; set; }

    [StringLength(10)]
    [Column("fld_ProductExpireEndDate")]
    public string? ExpireEndDate { get; set; }

    [Column("fld_ProductGuaranteeMonths")]
    public int? GuaranteeMonths { get; set; }

    [Column("fld_ProductExpireMonths")]
    public int? ExpireMonths { get; set; }

    public ICollection<TagsMovement> TagsMovements { get; set; }

    [Column("fld_HasDoubleTag")]
    public bool? HasDoubleTag { get; set; }
}
