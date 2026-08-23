namespace Silo.Domains.Entities;

[Table("tbl_DocumentLog")]
public class DocumentLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_LogId")]
    public int Id { get; set; }

    [Column("fld_LogDocumentKey")]
    public string? Key { get; set; }

    [StringLength(10)]
    [Column("fld_LogDocumentType")]
    public string DocumentType { get; set; }

    [Column("fld_LogDocumentStatus")]
    public int Status { get; set; }

    [StringLength(128)]
    [Column("fld_LogUserId")]
    public string? UserId { get; set; }
    public User User { get; set; }

    [Column("fld_LogDateTime")]
    public DateTime? DateTime { get; set; }

    [StringLength(10)]
    [Column("fld_LogShamsiDate")]
    public string? ShamsiDate { get; set; }

    [Column("fld_LogEventType")]
    public int EventType { get; set; }

    [Column("fld_LogDescription")]
    public string? Description { get; set; }
}
