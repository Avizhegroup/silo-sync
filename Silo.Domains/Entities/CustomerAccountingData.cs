
namespace Silo.Domains.Entities;

[Table("tbl_CustomerAccountingData")]
public class CustomerAccountingData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_CADId")]
    public int Id { get; set; }

    [Column("fld_CADOpCode")]
    public int OpCode { get; set; }

    [Column("fld_CADInventoryHeaderId")]
    public int? InventoryHeaderId { get; set; }

    [Column("fld_CADDateTime")]
    public DateTime? DateTime { get; set; }
   
    [StringLength(128)]
    [Column("fld_CADUser")]
    public string? User { get; set; }

    [StringLength(256)]
    [Column("fld_NDFLName")]
    public string? FileName { get; set; }

    [StringLength(128)]
    [Column("fld_CADProductCode")]
    public string? ProductCode { get; set; }

    [Column("fld_CADProductCount")]
    public decimal? ProductCount { get; set; }

    [Column("fld_CADRealityCount")]
    public decimal? RealityCount { get; set; }
}
