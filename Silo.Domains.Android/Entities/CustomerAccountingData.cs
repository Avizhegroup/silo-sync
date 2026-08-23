namespace Silo.Domains.Android;

[Table("tbl_CustomerAccountingData")]
public class CustomerAccountingData
{
    [Column("fld_CADId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("fld_CADProductCode")]
    public string ProductCode { get; set; }

    [Column("fld_CADProductCount")]
    public decimal Count { get; set; }
}
