namespace Silo.Domains.Android;

[Table("tbl_ProductSubGroup")]
public class AndroidProductSubGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ProductSubGroupCode { get; set; }
    public string ProductGroupCode { get; set; }   
    public string ProductSubGroupTitle { get; set; }

}


