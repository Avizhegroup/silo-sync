namespace Silo.Domains.Entities.Api;

[Table("tbl_ProductExpire")]
public class ProductExpire
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_ProductExpireId")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("fld_ProductExpireProductSerial")]
    public string? ProductSerial { get; set; }

    [StringLength(50)]
    [Column("fld_ProductExpireProductCode")]
    public string? ProductCode { get; set; }

    [Column("fld_ProductExpireStatus")]
    public int Status { get; set; }

    [StringLength(10)]
    [Column("fld_ProductExpireStartDate")]
    public string? StartDate { get; set; }
    
    [StringLength(10)]
    [Column("fld_ProductExpireEndDate")]
    public string? EndDate { get; set; }

    [Column("fld_ProductExpireActivationType")]
    public int ActivationType { get; set; }

    [Column("fld_ProductExpireLastModifiedDateTime")]
    public DateTime? LastModifiedDateTime { get; set; }

    [Column("fld_ProductExpireLastModifiedUserId")]
    [StringLength(128)]
    public string? LastModifiedUserId { get; set; }
    public User? User { get; set; }
}
