namespace Silo.Domains.Android;

[Table("tbl_TruckCrossShipment")]
public class AndroidTruckCrossShipment
{
    [Key]
    [Column("fld_TruckCrossShipmentId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossShipmentTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }
}
