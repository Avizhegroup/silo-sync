namespace Silo.Domains.Entities;

[Table("tbl_City")]
public class City
{
    [Key]
    [Column("fld_Id")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("fld_Name")]
    public string Title { get; set; }

    [Column("fld_ProvinceId")]
    public int ProvinceId { get; set; }
    public Province Province { get; set; }

    [Column("fld_CountryId")]
    public int CountryId { get; set; }
}
