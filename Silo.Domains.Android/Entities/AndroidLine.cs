namespace Silo.Domains.Android;

[Table("tbl_ProductPropertyA")]
public class AndroidLine
{
    [Key]
    [Column("fld_ProductPropertyAId")]
    public string Id { get; set; }

    [Required]
    [Column("fld_ProductPropertyATitle")]
    public string Title { get; set; }

    [Column("fld_ProductPropertyADesc")]
    public string? Desc { get; set; }

    [Required]
    [Column("fld_ProductPropertyAData")]
    public string Data { get; set; }
}
