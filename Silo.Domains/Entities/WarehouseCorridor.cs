namespace Silo.Domains.Entities;

[Table("tbl_WarehouseCorridor")]
public class WarehouseCorridor
{
    [Column("fld_WarehouseCorridorId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Empty = warehouse-list view; warehouseCode = zone view for that warehouse
    /// </summary>
    [StringLength(50)]
    [Column("fld_WarehouseCorridorContextKey")]
    public string ContextKey { get; set; } = string.Empty;

    [Column("fld_WarehouseCorridorX1")]
    public float X1 { get; set; }

    [Column("fld_WarehouseCorridorZ1")]
    public float Z1 { get; set; }

    [Column("fld_WarehouseCorridorX2")]
    public float X2 { get; set; }

    [Column("fld_WarehouseCorridorZ2")]
    public float Z2 { get; set; }

    [Column("fld_WarehouseCorridorWidth")]
    public float Width { get; set; } = 1.0f;

    [StringLength(200)]
    [Column("fld_WarehouseCorridorLabel")]
    public string? Label { get; set; }
}
