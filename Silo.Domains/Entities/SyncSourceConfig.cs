namespace Silo.Domains.Entities;

[Table("tbl_SyncSourceConfig")]
public class SyncSourceConfig
{
    [Key]
    [Column("fld_SyncSourceConfigId")]
    public int Id { get; set; }

    [Column("fld_SourceKey")]
    public string SourceKey { get; set; } = string.Empty;

    [Column("fld_DisplayName")]
    public string? DisplayName { get; set; }

    [Column("fld_SourceType")]
    public string? SourceType { get; set; }

    [Column("fld_ConnectionStringEncrypted")]
    public string? ConnectionStringEncrypted { get; set; }

    [Column("fld_Command")]
    public string? Command { get; set; }

    [Column("fld_FieldKey")]
    public string? FieldKey { get; set; }

    [Column("fld_FieldCheck")]
    public string? FieldCheck { get; set; }

    [Column("fld_FieldOrder")]
    public string? FieldOrder { get; set; }

    [Column("fld_IntervalSeconds")]
    public int? IntervalSeconds { get; set; }

    [Column("fld_IsEnabled")]
    public bool IsEnabled { get; set; } = true;

    [Column("fld_CreatedBy")]
    public string? CreatedBy { get; set; }

    [Column("fld_CreatedDate")]
    public DateTime? CreatedDate { get; set; }

    [Column("fld_ModifiedBy")]
    public string? ModifiedBy { get; set; }

    [Column("fld_ModifiedDate")]
    public DateTime? ModifiedDate { get; set; }
}
