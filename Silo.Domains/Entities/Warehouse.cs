namespace Silo.Domains.Entities;

[Table("tbl_Destination")]
public class Warehouse
{
    [Column("DestinationId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("DestinationTitle")]
    [StringLength(50)]
    public string? Title { get; set; }

    [Key]
    [Column("DestinationCode")]
    [StringLength(50)]
    public string? Code { get; set; }

    [Column("DestinationSt")]
    public int? IsActive { get; set; }

    [Column("DestinationParentId")]
    public int? IsDefault { get; set; }

    [Column("DestinationDesc")]
    public string? InventoryType { get; set; }
    
    [Column("DestinationType")]
    public int? OperationalType { get; set; }

    [Column("DestinationParentsId")]
    public string? Parents { get; set; }
    
    [Column("DestinationEpc")]
    public string? Permissions { get; set; }

    [Column("DestinationCoordinates")]
    [StringLength(512)]
    public string? Coordinates { get; set; }

    public ICollection<Corridor> Corridors { get; set; }
    public ICollection<Zone> Zones { get; set; }
}
