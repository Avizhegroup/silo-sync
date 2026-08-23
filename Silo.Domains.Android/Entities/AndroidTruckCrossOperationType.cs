namespace Silo.Domains.Android;

[Table("tbl_TruckCrossOperationType")]
public class AndroidTruckCrossOperationType
{
    [Key]
    [Column("fld_TruckCrossOperationTypeId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossOperationTypeTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }

    [Required]
    [Column("fld_TruckCrossCause", Order = 2)]
    public int TruckCrossCauseId { get; set; }
}
