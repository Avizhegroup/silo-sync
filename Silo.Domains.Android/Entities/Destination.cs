namespace Silo.Domains.Android;

[Table("tbl_Destination")]
public class Destination
{
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public string Title { get; set; }

    [Column(Order = 2)]
    public int? St { get; set; }

    [Column(Order = 3)]
    public string Desc { get; set; }

    [Column(Order = 4)]
    public string Code { get; set; }

    [Column(Order = 5)]
    public int? Type { get; set; }

    [Column(Order = 6)]
    public int? ParentId { get; set; }

    [Column(Order = 7)]
    public string ParentsId { get; set; }

    [Column(Order = 8)]
    public string Epc { get; set; }
}
