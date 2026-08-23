namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossOperationType")]
public class TruckCrossOperationType
{
    [Key]
    [Column("fld_TruckCrossOperationTypeId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossOperationTypeTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("fld_TruckCrossCause")]
    public int TruckCrossCauseId { get; set; }
    public TruckCrossCause TruckCrossCause { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
}
