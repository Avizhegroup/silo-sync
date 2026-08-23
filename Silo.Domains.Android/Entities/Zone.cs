namespace Silo.Domains.Android;

[Table("tbl_Zones")]
public class Zone
{
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public string Code { get; set; }

    [Column(Order = 2)]
    public string Title { get; set; }

    [Column(Order = 3)]
    public string StoreCode  { get; set; }
}
