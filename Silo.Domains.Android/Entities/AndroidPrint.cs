namespace Silo.Domains.Android;

[Table("tbl_Print")]
public class AndroidPrint
{
    [Key]
    [Column("Id", Order = 0)]
    public int Id { get; set; }

    [Column("fld_ProductSerial", Order = 1)]
    public string? ProductSerial { get; set; }

    [Column("fld_ProductCode", Order = 2)]
    public string ProductCode { get; set; }

    [Column("fld_ProductCount", Order = 3)]
    public decimal? ProductCount { get; set; }
}
