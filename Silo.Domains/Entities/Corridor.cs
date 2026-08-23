namespace Silo.Domains.Entities;

[Table("tbl_Corridor")]
public class Corridor
{
    [Column("fld_CorridorId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("fld_CorridorWarehouseCode")]
    public string WarehouseCode { get; set; }
    public Warehouse Warehouse { get; set; }

    [Required]
    [StringLength(128)]
    [Column("fld_CorridorName")]
    public string CorridorName { get; set; }

    [Column("fld_CorridorDirection")]
    public int Direction { get; set; }

    [Column("fld_CorridorVerticalOrder")]
    public int VerticalOrder { get; set; }

    [Column("fld_CorridorHorizontalOrder")]
    public int HorizontalOrder { get; set; }

    [Column("fld_CorridorWidth")]
    public int Width { get; set; }
    
    [Column("fld_CorridorZoom")]
    public int Zoom { get; set; }

    [Column("fld_CorridorIsFaken")]
    public bool IsFaken { get; set; }

    public ICollection<Zone> Zones { get; set; }
}
