namespace Silo.Domains.Entities;

[Table("tbl_TruckType")]
public class TruckType
{
    [Key]
    [Column("fld_TruckTypeId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckTypeTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
}
