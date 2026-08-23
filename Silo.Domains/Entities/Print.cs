namespace Silo.Domains.Entities;

[Table("tbl_Print")]
public class Print
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("ProductSerial")]
    public string? ProductSerial { get; set; }

    [StringLength(50)]
    [Column("ProductCode")]
    public string? ProductCode { get; set; }

    [StringLength(500)]
    [Column("ProductName")]
    public string? ProductName { get; set; }

    [StringLength(300)]
    [Column("ProductDescription")]
    public string? ProductDescription { get; set; }

    [StringLength(50)]
    [Column("ProductType")]
    public string? ProductType { get; set; }

    [Column("ProductCount")]
    public decimal? ProductCount { get; set; }

    [Column("ProductItemCount")]
    public decimal? ProductItemCount { get; set; }

    [Column("ProductCountInPack")]
    public decimal? ProductCountInPack { get; set; }

    [StringLength(50)]
    [Column("ProductUnit")]
    public string? ProductUnit { get; set; }

    [StringLength(50)]
    [Column("ProductSize")]
    public string? ProductSize { get; set; }

    [StringLength(50)]
    [Column("ProductRegCode")]
    public string? ProductRegCode { get; set; }

    [Column("ProductWeight")]
    public decimal? ProductWeight { get; set; }

    [Column("ProductVolume")]
    public decimal? ProductVolume { get; set; }

    [StringLength(50)]
    [Column("ProductStatus")]
    public string? ProductStatus { get; set; }

    [StringLength(50)]
    [Column("ProjectCode")]
    public string? ProjectCode { get; set; }

    [StringLength(50)]
    [Column("TagEpc")]
    public string? TagEpc { get; set; }

    [StringLength(50)]
    [Column("ProductProductionShift")]
    public string? ProductProductionShift { get; set; }

    [StringLength(50)]
    [Column("ProductProductionLine")]
    public string? ProductProductionLine { get; set; }

    [Column("ProductContractType")]
    public int? ProductContractType { get; set; }

    [Column("PackageId")]
    public int? PackageId { get; set; }

    [StringLength(50)]
    [Column("Location")]
    public string? Location { get; set; }

    [Column("PrintActionId")]
    public int? PrintActionId { get; set; }

    [StringLength(50)]
    [Column("PrintType")]
    public string? PrintType { get; set; }

    [Column("PrintActionDateTime")]
    public DateTime? PrintActionDateTime { get; set; }

    [StringLength(50)]
    [Column("PrintUser")]
    public string? PrintUser { get; set; }

    [Column("PrintFlag")]
    public int? PrintFlag { get; set; }

    [Column("RegisterActionDateTime")]
    public DateTime? RegisterActionDateTime { get; set; }

    [Column("RegisterFlag")]
    public int? RegisterFlag { get; set; }

    [Column("RegisterType")]
    public int? RegisterType { get; set; }

    [StringLength(50)]
    [Column("InputFileName")]
    public string? InputFileName { get; set; }

    [StringLength(50)]
    [Column("ErrorTime")]
    public string? ErrorTime { get; set; }

    [StringLength(200)]
    [Column("ErrorDesc")]
    public string? ErrorDesc { get; set; }

    [Column("SoftDelete")]
    public int? SoftDelete { get; set; }

    [Column("ReRegister")]
    public bool? ReRegister { get; set; }

    [Column("PrintQueue")]
    public bool? PrintQueue { get; set; }

    [StringLength(50)]
    [Column("DocumentId")]
    public string? DocumentId { get; set; }

    [StringLength(50)]
    [Column("DocumentItemId")]
    public string? DocumentItemId { get; set; }

    [StringLength(50)]
    [Column("offline_epc")]
    public string? OfflineEpc { get; set; }

    [Column("offline_reg_status")]
    public int? OfflineRegStatus { get; set; }

    [StringLength(10)]
    [Column("offline_regDate")]
    public string? OfflineRegDate { get; set; }

    [StringLength(50)]
    [Column("RegisterUser")]
    public string? RegisterUser { get; set; }

    [StringLength(50)]
    [Column("RegisterDate")]
    public string? RegisterDate { get; set; }

    [StringLength(50)]
    [Column("Manufacturer")]
    public string? Manufacturer { get; set; }

    [StringLength(50)]
    [Column("AntiFire")]
    public string? AntiFire { get; set; }

    [StringLength(50)]
    [Column("DestinationCode")]
    public string? DestinationCode { get; set; }

    [StringLength(50)]
    [Column("SoftDeleteUser")]
    public string? SoftDeleteUserId { get; set; }
    public User? SoftDeleteUser { get; set; }

    [StringLength(50)]
    [Column("SoftDeleteDate")]
    public string? SoftDeleteDate { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroup")]
    public string? ProductGroup { get; set; }

    [StringLength(128)]
    [Column("fld_ProductBrand")]
    public string? ProductBrand { get; set; }

    [Column("ProductProperties")]
    public string? ProductProperties { get; set; }

    [StringLength(128)]
    [Column("fld_ProductSubGroup")]
    public string? ProductSubGroup { get; set; }

    [StringLength(128)]
    [Column("fld_ProductClass")]
    public string? ProductClass { get; set; }
}
