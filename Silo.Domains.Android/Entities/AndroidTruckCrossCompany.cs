namespace Silo.Domains.Android;

[Table("tbl_TruckCompany")]
public class AndroidTruckCrossCompany
{
    [Key]
    [Column("fld_TruckCompanyId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCompanyTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }
}
