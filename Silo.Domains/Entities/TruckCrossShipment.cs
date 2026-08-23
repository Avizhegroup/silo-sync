namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossShipment")]
public class TruckCrossShipment
{
    [Key]
    [Column("fld_TruckCrossShipmentId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossShipmentTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
    public ICollection<TruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
}
