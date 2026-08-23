namespace Silo.Domains.Android;

[Table("tbl_TruckCrossCustomer")]
public class AndroidTruckCrossCustomer
{
    [Key]
    [Column("fld_TruckCrossCustomerId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossCustomerTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }
}
