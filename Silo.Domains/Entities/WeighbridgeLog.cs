namespace Silo.Domains.Entities;

[Table("tbl_WeighbridgeLog")]
public class WeighBridgeLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_WeighbridgeLogId")]
    public int Id { get; set; }

    [StringLength(256)]
    [Column("fld_WeighbridgeLogWeighbridgeCode")]
    public string? WeighbridgeCode { get; set; }

    [Column("fld_WeighbridgeLogWeight")]
    public decimal? Weight { get; set; }

    [Column("fld_WeighbridgeLogPlaque")]
    public string? Plaque { get; set; }

    [Column("fld_WeighbridgeLogDateTime")]
    public DateTime? DateTime { get; set; }

    [StringLength(10)]
    [Column("fld_WeighbridgeLogShamsiDate")]
    public string? ShamsiDate { get; set; }
}