namespace Silo.Domains.Entities;

[Table("tbl_DocumentStatus")]
public class DocumentStatus
{
    [Key]
    [Column("fld_DocumentStatusId")]
    public int Id { get; set; }

    [StringLength(256)]
    [Column("fld_DocumentStatusTitle")]
    public string Title { get; set; }

    [Column("fld_DocumentStatusIsUpdatePermitted")]
    public bool IsUpdatePermitted { get; set; }

    [Column("fld_DocumentStatusIsCartablePermitted")]
    public bool IsCartablePermitted { get; set; }

    public ICollection<DocumentHeader> DocumentHeaders { get; set; }
}
