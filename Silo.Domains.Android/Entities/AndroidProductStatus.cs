namespace Silo.Domains.Android;

[Table("tbl_ProductStatus")]
public class AndroidProductStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ProductStatusCode { get; set; }
    public string ProductStatusTitle { get; set; }
    
}
