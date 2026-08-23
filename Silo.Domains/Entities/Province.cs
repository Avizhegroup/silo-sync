namespace Silo.Domains.Entities;

[Table("tbl_Province")]
public class Province
{
    [Key]
    [Column("fld_Id")]
    public int Id { get; set; }

    [StringLength(50)]
    [Column("fld_Name")]
    public string Title { get; set; }

    public ICollection<City> Cities { get; set; }
}
