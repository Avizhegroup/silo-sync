namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossOperationDestination")]
public class TruckCrossOperationDestination
{
    [Key]
    [Column("fld_TruckCrossOperationDestinationId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossOperationDestinationTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
}
