namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossItem")]
public class TruckCrossItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_TruckCrossItemId")]
    public int Id { get; set; }

    [StringLength(256)]
    [Column("fld_TruckCrossItemTitle")]
    public string? Title { get; set; }

    [Column("fld_TruckCrossItemType")]
    public TruckCrossItemTypes? Type { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossItemProductUnit")]
    public string? ProductUnit { get; set; }

    [Column("fld_TruckCrossItemProductCount")]
    public decimal? ProductCount { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossItemProductSerial")]
    public string? ProductSerial { get; set; }

    [StringLength(50)]
    [Column("fld_TruckCrossItemProductCode")]
    public string? ProductCode { get; set; }

    [Column("fld_TruckCrossProductType")]
    public int? TruckCrossProductTypeId { get; set; }

    public TruckCrossProductType? TruckCrossProductType { get; set; }

    [Column("fld_TruckCross")]
    public long? TruckCrossId { get; set; }
    public TruckCrossData? TruckCross { get; set; }
}

public enum TruckCrossItemTypes
{
    Enter = 1,
    Exit = 2
}