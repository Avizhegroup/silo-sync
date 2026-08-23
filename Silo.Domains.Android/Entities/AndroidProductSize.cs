namespace Silo.Domains.Android;

[Table("tbl_ProductPropertyC")]
public class AndroidProductSize
{
    [Key]
    [Column("fld_ProductPropertyCId")]
    public string Id { get; set; }

    [Required]
    [Column("fld_ProductPropertyCTitle")]
    public string Title { get; set; }

     
}

