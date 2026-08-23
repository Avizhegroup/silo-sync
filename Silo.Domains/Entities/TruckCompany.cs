namespace Silo.Domains.Entities;

[Table("tbl_TruckCompany")]
public class TruckCrossCompany
{
    [Key]
    [Column("fld_TruckCompanyId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCompanyTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
    public ICollection<TruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
}
