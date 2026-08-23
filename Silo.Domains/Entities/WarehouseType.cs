namespace Silo.Domains.Entities;

[Table("tbl_DestinationType")]
public class WarehouseType
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_Id")]
    [Key]
    public int Id { get; set; }

    [Column("fld_DestinationTypeCode")]
    [StringLength(50)]
    public string? Code { get; set; }

    [Column("fld_DestinationTypeName")]
    [StringLength(250)]
    public string? Title { get; set; }
}
