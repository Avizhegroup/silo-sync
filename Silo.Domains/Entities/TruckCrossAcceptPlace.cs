namespace Silo.Domains.Entities;

[Table("tbl_TruckCrossAcceptPlace")]
public class TruckCrossAcceptPlace
{
    [Key]
    [Column("fld_TruckCrossAcceptPlaceId")]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossAcceptPlaceTitle")]
    [StringLength(256)]
    public string Title { get; set; }

    public ICollection<TruckCrossData> TruckCrosses { get; set; }
}
