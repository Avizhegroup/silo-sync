namespace Silo.Domains.Entities.Api;

[Table("tbl_ProductGuarantee")]
public class ProductGuarantee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductGuaranteeId")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("fld_ProductGuaranteeProductSerial")]
    public string? ProductSerial { get; set; }

    [StringLength(50)]
    [Column("fld_ProductGuaranteeProductCode")]
    public string? ProductCode { get; set; }

    [Column("fld_ProductGuaranteeStatus")]
    public int Status { get; set; }

    [StringLength(10)]
    [Column("fld_ProductGuaranteeStartDate")]
    public string? StartDate { get; set; }
    
    [StringLength(10)]
    [Column("fld_ProductGuaranteeEndDate")]
    public string? EndDate { get; set; }

    [Column("fld_ProductGuaranteeActivationType")]
    public int ActivationType { get; set; }

    [Column("fld_ProductGuaranteeLastModifiedDateTime")]
    public DateTime? LastModifiedDateTime { get; set; }

    [Column("fld_ProductGuaranteeLastModifiedUserId")]
    [StringLength(128)]
    public string? LastModifiedUserId { get; set; }
    public User? User { get; set; }
}
