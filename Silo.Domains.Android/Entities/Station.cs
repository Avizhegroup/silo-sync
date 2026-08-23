namespace Silo.Domains.Android;

[Table("tbl_Station")]
public class Station
{
    [Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public string Code { get; set; }

    [Column(Order = 2)]
    public string Name { get; set; }

    [Column(Order = 3)]
    public int? Type { get; set; }

    [Column(Order = 4)]
    public int? ActionType { get; set; }

    [Column(Order = 5)]
    public int? Status { get; set; }

    [Column(Order = 6)]
    public string Description { get; set; }

    [Column(Order = 7)]
    public string FromDestination { get; set; }

    [Column(Order = 8)]
    public string ToDestination { get; set; }

    [Column(Order = 9)]
    public string MacAddress { get; set; }
}
