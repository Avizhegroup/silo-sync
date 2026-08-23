namespace Silo.Domains.Android;

[Table("tbl_TruckType")]
public class AndroidTruckType
{
    [Key]
    [Column("fld_TruckTypeId", Order = 0)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckTypeTitle", Order = 1)]
    public string Title { get; set; }
}
