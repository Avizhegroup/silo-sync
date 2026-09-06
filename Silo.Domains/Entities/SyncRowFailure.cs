namespace Silo.Domains.Entities;

[Table("tbl_SyncRowFailure")]
public class SyncRowFailure
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_SyncRowFailureId")]
    public int Id { get; set; }

    [Column("fld_SyncRunLogId")]
    public int? SyncRunLogId { get; set; }

    [Column("fld_SourceKey")]
    public string? SourceKey { get; set; }

    [Column("fld_RowKey")]
    public string? RowKey { get; set; }

    [Column("fld_ErrorCategory")]
    public string? ErrorCategory { get; set; }

    [Column("fld_ErrorMessage")]
    public string? ErrorMessage { get; set; }

    [Column("fld_RawPayload")]
    public string? RawPayload { get; set; }

    [Column("fld_AttemptCount")]
    public int AttemptCount { get; set; }

    [Column("fld_LastAttemptAt")]
    public DateTime? LastAttemptAt { get; set; }

    [Column("fld_NextAttemptAt")]
    public DateTime? NextAttemptAt { get; set; }

    [Column("fld_Status")]
    public string? Status { get; set; }

    [Column("fld_ResolvedDate")]
    public DateTime? ResolvedDate { get; set; }
}
