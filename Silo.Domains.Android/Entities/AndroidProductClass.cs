namespace Silo.Domains.Android;

[Table("tbl_ProductClass")]
public class AndroidProductClass
{
    [Key]
    [Column("Id", Order = 0)]
    public int Id { get; set; }

    [Column("Code", Order = 1)]
    public string Code { get; set; }

    [Column("Title", Order = 1)]
    public string Title { get; set; }
}
