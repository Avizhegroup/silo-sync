namespace Silo.Domains.Android;

[Table("tbl_ProductGroup")]
public class AndroidProductGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ProductGroupCode { get; set; }
    public string ProductGroupTitle { get; set; }

}



