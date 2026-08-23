namespace Silo.Domains.Entities;

[Table("tbl_Zones")]
public class Zone
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("ZoneCode")]
    [StringLength(50)]
    public string? Code { get; set; }

    [Column("ZoneTitle")]
    [StringLength(50)]
    public string? Title { get; set; }

    [Column("ZoneCapacity")]
    public decimal? Capacity { get; set; }

    [Column("ZoneDimention")]
    [StringLength(50)]
    public string? Dimention { get; set; }
    
    [Column("ZoneParentCode")]
    [StringLength(50)]
    public string? ParentCode { get; set; }

    [Column("ZoneParentLayer")]
    public int? ParentLayer { get; set; }

    [Column("ZoneStoreCode")]
    [StringLength(50)]
    public string? WarehouseCode { get; set; }
    public Warehouse? Warehouse { get; set; }

    [Column("ZoneCountPixle")]
    public int? CountPixle { get; set; }
    
    [Column("ZoneOccupiedCapacity")]
    public int? OccupiedCapacity { get; set; }
    
    [Column("MinZoneCapacity")]
    public decimal? MinCapacity { get; set; }

    [Column("MaxZoneCapacity")]
    public decimal? MaxCapacity { get; set; }
    
    [Column("ZoneRowIndex")]
    public int? RowIndex { get; set; }

    [Column("ZoneAddress")]
    [StringLength(250)]
    public string? Address { get; set; }
    
    [Column("ZoneCorridorId")]
    public int? CorridorId { get; set; }
    public Corridor? Corridor { get; set; }

    [Column("ZoneCoordinates")]
    [StringLength(512)]
    public string? Coordinates { get; set; }
}
