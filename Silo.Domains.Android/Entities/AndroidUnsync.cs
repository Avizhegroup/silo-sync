namespace Silo.Domains.Android.Entities;

[Table("tbl_Unsync")]
public  class AndroidUnsync
{
    [Column("optype", Order = 0)]
    public string Optype { get; set; }

    [Column("data", Order = 1)]
    public string Data { get; set; }

    [Column("saveDateTime", Order = 2)]
    public string SaveDateTime { get; set; }
}
