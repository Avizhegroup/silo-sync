namespace Silo.Domains.Android;

[Table("tbl_ProductType")]
public class AndroidProductType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductTypeId { get; set; }
    public string ProductTypeTitle { get; set; }
    public string ProductTypeParentId { get; set; }
    public string ProductTypeParentsId { get; set; }
    public string ProductTypeCode { get; set; }
}
