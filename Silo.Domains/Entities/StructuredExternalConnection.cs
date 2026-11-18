namespace Silo.Domains.Entities;

[Table("tbl_StructuredExternalConnection")]
public class StructuredExternalConnection
{
    [Key]
    [Column("fld_SecId")]
    public int Id { get; set; }

    [Column("fld_SecType")]
    [StringLength(128)]
    public string Type { get; set; }

    [Column("fld_SecEventId")]
    [StringLength(256)]
    public string EventId { get; set; }

    [Column("fld_SecDateTime")]
    public DateTime DateTime { get; set; }

    [Column("fld_SecData")]
    public string Data { get; set; }

    [Column("fld_SecMethod")]
    [StringLength(256)]
    public string Method { get; set; }
}
