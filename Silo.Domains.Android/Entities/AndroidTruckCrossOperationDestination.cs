namespace Silo.Domains.Android;

[Table("tbl_TruckCrossOperationDestination")]
public class AndroidTruckCrossOperationDestination
{
    [Key]
    [Column("fld_TruckCrossOperationDestinationId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossOperationDestinationTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }
}
