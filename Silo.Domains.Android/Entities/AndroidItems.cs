namespace Silo.Domains.Android;

[Table("tbl_Items")]
public class AndroidItems
{
    //[Key]
    [Column(Order = 0)]
    public int Id { get; set; }

    [Column(Order = 1)]
    public string? Formula { get; set; }
}

