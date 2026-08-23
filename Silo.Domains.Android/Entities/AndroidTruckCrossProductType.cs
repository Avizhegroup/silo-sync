namespace Silo.Domains.Android;

[Table("tbl_TruckCrossProductType")]
public class AndroidTruckCrossProductType
{
    [Key]
    [Column("fld_TruckCrossProductTypeId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossProductTypeTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }

    [Column("fld_TruckCrossCauseIdsArray", Order = 2)]
    public string? TruckCrossCauseIdsArray { get; set; }
}
