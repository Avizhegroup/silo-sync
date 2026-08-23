namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossCustomer")]
public class TruckCrossCustomer
{
    [Key]
    [Column("fld_TruckCrossCustomerId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossCustomerTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
    public ICollection<TruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
}
