namespace Silo.Domains.Android;

[Table("tbl_DocumentStatus")]
public class AndroidDocumentStatus
{
    [Key]
    [Column("Id", Order = 0)]
    public int Id { get; set; }

    [Required]
    [Column("Title", Order = 1)]
    public string Title { get; set; }

    [Column("IsUpdatePermitted", Order = 2)]
    public int? IsUpdatePermitted { get; set; }

    [Column("IsCartablePermitted", Order = 3)]
    public int? IsCartablePermitted { get; set; }
}
