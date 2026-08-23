namespace Silo.Domains.Entities;
[Table("tbl_Tags")]
public class Tag
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("ProductSerial")]
    public string ProductSerial { get; set; }

    [StringLength(50)]
    [Column("ProductCode")]
    public string? ProductCode { get; set; }

    [Required]
    [StringLength(50)]
    [Column("TagEpc")]
    public string TagEpc { get; set; }

    [StringLength(50)]
    [Column("ProjectCode")]
    public string? ProjectCode { get; set; }

    [Column("ProductCount")]
    public decimal? ProductCount { get; set; }

    [Column("ProductName")]
    public string? ProductName { get; set; }

    [StringLength(256)]
    [Column("ProductType")]
    public string? ProductType { get; set; }

    [StringLength(256)]
    [Column("ProductStatus")]
    public string? ProductStatus { get; set; }

    [StringLength(50)]
    [Column("TagStatus")]
    public string? TagStatus { get; set; }

    [StringLength(50)]
    [Column("TagRegisterShamsiUnixDate")]
    public string? TagRegisterShamsiUnixDate { get; set; }

    [StringLength(50)]
    [Column("TagRegisterUser")]
    public string? RegisterUser { get; set; }

    [Column("TagTreeParentId")]
    public int? TagTreeParentId { get; set; }

    [Column("TagTreeSecondParentId")]
    public int? TagTreeSecondParentId { get; set; }

    [Column("TagTreeParentsId")]
    public string? TagTreeParentsId { get; set; }

    [StringLength(50)]
    [Column("NewProductSerial")]
    public string? NewProductSerial { get; set; }

    [Column("ProductProperties")]
    public string? ProductProperties { get; set; }

    [Column("Lock")]
    public bool? Lock { get; set; }

    [StringLength(50)]
    [Column("Username")]
    public string? Username { get; set; }

    [StringLength(50)]
    [Column("DeviceId")]
    public string? DeviceId { get; set; }

    [StringLength(50)]
    [Column("DeviceIp")]
    public string? DeviceIp { get; set; }

    [Column("Freeze")]
    public bool? Freeze { get; set; }

    [Column("Deactivate")]
    public bool? Deactivate { get; set; }

    [Column("TagInActionId")]
    public int? TagInActionId { get; set; }

    [StringLength(50)]
    [Column("TagInDestinationId")]
    public string? TagInDestinationId { get; set; }

    [Column("TagInActionId2")]
    public int? TagInActionId2 { get; set; }

    [StringLength(50)]
    [Column("TagInDestinationId2")]
    public string? TagInDestinationId2 { get; set; }

    [StringLength(50)]
    [Column("fld_ProductPropertyAId")]
    public string? ProductPropertyAId { get; set; }

    [StringLength(50)]
    [Column("fld_ProductPropertyBId")]
    public string? ProductPropertyBId { get; set; }

    [StringLength(50)]
    [Column("fld_ProductPropertyCId")]
    public string? ProductPropertyCId { get; set; }

    [StringLength(50)]
    [Column("RegCode")]
    public string? RegCode { get; set; }

    [StringLength(128)]
    [Column("fld_LastModifierUser")]
    public string? LastModifierUser { get; set; }

    [StringLength(50)]
    [Column("ContractStatus")]
    public string? ContractStatus { get; set; }

    [StringLength(50)]
    [Column("TagZone")]
    public string? TagZone { get; set; }

    [Column("TagRegisterDateTime")]
    public DateTime? TagRegisterDateTime { get; set; }

    [Column("ReProduct")]
    public bool? ReProduct { get; set; }

    [Column("fld_InspectActionId")]
    public int? InspectActionId { get; set; }

    [Column("fld_LastInspectResult")]
    public string? LastInspectResult { get; set; }

    [StringLength(128)]
    [Column("fld_ProductGroup")]
    public string? ProductGroup { get; set; }

    [StringLength(128)]
    [Column("fld_ProductBrand")]
    public string? ProductBrand { get; set; }

    [StringLength(128)]
    [Column("fld_ProductSubGroup")]
    public string? ProductSubGroup { get; set; }

    [StringLength(128)]
    [Column("fld_ProductClass")]
    public string? ProductClass { get; set; }

    [StringLength(128)]
    [Column("TagTreeParentsEpc")]
    public string? TagTreeParentsEpc { get; set; }

    [StringLength(128)]
    [Column("TagEpc2")]
    public string? TagEpc2 { get; set; }


    public ICollection<TagsMovement> TagsMovements { get; set; }
}
