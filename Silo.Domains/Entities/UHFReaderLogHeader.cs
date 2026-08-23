namespace Silo.Domains.Entities.Api;

[Table("tbl_UHFReaderLogHeader")]
public class UHFReaderLogHeader
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("fld_UHFReaderLogHeaderId")]
    public int Id { get; set; }

    [Column("fld_StationCode")]
    public string? StationCode { get; set; }
    public Station? Station { get; set; }

    [StringLength(128)]
    [Column("fld_ActionType")]
    public string? ActionTypeCode { get; set; }

    [StringLength(450)]
    [Column("fld_DocumentCode")]
    public string? DocumentCode { get; set; }
    public DocumentHeader? DocumentHeader { get; set; }

    [StringLength(128)]
    [Column("fld_TruckCrossId")]
    public long? TruckCrossId { get; set; }
    public TruckCrossData? TruckCross { get; set; }

    [StringLength(128)]
    [Column("fld_UHFReaderLogHeaderUserId")]
    public string? UserId { get; set; }
    public User? User { get; set; }

    [Column("fld_UHFReaderLogHeaderControlType")]
    public int? ControlType { get; set; }

    [Column("fld_CarProperties")]
    public string? CarProperties { get; set; }

    [Column("fld_HeaderUsedStatus")]
    public int? HeaderUsedStatus { get; set; }

    [Column("fld_MovementActionId")]
    public int? MovementActionId { get; set; }
    public MovementAction? MovementAction { get; set; }

    [Column("fld_HeaderCreateDateTime")]
    public DateTime? CreateDateTime { get; set; }

    public ICollection<UHFReaderLogItem> UHFReaderLogItems { get; set; }
}
