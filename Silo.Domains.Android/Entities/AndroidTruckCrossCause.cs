namespace Silo.Domains.Android;

[Table("tbl_TruckCrossCause")]
public class AndroidTruckCrossCause
{
    [Key]
    [Column("fld_TruckCrossCauseId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossCauseTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }

    [Column("fld_TruckCrossCauseEnterActionTypeId", Order = 2)]
    public int? EnterActionTypeId { get; set; }

    [Column("fld_TruckCrossCauseExitActionTypeId", Order = 3)]
    public int? ExitActionTypeId { get; set; }
}
