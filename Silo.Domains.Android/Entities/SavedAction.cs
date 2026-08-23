namespace Silo.Domains.Android;

[Table("tbl_savedActions")]
public class SavedAction
{
    [Key]
    [Column(Order = 0)]
    public string id { get; set; }

    [Column(Order = 1)]
    public string method { get; set; }

    [Column(Order = 2)]
    public string parameters { get; set; }

}
