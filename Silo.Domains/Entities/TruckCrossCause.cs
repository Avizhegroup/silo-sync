namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossCause")]
public class TruckCrossCause
{
    [Key]
    [Column("fld_TruckCrossCauseId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossCauseTitle")]
    [StringLength(256)]
    public string Title { get; set; }
    
    [Column("fld_TruckCrossCauseEnterActionTypeId")]
    public int? EnterActionTypeId { get; set; }
    public ActionType? EnterActionType { get; set; }
    
    [Column("fld_TruckCrossCauseExitActionTypeId")]
    public int? ExitActionTypeId { get; set; }
    public ActionType? ExitActionType { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
    public ICollection<TruckCrossOperationType> TruckCrossOperationTypes { get; set; }
}
