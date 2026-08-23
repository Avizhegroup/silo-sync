using Silo.Domains.Entities.Api;

namespace Silo.Domains.Entities;

[Table("tbl_MovementActions")]
public class MovementAction
{
    [Key]
    [Column("MovementActionId")]
    public int MovementActionId { get; set; }

    [Column("MovementActionTp")]
    public int? MovementActionTp { get; set; }

    [StringLength(50)]
    [Column("MovementActionStore")]
    public string? MovementActionStore { get; set; }

    [StringLength(128)]
    [Column("MovementActionUserId")]
    public string? MovementActionUserId { get; set; }

    [StringLength(10)]
    [Column("MovementActionDate")]
    public string? MovementActionDate { get; set; }

    [StringLength(5)]
    [Column("MovementActionTime")]
    public string? MovementActionTime { get; set; }

    [Column("MovementActionDateTime")]
    public DateTime? MovementActionDateTime { get; set; }

    [Column("MovementActionCountTags")]
    public int? MovementActionCountTags { get; set; }

    [StringLength(50)]
    [Column("MovementActionDestinationId")]
    public string? MovementActionDestinationId { get; set; }

    [StringLength(16)]
    [Column("MovementActionCarPlaque")]
    public string? MovementActionCarPlaque { get; set; }

    [StringLength(50)]
    [Column("MovementActionDriverName")]
    public string? MovementActionDriverName { get; set; }

    [StringLength(50)]
    [Column("MovementActionDriverMobile")]
    public string? MovementActionDriverMobile { get; set; }

    [Column("MovementActionData")]
    public string? MovementActionData { get; set; }

    [Column("MovementActionLinkId")]
    public int? MovementActionLinkId { get; set; }

    [StringLength(50)]
    [Column("MovementActionLinkDestId")]
    public string? MovementActionLinkDestId { get; set; }

    [StringLength(128)]
    [Column("MovementActionDocumentId")]
    public string? MovementActionDocumentId { get; set; }

    [Column("MovementActionDesc")]
    public string? MovementActionDesc { get; set; }

    [StringLength(256)]
    [Column("MovementActionUHFLogId")]
    public string? MovementActionUHFLogId { get; set; }

    [StringLength(30)]
    [Column("MovementActionUHFLogGate")]
    public string? MovementActionUHFLogGate { get; set; }

    [ForeignKey("TruckCrossData")]
    [Column("MovementActionTruckCrossId")]
    public long? MovementActionTruckCrossId { get; set; }
    public TruckCrossData? TruckCrossData { get; set; }

    public ICollection<TagsMovement> TagsMovements { get; set; }
    public ICollection<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
}

