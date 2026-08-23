namespace Silo.Domains.Android;

[Table("tbl_ProductBrand")]
public class AndroidProductBrand
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ProductBrandCode { get; set; }
    public string ProductBrandTitle { get; set; }

}





