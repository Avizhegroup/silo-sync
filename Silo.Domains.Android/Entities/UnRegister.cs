namespace Silo.Domains.Android;

[Table("tbl_unregister")]
public class UnRegister
{
    [Key]
    [Column(Order = 0)]
    public string tagEpc { get; set; }

    [Column(Order = 1)]
    public string barcode { get; set; }

}

