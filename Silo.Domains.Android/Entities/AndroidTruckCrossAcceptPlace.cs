namespace Silo.Domains.Android;

[Table("tbl_TruckCrossAcceptPlace")]
public class AndroidTruckCrossAcceptPlace
{
    [Key]
    [Column("fld_TruckCrossAcceptPlaceId", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [Column("fld_TruckCrossAcceptPlaceTitle", Order = 1)]
    [StringLength(256)]
    public string Title { get; set; }
}
