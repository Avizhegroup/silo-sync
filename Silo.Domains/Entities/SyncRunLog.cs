namespace Silo.Domains.Entities;

[Table("tbl_SyncRunLog")]
public class SyncRunLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_SyncRunLogId")]
    public int Id { get; set; }

    [Column("fld_SourceKey")]
    public string? SourceKey { get; set; }

    [Column("fld_StartedAt")]
    public DateTime? StartedAt { get; set; }

    [Column("fld_FinishedAt")]
    public DateTime? FinishedAt { get; set; }

    [Column("fld_RowsFetched")]
    public int? RowsFetched { get; set; }

    [Column("fld_RowsSucceeded")]
    public int? RowsSucceeded { get; set; }

    [Column("fld_RowsFailed")]
    public int? RowsFailed { get; set; }

    [Column("fld_Status")]
    public string? Status { get; set; }

    [Column("fld_ErrorSummary")]
    public string? ErrorSummary { get; set; }
}
