namespace Silo.Domains.Entities;

[Table("tbl_SyncCheckpoint")]
public class SyncCheckpoint
{
    [Key]
    [Column("fld_SourceKey")]
    public string SourceKey { get; set; } = string.Empty;

    [Column("fld_LastCheckpointValue")]
    public DateTime? LastCheckpointValue { get; set; }

    [Column("fld_UpdatedDate")]
    public DateTime? UpdatedDate { get; set; }
}
