namespace Silo.Domains.Entities.Api;

[Table("tbl_ExpireGuaranteeLog")]
public class ExpireGuaranteeLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ExpireGuaranteeId")]
    public int Id { get; set; }

    [Column("fld_ExpireGuaranteeDateTime")]
    public DateTime? DateTime { get; set; }

    [Column("fld_ExpireGuaranteeDate")]
    [StringLength(10)]
    public string? Date { get; set; }

    [Column("fld_ExpireGuaranteeTime")]
    [StringLength(5)]
    public string? Time { get; set; }
    
    [Column("fld_ExpireGuaranteeProductCode")]
    [StringLength(50)]
    public string? ProductCode { get; set; }

    [Column("fld_ExpireGuaranteeProductSerial")]
    [StringLength(50)]
    public string? ProductSerial { get; set; }
    
    [Column("fld_ExpireGuaranteeUserId")]
    [StringLength(128)]
    public string? UserId { get; set; }
    public User? User { get; set; }

    [Column("fld_ExpireGuaranteeGuaranteeType")]
    public int? GuaranteeType { get; set; }

    [Column("fld_ExpireGuaranteeGuaranteeDays")]
    public int? GuaranteeDays { get; set; }

    [Column("fld_ExpireGuaranteeExpireType")]
    public int? ExpireType { get; set; }

    [Column("fld_ExpireGuaranteeExpireDays")]
    public int? ExpireDays { get; set; }

    [Column("fld_ExpireGuaranteeExpireEndDate")]
    [StringLength(10)]
    public string? ExpireEndDate { get; set; }

    [Column("fld_ExpireGuaranteeGuaranteeEndDate")]
    [StringLength(10)]
    public string? GuaranteeEndDate { get; set; }

    [Column("fld_ExpireGuaranteeGuaranteeMonths")]
    public int? GuaranteeMonths { get; set; }

    [Column("fld_ExpireGuaranteeExpireMonths")]
    public int? ExpireMonths { get; set; }
}
